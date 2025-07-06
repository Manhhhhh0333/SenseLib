using DOAN.Models;
using Microsoft.AspNetCore.Mvc;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublisherController : Controller
    {
        private readonly DataContext _context;
        public PublisherController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var c = _context.Publishers.OrderBy(g => g.PublisherID).ToList();
            return View(c);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tblPublishers ab)
        {
            if (ModelState.IsValid)
            {
                _context.Publishers.Add(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ab = _context.Publishers.Find(id);
            if (ab == null)
            {
                return NotFound();
            }
            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var ab = _context.Publishers.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(ab);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ab = _context.Publishers.Find(id);
            if (ab == null)
            {
                return NotFound();
            }

            return View(ab);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblPublishers ab)
        {
            if (ModelState.IsValid)
            {
                _context.Publishers.Update(ab);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(ab);
        }
    }
}
