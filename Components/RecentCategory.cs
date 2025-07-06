using DOAN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Components
{
    [ViewComponent(Name = "RecentCategory")]
    public class RecentCategory : ViewComponent
    {
        private readonly DataContext _context;
        public RecentCategory(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfMenu = (from m in _context.Categories
                              where (m.Status == true)
                              select m).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfMenu));
        }
    }
}
