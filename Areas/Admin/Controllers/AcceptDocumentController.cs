using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AcceptDocumentController : Controller
    {
        private readonly DataContext _context;

        public AcceptDocumentController(DataContext context)
        {
            _context = context;
        }

        // Danh sách tài liệu chờ duyệt
        public IActionResult Index()
        {
            var docList = _context.Documents
                .Include(d => d.Author)
                .Include(d => d.UploadedUser) // nếu bạn có navigation property tới tblUsers
                .Where(d => d.Status == "Chờ phê duyệt")
                .OrderBy(d => d.DocumentID)
                .ToList();

            return View(docList);
        }

        // Phê duyệt tài liệu
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var doc = _context.Documents.Find(id);
            if (doc != null && doc.Status?.Trim().ToLower() == "chờ phê duyệt")
            {
                doc.Status = "Phê duyệt";

                // Cộng điểm cho người đăng
                var user = _context.Users.FirstOrDefault(u => u.UserID == doc.UploadedBy);
                if (user != null)
                {
                    user.UserPoint = (user.UserPoint ?? 0) + 5;
                    _context.Users.Update(user);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Từ chối tài liệu với lý do
        [HttpPost]
        public IActionResult Reject(int id, string reason)
        {
            var doc = _context.Documents.Find(id);
            if (doc == null || doc.Status != "Chờ phê duyệt")
                return NotFound();

            doc.Status = "Từ chối";
            _context.Documents.Update(doc);

            var user = _context.Users.FirstOrDefault(u => u.UserID == doc.UploadedBy);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                SendRejectionEmail(user.Email, doc.Title, reason);
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Gửi email lý do từ chối
        private void SendRejectionEmail(string toEmail, string documentTitle, string reason)
        {
            string subject = "Tài liệu của bạn đã bị từ chối";
            string body = $@"
                <p>Xin chào,</p>
                <p>Tài liệu <strong>{documentTitle}</strong> của bạn đã bị <span style='color:red;'>từ chối</span> bởi quản trị viên.</p>
                <p><strong>Lý do:</strong> {reason}</p>
                <p>Vui lòng chỉnh sửa và gửi lại nếu cần.</p>";

            using (var message = new MailMessage("your-email@example.com", toEmail))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.yourserver.com", 587))
                {
                    client.Credentials = new NetworkCredential("your-email@example.com", "your-password");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }
    }
}
