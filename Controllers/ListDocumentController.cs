using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN.ViewModel;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Spire.Doc;
using Spire.Presentation;
using Spire.Xls;
using Newtonsoft.Json;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;
using Spire.Pdf.Texts;
using System.Reflection.PortableExecutable;
using System.Text.Json;
namespace DOAN.Controllers
{
    public class ListDocumentController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;
        public ListDocumentController(DataContext context, IWebHostEnvironment env)
        {
            _env = env;           
            _context = context;
        }
        public IActionResult Index(string keyword = "", string sortBy = "default", int page = 1, int pageSize = 9)
        {
            var documents = _context.Documents
                .Include(d => d.Author)
                .AsQueryable();

            // --- Tìm kiếm ---
            if (!string.IsNullOrEmpty(keyword))
            {
                documents = documents.Where(d => d.Title.Contains(keyword));
            }

            // --- Sắp xếp theo tiêu chí ---
            switch (sortBy)
            {
                case "downloads":
                    documents = documents
                        .OrderByDescending(d => _context.Downloads.Count(dl => dl.DocumentID == d.DocumentID));
                    break;

                case "rating":
                    documents = documents
                        .OrderByDescending(d => _context.Ratings
                            .Where(r => r.DocumentID == d.DocumentID)
                            .Select(r => r.RatingValue)
                            .DefaultIfEmpty(0)
                            .Average());
                    break;

                default:
                    documents = documents.OrderByDescending(d => d.UploadDate);
                    break;
            }

            // --- Tổng số bản ghi ---
            var totalRecords = documents.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // --- Phân trang ---
            var data = documents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // --- Map sang ViewModel nếu cần ---
            var result = data.Select(d => new DocumentViewModel
            {
                DocumentID = d.DocumentID,
                Title = d.Title,
                Description = d.Description,
                FileType = d.FileType,
                FilePath = d.FilePath,
                Status = d.Status,
                CategoryID = d.CategoryID,
                PublisherID = d.PublisherID,
                AuthorID = d.AuthorID,
                AuthorName = d.Author != null ? d.Author.Name : "Không rõ",
                UploadedBy = d.UploadedBy,
                Image = d.Image,
                UploadDate = d.UploadDate,
                DocumentPoint = d.DocumentPoint,
              //  DownloadCount = _context.Downloads.Count(dl => dl.DocumentID == d.DocumentID),
              
            }).ToList();

            // --- Truyền thông tin phân trang và tìm kiếm về View ---
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;
            ViewBag.SortBy = sortBy;

            return View(result);
        }


        [HttpGet]
        [Route("ListDocument/SuggestTitles")]
        public JsonResult SuggestTitles(string term)
        {
            var suggestions = _context.Documents
                .Where(d => d.Title.Contains(term))
                .Select(d => d.Title)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(suggestions);
        }



        [Route("/document-{slug}-{id:int}.html", Name = "Detail")]
        // GET: Documents/Detail/5
        public IActionResult Detail(int id)
        {
            var document = _context.Documents
                .Include(d => d.Author)
                .FirstOrDefault(d => d.DocumentID == id);

            if (document == null)
                return NotFound();

            // 🆕 Lấy tên loại tài liệu
            var docTypeName = _context.Categories
                .Where(t => t.CategoryID == document.CategoryID)
                .Select(t => t.Name)
                .FirstOrDefault();

            // 🆕 Lấy tên nhà xuất bản
            var publisherName = _context.Publishers
                .Where(p => p.PublisherID == document.PublisherID)
                .Select(p => p.Name)
                .FirstOrDefault();

            // Lấy comment + tên người dùng
            var comments = (from c in _context.Comments
                            join u in _context.Users on c.UserID equals u.UserID
                            where c.DocumentID == id
                            select new CommentWithUserViewModel
                            {
                                CommentID = c.CommentID,
                                Content = c.Content,
                                CreatedAt = c.CreatedAt,
                                FullName = u.FullName,
                                DocumentID = c.DocumentID
                            }).ToList();

            var viewModel = new DocumentDetailViewModel
            {
                Document = document,
                Comments = comments,
                NewComment = new tblComments { DocumentID = id },

                // 🆕 Gán tên loại và nhà xuất bản
                DocumentTypeName = docTypeName,
                PublisherName = publisherName
            };

            return View(viewModel);
        }


        // Nhập thêm bình luận
        [HttpPost]
        public IActionResult PostComment([FromBody] tblComments comment)
        {
            var userId = User.FindFirst("UserID")?.Value; ;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return Unauthorized(" ID không hợp lệ.");
            }
            if (userId == null)
            {
                return Json(new { success = false, message = "Bạn chưa đăng nhập." });
            }
            comment.UserID = id;
            comment.CreatedAt = DateTime.Now;

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        //Tải file tài liệu
        public IActionResult DownloadFile(int id, string type)
        {
            // 1. Lấy thông tin tài liệu
            var document = _context.Documents.FirstOrDefault(d => d.DocumentID == id);
            if (document == null || string.IsNullOrEmpty(document.FilePath))
                return NotFound("Tài liệu không tồn tại.");

            string uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "Documents");
            string originalPath = Path.Combine(uploadsFolder, document.FilePath);

