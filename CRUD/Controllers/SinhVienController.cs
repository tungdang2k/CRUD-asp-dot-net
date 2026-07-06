using CRUD.Data;
using CRUD.Models;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ds = _context.SinhVien.ToList();

            return View(ds);
        }

        // Hiển thị 
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Create(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                _context.SinhVien.Add(sv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(sv);
        }

        //  sửa
        public IActionResult Edit(int id)
        {
            var sv = _context.SinhVien.Find(id);

            if (sv == null)
                return NotFound();

            return View(sv);
        }

        
        [HttpPost]
        public IActionResult Edit(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                _context.SinhVien.Update(sv);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(sv);
        }


        //  xóa
        public IActionResult Delete(int id)
        {
            var sv = _context.SinhVien.Find(id);

            if (sv == null)
                return NotFound();

            return View(sv);
        }

  
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var sv = _context.SinhVien.Find(id);

            if (sv != null)
            {
                _context.SinhVien.Remove(sv);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
