using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DOAN.Components
{
    [ViewComponent(Name = "MenuView")]
    public class MenuViewComponent : ViewComponent
    {
        private readonly DataContext _context;
        public MenuViewComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfMenu = (from m in _context.Menus
                            where (m.IsActive == true) && (m.Position == 1)
                            select m).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfMenu));
        }
        
    }
}