using System.ComponentModel.DataAnnotations.Schema;

namespace CRUD.Models
{
    public class SinhVien
    {
        public int Id { get; set; }

        public string HoTen { get; set; } = string.Empty;

        public int Tuoi { get; set; }

        public string Lop { get; set; } = string.Empty;
        public string? Anh { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

    }
}
