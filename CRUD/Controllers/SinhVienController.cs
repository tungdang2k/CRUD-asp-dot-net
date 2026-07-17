using CRUD.Data;
using CRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
namespace CRUD.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment _environment;
        public SinhVienController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            int totalItems = await _context.SinhVien.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var sinhViens = await _context.SinhVien
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(sinhViens);
        }
        // Hiển thị 
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(SinhVien sv)
        {
            if (sv.ImageFile != null)
            {
                string fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(sv.ImageFile.FileName);

                string folder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "sinhvien");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await sv.ImageFile.CopyToAsync(stream);
                }

                sv.Anh = "sinhvien/" + fileName;
            }

            _context.SinhVien.Add(sv);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(SinhVien sv)
        {
            var sinhVien = await _context.SinhVien.FindAsync(sv.Id);

            if (sinhVien == null)
                return NotFound();

            sinhVien.HoTen = sv.HoTen;
            sinhVien.Tuoi = sv.Tuoi;
            sinhVien.Lop = sv.Lop;

            if (sv.ImageFile != null)
            {
                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(sv.ImageFile.FileName);

                string folder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "sinhvien");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await sv.ImageFile.CopyToAsync(stream);
                }

                sinhVien.Anh = "sinhvien/" + fileName;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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

        // search
        public async Task<IActionResult> Search(string searchString)
        {
            var sinhViens = _context.SinhVien.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                sinhViens = sinhViens.Where(x => x.HoTen.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;

            return View(await sinhViens.ToListAsync());
        }


        // import excel
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "Vui lòng chọn file Excel.";
                return RedirectToAction("Index");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var sv = new SinhVien
                {
                    HoTen = row.Cell(1).GetString(),
                    Tuoi = row.Cell(2).GetValue<int>(),
                    Lop = row.Cell(3).GetString()
                };

                _context.SinhVien.Add(sv);
            }

            _context.SaveChanges();

            TempData["Message"] = "Import thành công!";
            return RedirectToAction("Index");
        }


        // export excel
        public async Task<IActionResult> ExportExcel()
        {
            var danhSach = await _context.SinhVien.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("SinhVien");

                // Tiêu đề cột
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Họ tên";
                worksheet.Cell(1, 3).Value = "Tuổi";
                worksheet.Cell(1, 4).Value = "Lớp";
                worksheet.Cell(1, 5).Value = "Ảnh";

                int row = 2;

                foreach (var sv in danhSach)
                {
                    worksheet.Cell(row, 1).Value = sv.Id;
                    worksheet.Cell(row, 2).Value = sv.HoTen;
                    worksheet.Cell(row, 3).Value = sv.Tuoi;
                    worksheet.Cell(row, 4).Value = sv.Lop;
                    worksheet.Cell(row, 5).Value = sv.Anh;

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DanhSachSinhVien.xlsx");
                }
            }
        }
    }
}
