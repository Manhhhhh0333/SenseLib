using Microsoft.AspNetCore.Mvc;

namespace DOAN.Controllers
{
    [Route("/file-manager")]
    public class FileManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
