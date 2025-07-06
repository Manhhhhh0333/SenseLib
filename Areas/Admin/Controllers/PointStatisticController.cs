using DOAN.Models;
using Microsoft.AspNetCore.Mvc;

[Area("Admin")]
public class PointStatisticController : Controller
{
    private readonly DataContext _context;

    public PointStatisticController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var totalPointsInSystem = _context.Users.Sum(u => u.UserPoint ?? 0);
        var totalPointsEarned = _context.Transactions.Where(t => t.PointsChanged > 0).Sum(t => t.PointsChanged);
        var totalPointsSpent = _context.Transactions.Where(t => t.PointsChanged < 0).Sum(t => -t.PointsChanged);

        var userStats = _context.Users
            .Select(u => new UserPointStatViewModel
            {
                UserID = u.UserID,
                FullName = u.FullName,
                Email = u.Email,
                CurrentPoints = u.UserPoint ?? 0,
                TotalEarned = _context.Transactions.Where(t => t.UserID == u.UserID && t.PointsChanged > 0).Sum(t => t.PointsChanged),
                TotalSpent = _context.Transactions.Where(t => t.UserID == u.UserID && t.PointsChanged < 0).Sum(t => -t.PointsChanged)
            })
            .ToList();

        var docStats = _context.Documents
            .Select(d => new DocumentPointStatViewModel
            {
                DocumentID = d.DocumentID,
                Title = d.Title,
                UploadedBy = d.UploadedBy,
                TotalDownloads = _context.Downloads.Count(dl => dl.DocumentID == d.DocumentID),
                TotalPointsEarned = _context.Transactions
                    .Where(t => t.Description.Contains(d.Title) && t.Action == "Earn")
                    .Sum(t => t.PointsChanged)
            })
            .ToList();

        var model = new PointStatisticDashboardViewModel
        {
            TotalPointsInSystem = totalPointsInSystem,
            TotalPointsEarned = totalPointsEarned,
            TotalPointsSpent = totalPointsSpent,
            UserStats = userStats,
            DocumentStats = docStats
        };

        return View(model);
    }
}
