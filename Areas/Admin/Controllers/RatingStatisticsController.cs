using DOAN.Models;
using DOAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
public class RatingStatisticsController : Controller
{
    private readonly DataContext _context;

    public RatingStatisticsController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var viewModel = new RatingStatisticViewModel
        {
            TotalRatings = _context.Ratings.Count(),

            RatingsByDocument = _context.Ratings
                .GroupBy(r => r.DocumentID)
                .Select(g => new RatingStatisticViewModel.RatingByDocument
                {
                    DocumentTitle = g.Key.ToString(),
                    AverageRating = Math.Round(g.Average(r => r.RatingValue), 2),
                    RatingCount = g.Count()
                })
                .ToList(),

            RecentRatings = _context.Ratings
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .Select(r => new RatingStatisticViewModel.RecentRating
                {
                    RatingID = r.RatingID,
                    UserName = r.UserID.ToString(),
                    DocumentTitle = r.DocumentID.ToString(),
                    RatingValue = r.RatingValue,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue
                })
                .ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var rating = _context.Ratings.Find(id);
        if (rating != null)
        {
            _context.Ratings.Remove(rating);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
