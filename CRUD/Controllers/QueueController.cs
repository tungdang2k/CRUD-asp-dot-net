using CRUD.Models;
using CRUD.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly RedisQueueService _queue;

        public QueueController(RedisQueueService queue)
        {
            _queue = queue;
        }

        // Xem số lượng Job
        [HttpGet("length")]
        public async Task<IActionResult> Length()
        {
            var count = await _queue.GetQueueLengthAsync();

            return Ok(new
            {
                QueueLength = count
            });
        }

        // Xóa Queue
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            await _queue.ClearQueueAsync();

            return Ok("Queue đã được xóa.");
        }

        // Thêm Job để test
        [HttpPost("test")]
        public async Task<IActionResult> AddTestJob()
        {
            await _queue.AddJobAsync(new QueueJob
            {
                FileName = "SinhVien_Import_Mau.xlsx",
                FilePath = @"D:\download\SinhVien_Import_Mau.xlsx"
            });

            return Ok("Đã thêm Job.");
        }
    }
}