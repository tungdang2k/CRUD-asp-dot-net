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

        public void ImportFakeStudents()
        {
            Console.WriteLine("===== BẮT ĐẦU IMPORT =====");

            for (int i = 1; i <= 10000; i++)
            {
                _context.SinhVien.Add(new SinhVien
                {
                    HoTen = $"Sinh viên {i}",
                    Tuoi = 20,
                    Lop = "CNTT"
                });

                // Lưu mỗi 1000 bản ghi
                if (i % 1000 == 0)
                {
                    _context.SaveChanges();

                    Console.WriteLine($"Đã import {i} sinh viên");

                    Thread.Sleep(2000); // Giả lập xử lý lâu
                }
            }

            _context.SaveChanges();

            Console.WriteLine("===== IMPORT XONG =====");
        }
    }
}
