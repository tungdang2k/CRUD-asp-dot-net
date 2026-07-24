namespace CRUD.Models
{
    public class QueueJob
    {
        public Guid JobId { get; set; } = Guid.NewGuid();

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public string FileName { get; set; } = "";

        public string FilePath { get; set; } = "";

        public string CreatedBy { get; set; } = "Admin";

    }
}