            if (!System.IO.File.Exists(originalPath))
                return NotFound("Tệp không tồn tại trên máy chủ.");

            // 2. Lấy thông tin người dùng đăng nhập
            var userId = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int uid))
                return Unauthorized("ID người dùng không hợp lệ.");

            var user = _context.Users.FirstOrDefault(u => u.UserID == uid);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            // 3. Kiểm tra đã từng tải chưa
            bool hasDownloaded = _context.Downloads.Any(dl => dl.DocumentID == id && dl.UserID == uid);

            // 4. Lấy số điểm yêu cầu từ tài liệu
            int cost = document.DocumentPoint ?? 0;

            // 5. Kiểm tra nếu chưa từng tải và tài liệu có phí => kiểm tra điểm
            if ((type == "file" || type== "pdf") && cost > 0 && !hasDownloaded)
            {
                if (user.UserPoint < cost)
                {
                    return BadRequest("Bạn không đủ điểm để tải tài liệu này.");
                }

                // Trừ điểm người tải
                user.UserPoint -= cost;
                _context.Users.Update(user);

                // Cộng điểm cho người đăng (nếu không phải là chính mình)
                if (document.UploadedBy != user.UserID)
                {
                    var uploader = _context.Users.FirstOrDefault(u => u.UserID == document.UploadedBy);
                    if (uploader != null)
                    {
                        uploader.UserPoint = (uploader.UserPoint ?? 0) + cost;
                        _context.Users.Update(uploader);
                    }
                }

                // Ghi log điểm
                _context.Transactions.Add(new tblTransactions
                {
                    UserID = uid,
                    PointsChanged = -cost,
                    Action = "Download",
                    Description = $"Tải tài liệu: {document.Title}",
                    CreatedAt = DateTime.Now
                });

                if (document.UploadedBy != user.UserID)
                {
                    _context.Transactions.Add(new tblTransactions
                    {
                        UserID = document.UploadedBy ?? 0,
                        PointsChanged = cost,
                        Action = "Earn",
                        Description = $"Tài liệu được tải: {document.Title}",
                        CreatedAt = DateTime.Now
                    });
                }
            }

            // 6. Ghi lại lịch sử tải (dù là tài liệu miễn phí hoặc đã tải trước đó)
            var download = new tblDownload
            {
                DocumentID = document.DocumentID,
                UserID = uid,
                DownloadDate = DateTime.Now
            };
            _context.Downloads.Add(download);

            _context.SaveChanges();

            // 7. Trả file hoặc PDF
            string ext = Path.GetExtension(originalPath).ToLowerInvariant();

            if (type == "file")
            {
                return PhysicalFile(originalPath, GetMimeType(originalPath), Path.GetFileName(originalPath));
            }
            else if (type == "pdf")
            {
                try
                {
                    byte[] pdfBytes = ConvertToPdf(originalPath, ext);
                    if (pdfBytes == null)
                        return BadRequest("Không hỗ trợ định dạng tệp này để chuyển PDF.");

                    string pdfName = Path.GetFileNameWithoutExtension(originalPath) + ".pdf";
                    return File(pdfBytes, "application/pdf", pdfName);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Lỗi chuyển đổi sang PDF: " + ex.Message);
                }
            }

            return BadRequest("Loại yêu cầu không hợp lệ.");
        }




        //Chuyển đổi tệp gốc sang PDF
        private byte[]? ConvertToPdf(string path, string ext)
        {
            using var ms = new MemoryStream();

            if (ext == ".doc" || ext == ".docx")
            {
                var doc = new Spire.Doc.Document();
                doc.LoadFromFile(path);
                doc.SaveToStream(ms, Spire.Doc.FileFormat.PDF);
            }
            else if (ext == ".ppt" || ext == ".pptx")
            {
                var pres = new Spire.Presentation.Presentation();
                pres.LoadFromFile(path);
                pres.SaveToFile(ms, Spire.Presentation.FileFormat.PDF);
            }
            else if (ext == ".xls" || ext == ".xlsx")
            {
                var wb = new Spire.Xls.Workbook();
                wb.LoadFromFile(path);
                wb.SaveToStream(ms, Spire.Xls.FileFormat.PDF);
            }
            else
            {
                return null;
            }

            return ms.ToArray();
        }
        private string GetMimeType(string filePath)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            return provider.TryGetContentType(filePath, out var contentType) ? contentType : "application/octet-stream";
        }
        public IActionResult PreviewFile(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.DocumentID == id);
            if (document == null)
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "Uploads", "Documents", document.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Không tìm thấy file.");

            var ext = Path.GetExtension(filePath).ToLower();

            // Nếu file đã là PDF thì trả về trực tiếp
            if (ext == ".pdf")
            {
                var pdfBytes = System.IO.File.ReadAllBytes(filePath);
                return File(pdfBytes, "application/pdf");
            }

            // Nếu không phải PDF thì chuyển đổi
            var convertedBytes = ConvertToPdf(filePath, ext);
            if (convertedBytes == null)
                return BadRequest("Không thể chuyển đổi file này sang PDF.");

            return File(convertedBytes, "application/pdf");
        }



        // Tóm tắt nội dung bằng Gemini
        public async Task<string> SummarizeWithGeminiAsync(string inputText)
        {
            using (var httpClient = new HttpClient())
            {
                string apiKey = "AIzaSyAVNir24h6pF1H50oZfSE-Ccn1SxZ9cMoY"; // 🔑 Thay bằng API Key thật
                string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                new {
                    parts = new[]
                    {
                        new { text = $"Hãy tóm tắt đoạn văn sau:\n\n{inputText}" }
                    }
                }
            }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseString);

                try
                {
                    return result.candidates[0].content.parts[0].text;
                }
                catch
                {
                    return "❌ Lỗi khi tóm tắt nội dung bằng Gemini.";
                }
            }
        }

        // Tóm tắt tài liệu
        [HttpPost]
        public async Task<IActionResult> Summarize(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.DocumentID == id);
            if (document == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "Uploads", "Documents", document.FilePath);
            if (!System.IO.File.Exists(filePath)) return NotFound("Không tìm thấy file.");

            string fileText;
            try
            {
                fileText = ReadFileByExtension(filePath);
            }
            catch (Exception ex)
            {
                return Content("❌ Lỗi đọc file: " + ex.Message);
            }

            string summary = await SummarizeWithGeminiAsync(fileText);

            ViewBag.Summary = summary;
            var viewModel = new DocumentDetailViewModel
            {
                Document = document,
                Comments = new List<CommentWithUserViewModel>(),
                NewComment = new tblComments { DocumentID = id }
            };

            return View("Detail", viewModel);
        }
        public string ReadFileByExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".docx" => ReadDocx(filePath),
                ".txt" => ReadTxt(filePath),
                ".pdf" => ReadPdf(filePath),
                _ => throw new NotSupportedException("Định dạng file không hỗ trợ.")
            };
        }
        public string ReadPdf(string filePath)
        {
            var text = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }

            return text.ToString();
        }
        public string ReadDocx(string filePath)
        {
            using var wordDoc = WordprocessingDocument.Open(filePath, false);
            var body = wordDoc.MainDocumentPart?.Document?.Body;
            return body?.InnerText ?? "";
        }
        public string ReadTxt(string filePath)
        {
            return System.IO.File.ReadAllText(filePath);
        }
        // 
        public string ExtractContentFromFile(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();

            switch (ext)
            {
                case ".txt":
                    return System.IO.File.ReadAllText(filePath);

                case ".docx":
                    using (var doc = WordprocessingDocument.Open(filePath, false))
                    {
                        return doc.MainDocumentPart.Document.Body.InnerText;
                    }

                case ".pdf":
                    var text = new StringBuilder();
                    using (PdfDocument document = PdfDocument.Open(filePath))
                    {
                        foreach (Page page in document.GetPages())
                        {
                            text.AppendLine(page.Text);
                        }
                    }
                    return text.ToString();

                default:
                    return "Không hỗ trợ định dạng này.";
            }
        }
        public IActionResult ExtractContent(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.DocumentID == id);
            if (document == null)
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "Uploads", "Documents", document.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Không tìm thấy file.");

            var content = ExtractContentFromFile(filePath);

            return Content(content); // hoặc truyền qua ViewModel để render ra View
        }
        [HttpPost]
        public async Task<IActionResult> AskAI(int id, string question)
        {
            var document = _context.Documents.FirstOrDefault(d => d.DocumentID == id);
            if (document == null)
                return NotFound("Không tìm thấy tài liệu.");

            var filePath = Path.Combine(_env.WebRootPath, "Uploads", "Documents", document.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại.");

            string content = ExtractContentFromFile(filePath); // bạn đã có hàm này
            string prompt = $"Dựa trên tài liệu sau đây:\n\"\"\"\n{content}\n\"\"\"\nTrả lời câu hỏi: {question}";

            string geminiKey = "AIzaSyAVNir24h6pF1H50oZfSE-Ccn1SxZ9cMoY";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={geminiKey}";

            var requestBody = new
            {
                contents = new[]
                {
            new {
                parts = new[] {
                    new { text = prompt }
                }
            }
        }
            };

            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
                return BadRequest("Không thể kết nối với AI.");

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonDocument.Parse(json);
            var text = obj.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Content(text ?? "Không có phản hồi từ AI.");
        }
    }

}

