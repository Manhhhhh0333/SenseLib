using DOAN.Models;
using DOAN.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DOAN.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly DataContext _context;

        public UserController(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Username == model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại");
                }
                else if (user.Status == false)
                {
                    ModelState.AddModelError("", " Tài khoản đã bị khóa do ad");
                }
                else if (user.IsLocked == true)
                {
                    ModelState.AddModelError("", $"Tài khoản đang bị tạm khóa đến {user.LockoutEndTime?.ToString("HH:mm:ss dd/MM/yyyy")}");
                }
                else
                {
                    string hashedPassword = model.Passwords;

                    if (user.PasswordHash != hashedPassword)
                    {
                        user.FailedLoginAttempts++;

                        if (user.FailedLoginAttempts >= 3)
                        {
                            user.LockoutEndTime = DateTime.Now.AddMinutes(1); // khóa 5 phút
                            ModelState.AddModelError("", "Tài khoản đã bị khóa tạm thời do nhập sai nhiều lần. Vui lòng thử lại sau.");
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Sai mật khẩu. Bạn còn {3 - user.FailedLoginAttempts} lần thử.");
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Đăng nhập thành công
                        user.FailedLoginAttempts = 0;
                        user.LockoutEndTime = null;
                        _context.SaveChanges();

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(principal);

                        return Url.IsLocalUrl(ReturnUrl) ? Redirect(ReturnUrl) : RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                var existUser = _context.Users.FirstOrDefault(u => u.Username == model.UserName);
                if (existUser != null)
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                var user = new tblUsers
                {
                    Username = model.UserName,
                    PasswordHash = model.Passwords, // có thể thêm hash sau nếu muốn
                    FullName = model.FullName,
                    Email = model.Email,
                    CreatedAt = DateTime.Now,
                    Status = true,
                    Role = "User"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "User");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("Email", "Không tìm thấy người dùng với email này.");
                    return View(model);
                }

                // Tạo mã token đơn giản (có thể dùng Guid hoặc đoạn mã ngẫu nhiên)
                string token = Guid.NewGuid().ToString();
                string resetLink = Url.Action("ResetPassword", "User", new { email = user.Email, token = token }, Request.Scheme);

                // Lưu token vào TempData hoặc DB tùy ý (ở đây tạm dùng TempData)
                TempData["ResetToken_" + user.Email] = token;

                // Gửi email
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("pmk20030309@gmail.com");
                    mail.To.Add(user.Email);
                    mail.IsBodyHtml = true;
                    mail.Subject = "Khôi phục mật khẩu";

                    mail.Body = $@"
                <p>Xin chào {user.FullName},</p>
                <p>Bạn đã yêu cầu khôi phục mật khẩu. Hãy nhấp vào liên kết dưới đây để đặt lại mật khẩu:</p>
                <p><a href='{resetLink}'>Đặt lại mật khẩu</a></p>
                <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>";

                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential("pmk20030309@gmail.com", "sxhp fhdo wspm kmcf");
                        smtpClient.Port = 587;
                        smtpClient.EnableSsl = true;
                        smtpClient.Send(mail);
                    }
                }

                ViewBag.Message = "Liên kết đặt lại mật khẩu đã được gửi về email của bạn.";
                return RedirectToAction("Success");
            }
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (TempData["ResetToken_" + email]?.ToString() != token)
            {
                return BadRequest("Liên kết không hợp lệ hoặc đã hết hạn.");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPasswordPost(string email, string newPassword)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            user.PasswordHash = newPassword; // Băm nếu cần
            _context.SaveChanges();

            TempData.Remove("ResetToken_" + email);
            return RedirectToAction("Login");
        }
        public IActionResult Success()
        {
            return View();
        }
        [Authorize]
        public IActionResult DownloadHistories()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("ID người dùng không hợp lệ.");
            }

            // Lịch sử tải xuống
            var downloadHistory = (from d in _context.Downloads
                                   join doc in _context.Documents on d.DocumentID equals doc.DocumentID
                                   join user in _context.Users on d.UserID equals user.UserID
                                   where d.UserID == userId
                                   select new DownloadHistoryViewModel
                                   {
                                       DocumentID = doc.DocumentID,
                                       DocumentName = doc.Title,
                                       UserName = user.FullName,
                                       DownloadDate = d.DownloadDate,
                                       RatedStar = _context.Ratings
                                                       .Where(r => r.DocumentID == doc.DocumentID && r.UserID == userId)
                                                       .Select(r => (int?)r.RatingValue)
                                                       .FirstOrDefault()
                                   }).ToList();


            // Lịch sử upload
            var uploadHistory = (from doc in _context.Documents
                                 where doc.UploadedBy == userId
                                 orderby doc.UploadDate descending
                                 select new UploadHistoryViewModel
                                 {
                                     DocumentID = doc.DocumentID,
                                     DocumentName = doc.Title,
                                     UploadDate = doc.UploadDate
                                 }).ToList();

            // Gộp vào ViewModel cha
            var model = new HistoryPageViewModel
            {
                DownloadHistories = downloadHistory,
                UploadHistories = uploadHistory
            };

            return View(model);
        }

        // Controller Action
        [Authorize]
        public IActionResult FavoriteDocuments()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("ID người dùng không hợp lệ.");
            }

            var favorites = (from fav in _context.Favorites
                             join doc in _context.Documents on fav.DocumentID equals doc.DocumentID
                             where fav.UserID == userId
                             select new FavoriteDocumentViewModel
                             {
                                 DocumentID = doc.DocumentID,
                                 Title = doc.Title,
                                 Author = doc.Author.Name,
                                 ImagePath = doc.Image // giả sử có trường hình ảnh
                             }).ToList();

            return View(favorites);
        }
        // Toggle yêu thích tài liệu
        public ActionResult ToggleFavorite(int documentId)
        {
            // Kiểm tra người dùng đã đăng nhập chưa
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var existingFavorite = _context.Favorites
            .FirstOrDefault(f => f.UserID == userId && f.DocumentID == documentId);

            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite); // CHỈ xóa bản ghi này
                TempData["FavoriteMessage"] = "Đã xóa khỏi danh sách yêu thích!";
                _context.SaveChanges();
            }
            else
            {
                // Chưa yêu thích → thêm mới
                var newFavorite = new tblFavorites
                {
                    UserID = userId,
                    DocumentID = documentId,
                    CreatedAt = DateTime.Now
                };
                _context.Favorites.Add(newFavorite);
                TempData["FavoriteMessage"] = "Đã thêm vào danh sách yêu thích!";
                _context.SaveChanges();
            }



            return RedirectToAction("FavoriteDocuments", "User");
        }
        [Authorize]
        public ActionResult Profile()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var user = _context.Users.Find(userId);


            var profileVM = new UpdateProfileVM
            {
                Id = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                Point = user.UserPoint
            };

            ViewBag.ChangePasswordVM = new ChangePasswordVM { Id = user.UserID };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UpdateProfileVM model)
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ.";
                return RedirectToAction("Profile");
            }

            var user = _context.Users.Find(userId);

            user.FullName = model.FullName;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["PasswordError"] = "Vui lòng kiểm tra lại thông tin.";
                return RedirectToAction("Profile");
            }

            var user = _context.Users.Find(model.Id);

            if (user.PasswordHash != model.CurrentPassword)
            {
                TempData["PasswordError"] = "Mật khẩu hiện tại không đúng.";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = model.NewPassword;
            _context.SaveChanges();

            TempData["PasswordSuccess"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("Profile");
        }
        [HttpGet]
        public IActionResult Upload()
        {
            var vm = new UploadDocumentVM
            {
                Authors = _context.Authors.Select(a => new SelectListItem { Value = a.AuthorID.ToString(), Text = a.Name }).ToList(),
                Categories = _context.Categories.Select(c => new SelectListItem { Value = c.CategoryID.ToString(), Text = c.Name }).ToList(),
                Publishers = _context.Publishers.Select(p => new SelectListItem { Value = p.PublisherID.ToString(), Text = p.Name }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadDocumentVM vm)
        {
            // Lấy ID người dùng
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var document = new tblDocuments
            {
                Title = vm.Title,
                AuthorID = vm.AuthorID,
                CategoryID = vm.CategoryID,
                PublisherID = vm.PublisherID,
                Description = vm.Description,
                FileType = "Tệp tải lên",
                FilePath = Path.GetFileName(vm.FilePath),
                UploadDate = DateTime.Now,
                UploadedBy = userId,
                Image = vm.Image,
                DocumentPoint = vm.DocumentPoint,
                Status = "Chờ phê duyệt"
            };
            var transaction = new tblTransactions
            {
                UserID = userId,
                Action = "Đăng tải tài liệu",
                Description = "Đăng tải tài liệu" + vm.Title,
                CreatedAt = DateTime.Now
            };
            _context.Transactions.Add(transaction);// Lưu vào trang hoạt động
            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Tải tài liệu thành công!";
            return RedirectToAction("Upload");
        }
        [HttpPost]
        public IActionResult RateDocument(int documentId, int star)
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("ID người dùng không hợp lệ.");
            }

            var existingRating = _context.Ratings
                .FirstOrDefault(r => r.DocumentID == documentId && r.UserID == userId);

            if (existingRating != null)
            {
                existingRating.RatingValue = star;
                existingRating.CreatedAt = DateTime.Now;
                _context.Ratings.Update(existingRating);
            }
            else
            {
                var newRating = new tblRating
                {
                    DocumentID = documentId,
                    UserID = userId,
                    RatingValue = star,
                    CreatedAt = DateTime.Now
                };
                _context.Ratings.Add(newRating);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("DownloadHistories", "User"); // hoặc trang bạn đang hiển thị
        }
    }

}
