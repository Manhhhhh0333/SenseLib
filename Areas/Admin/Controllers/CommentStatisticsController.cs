using DOAN.Models;
using DOAN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CommentStatisticsController : Controller
    {
        private readonly DataContext _context;

        public CommentStatisticsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalComments = await _context.Comments.CountAsync();
            var commentsByUser = await _context.Comments
                .GroupBy(c => c.User.Email)
                .Select(g => new CommentStatisticViewModel.CommentByUser
                {
                    Email = g.Key,
                    Count = g.Count()
                }).ToListAsync();

            var recentComments = await _context.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Take(20)
                .Select(c => new CommentStatisticViewModel.CommentDetail
                {
                    Id = c.CommentID,
                    Content = c.Content,
                    UserEmail = c.User.Email,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            var viewModel = new CommentStatisticViewModel
            {
                TotalComments = totalComments,
                CommentsByUser = commentsByUser,
                RecentComments = recentComments
            };

            return View(viewModel);
        }

        public IActionResult Delete(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.CommentID == id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
