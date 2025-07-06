using DOAN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DOAN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DocumentController : Controller
    {
        private readonly DataContext _context;
        public DocumentController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var docList = _context.Documents.OrderBy(d => d.DocumentID).ToList();
            return View(docList);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var doc = _context.Documents.Find(id);
            if (doc == null)
            {
                return NotFound();
            }
            return View(doc);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleDoc = _context.Documents.Find(id);
            if (deleDoc == null)
            {
                return NotFound();
            }
            _context.Documents.Remove(deleDoc);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        // Create
        public IActionResult Create()
        {
            var mnCat = (from m in _context.Categories
                          select new SelectListItem()
                          {
                              Text = m.Name,
                              Value = m.CategoryID.ToString(),
                          }).ToList();

            mnCat.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.mnCat = mnCat;

            var pubList = (from p in _context.Publishers
                           select new SelectListItem()
                           {
                               Text = p.Name,
                               Value = p.PublisherID.ToString(),
                           }).ToList();
            pubList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.pubList = pubList;
            // Lấy danh sách tác giả từ cơ sở dữ liệu
            var authList = (from a in _context.Authors
                            select new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.AuthorID.ToString(),
                            }).ToList();
            authList.Insert(0, new SelectListItem()
            {
                Text = "----Select----",
                Value = "0"
            });
            ViewBag.authList = authList;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tblDocuments mn)
        {
            if (ModelState.IsValid)
            {
                _context.Documents.Add(mn);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(mn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblDocuments mn)
        {
            if (ModelState.IsValid)
            {
                _context.Documents.Update(mn);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Gán lại ViewBag khi có lỗi trả về View
            ViewBag.mnList = _context.Categories
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.CategoryID.ToString()
                })
                .ToList();

            ViewBag.pubList = _context.Publishers
                .Select(p => new SelectListItem
                {
                    Text = p.Name,
                    Value = p.PublisherID.ToString()
                })
                .ToList();

            ViewBag.authList = _context.Authors
                .Select(a => new SelectListItem
                {
                    Text = a.Name,
                    Value = a.AuthorID.ToString()
                })
                .ToList();

            return View(mn);
        }

    }
}
