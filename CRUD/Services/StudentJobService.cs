using ClosedXML.Excel;
using CRUD.Data;
using CRUD.Models;

namespace CRUD.Services
{
    public class StudentJobService
    {
        private readonly ApplicationDbContext _context;

        public StudentJobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportExcelAsync(string filePath)
        {
            Console.WriteLine($"===== BẮT ĐẦU IMPORT {filePath} =====");

            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().Skip(1);

            int count = 0;

            foreach (var row in rows)
            {
                var sv = new SinhVien
                {
                    HoTen = row.Cell(1).GetString(),
                    Tuoi = row.Cell(2).GetValue<int>(),
                    Lop = row.Cell(3).GetString()
                };

                _context.SinhVien.Add(sv);

                count++;

                // Cứ 100 bản ghi lưu một lần
                if (count % 100 == 0)
                {
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Đã import {count} sinh viên");

                    // Giả lập import chậm
                    await Task.Delay(1000);
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine("===== IMPORT THÀNH CÔNG =====");
        }
    }
}