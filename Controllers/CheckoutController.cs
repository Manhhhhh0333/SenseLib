using DOAN.Models;
using DOAN.ViewModel;
using DOAN.Services.Momo;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DOAN.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _context;
        private IMomoService _momoService;
        public CheckoutController(DataContext context, IMomoService momoService)
        {
            _context = context;
            _momoService = momoService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult PaymentCallBack(MomoOrderInfo model)
        {

            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            if (requestQuery["resultCode"] != "0") // Giao dịch thành công
            {
                // Lấy thông tin người dùng từ Claims
                var userIdClaim = User.FindFirst("UserID")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                // Lấy người dùng từ database
                var user = _context.Users.FirstOrDefault(u => u.UserID == userId);
                if (user != null)
                {
                    // Lấy số tiền đã thanh toán
                    decimal amount = decimal.Parse(requestQuery["amount"]);

                    // Quy đổi điểm (Ví dụ: 1.000 VNĐ = 1 điểm)
                    int points = (int)(amount / 1000);

                    // Cộng điểm vào User
                    user.UserPoint = (user.UserPoint ?? 0) + points;

                    _context.Update(user);
                }

                // Lưu giao dịch MoMo
                var momo = new MomoOrderInfo
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Name),
                    Amount = decimal.Parse(requestQuery["amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    UserID = userIdClaim,
                    DatePaid = DateTime.Now
                };

                _context.MomoOrderInfos.Add(momo);
                _context.SaveChanges();

                TempData["success"] = $"Nạp tiền thành công. Bạn đã nhận được điểm.";
                return RedirectToAction("PointHistory", "Checkout");
            }
            else
            {
                TempData["error"] = "Giao dịch MoMo đã bị huỷ hoặc thất bại.";
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult PointHistory()
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            // 1. Nạp điểm từ MoMo
            var momoTransactions = _context.MomoOrderInfos
                .Where(o => o.UserID == userIdClaim)
                .Select(o => new PointTransactionViewModel
                {
                    Date = o.DatePaid,
                    Points = (int)(o.Amount / 10000),
                    Description = $"Nạp điểm qua MoMo (Mã đơn: {o.OrderId})",
                    ActionType = "Recharge"
                })
                .ToList();

            // 2. Giao dịch từ hệ thống (Download, Earn, Approve)
            var internalTransactions = _context.Transactions
                .Where(t => t.UserID == userId)
                .Select(t => new PointTransactionViewModel
                {
                    Date = t.CreatedAt,
                    Points = t.PointsChanged,
                    Description = t.Description ?? t.Action,
                    ActionType = t.Action
                })
                .ToList();

            // 3. Gộp lại và sắp xếp
            var allTransactions = momoTransactions
                .Concat(internalTransactions)
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(allTransactions);
        }


    }
}
