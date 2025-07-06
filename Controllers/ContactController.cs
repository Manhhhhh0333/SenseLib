using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using DOAN.ViewModel;
namespace DOAN.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _context;
        public ContactController(DataContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            return View();
        }

        // POST: Contact/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string fullName = $"{model.FirstName} {model.LastName}";

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(model.Email);
                        mail.To.Add("phamminhkhanh614@gmail.com"); // Email quản trị viên
                        mail.Subject = "Liên hệ từ người dùng";
                        mail.IsBodyHtml = true;

                        mail.Body = $@"
                        <p><strong>Họ tên:</strong> {fullName}</p>
                        <p><strong>Email:</strong> {model.Email}</p>
                        <p><strong>Số điện thoại:</strong> {model.Phone}</p>
                        <p><strong>Nội dung:</strong></p>
                        <p>{model.Message}</p>
                    ";

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential("pmk20030309@gmail.com", "sxhp fhdo wspm kmcf");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }

                    TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Gửi liên hệ thất bại. Vui lòng thử lại sau.";
                    // Log lỗi nếu cần
                }

                return RedirectToAction("Index");
            }

            return View("Index", model);
        }
    }
}
