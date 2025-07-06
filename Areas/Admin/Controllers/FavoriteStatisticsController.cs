using DOAN.Models;
using DOAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[Area("Admin")]
public class FavoriteStatisticsController : Controller
{
    private readonly DataContext _context;

    public FavoriteStatisticsController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var favoritesGrouped = _context.Favorites
            .Include(f => f.Document)
            .Include(f => f.User)
            .GroupBy(f => new { f.DocumentID, f.Document.Title })
            .Select(g => new FavoriteStatisticViewModel.DocumentFavorite
            {
                DocumentTitle = g.Key.Title,
                FavoriteCount = g.Count(),
                UserEmails = g.Select(f => f.User.Email).ToList()
            })
            .OrderByDescending(g => g.FavoriteCount)
            .ToList();

        var viewModel = new FavoriteStatisticViewModel
        {
            TotalFavorites = _context.Favorites.Count(),
            TopFavoritedDocuments = favoritesGrouped
        };

        return View(viewModel);
    }
}
