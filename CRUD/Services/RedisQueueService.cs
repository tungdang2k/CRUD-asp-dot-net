using CRUD.Models;
using StackExchange.Redis;
using System.Text.Json;
namespace CRUD.Services
{
    public class RedisQueueService
    {
        private readonly IDatabase _database;

        private readonly IConfiguration _configuration;

        private readonly string _queueName;

        public RedisQueueService(
            IConnectionMultiplexer redis,
            IConfiguration configuration)
        {
            _database = redis.GetDatabase();

            _configuration = configuration;

            _queueName = _configuration["RedisQueue:QueueName"]!;
        }

        //-----------------------------------
        // Thêm Job vào Queue
        //-----------------------------------

        public async Task AddJobAsync(QueueJob job)
        {
            var json = JsonSerializer.Serialize(job);

            await _database.ListRightPushAsync(
                _queueName,
                json);
        }

        //-----------------------------------
        // Lấy Job khỏi Queue
        //-----------------------------------

        public async Task<QueueJob?> GetJobAsync()
        {
            var value = await _database.ListLeftPopAsync(_queueName);

            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<QueueJob>(value!);
        }

        //-----------------------------------
        // Đếm Queue
        //-----------------------------------

        public async Task<long> GetQueueLengthAsync()
        {
            return await _database.ListLengthAsync(_queueName);
        }

        //-----------------------------------
        // Xóa Queue
        //-----------------------------------

        public async Task ClearQueueAsync()
        {
            await _database.KeyDeleteAsync(_queueName);
        }
    }
}
