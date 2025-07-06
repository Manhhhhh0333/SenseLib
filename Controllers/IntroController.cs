using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    public class IntroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
