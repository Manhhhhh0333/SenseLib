using DOAN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Components
{
	[ViewComponent(Name = "Sliders")]
	public class SlidersComponent : ViewComponent
	{
		private readonly DataContext _context;
		public SlidersComponent(DataContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var listOfMenu = (from m in _context.Sliders
							  where (m.Status == true) 
							  select m).ToList();
			return await Task.FromResult((IViewComponentResult)View("Default", listOfMenu));
		}
	}
}
