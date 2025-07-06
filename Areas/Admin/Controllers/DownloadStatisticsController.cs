// Areas/Admin/Controllers/DownloadStatisticsController.cs
using DOAN.Models;
using DOAN.ViewModel;
using DOAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DownloadStatisticsController : Controller
    {
        private readonly DataContext _context;

        public DownloadStatisticsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var totalDownloads = _context.Downloads.Count();

            var downloadsByDoc = _context.Downloads 
                .Include(d => d.Document)
                .GroupBy(d => d.Document.Title)
                .Select(g => new DownloadStatisticViewModel.DownloadByDocument
                {
                    DocumentTitle = g.Key,
                    DownloadCount = g.Count()
                }).ToList();

            var viewModel = new DownloadStatisticViewModel
            {
                TotalDownloads = totalDownloads,
                DownloadsByDocument = downloadsByDoc
            };

            return View(viewModel);
        }
    }
}
