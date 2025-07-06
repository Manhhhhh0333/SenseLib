using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DOAN.Models;
using DOAN.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;
    public HomeController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var categories = _context.Categories.ToList();
        var documents =  _context.Documents.ToList();

        var viewModel = new DocumentCategoryViewModel
        {
            Categories = categories,
            Documents = documents
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
