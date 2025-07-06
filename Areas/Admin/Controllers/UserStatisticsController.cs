using ClosedXML.Excel;
using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserStatisticsController : Controller
    {
        private readonly DataContext _context;

        public UserStatisticsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _context.Users.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.Status == true);
            var inactiveUsers = await _context.Users.CountAsync(u => u.Status == false);

            var usersByRole = await _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToListAsync();

            var usersByMonth = await _context.Users
                .Where(u => u.CreatedAt.HasValue)
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                }).ToListAsync();

            var loggedInThisMonth = await _context.Users.CountAsync(u =>
                u.LastLogin.HasValue &&
                u.LastLogin.Value.Month == DateTime.Now.Month &&
                u.LastLogin.Value.Year == DateTime.Now.Year
            );

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.InactiveUsers = inactiveUsers;
            ViewBag.UsersByRole = usersByRole;
            ViewBag.UsersByMonth = usersByMonth;
            ViewBag.LoggedInThisMonth = loggedInThisMonth;

            var allUsers = await _context.Users.ToListAsync();
            return View(allUsers);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var users = await _context.Users.ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");

            // Header
            worksheet.Cell(1, 1).Value = "Email";
            worksheet.Cell(1, 2).Value = "Trạng thái";
            worksheet.Cell(1, 3).Value = "Vai trò";
            worksheet.Cell(1, 4).Value = "Ngày tạo";
            worksheet.Cell(1, 5).Value = "Lần đăng nhập gần nhất";

            int row = 2;
            foreach (var user in users)
            {
                worksheet.Cell(row, 1).Value = user.Email;
                worksheet.Cell(row, 2).Value = user.Status == true ? "Hoạt động" : "Chưa kích hoạt";
                worksheet.Cell(row, 3).Value = user.Role ?? "Không xác định";
                worksheet.Cell(row, 4).Value = user.CreatedAt?.ToString("dd/MM/yyyy") ?? "Không có";
                worksheet.Cell(row, 5).Value = user.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa đăng nhập";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ThongKeTaiKhoan.xlsx");
        }
    }
}
