using StackExchange.Redis;

namespace CRUD.Services
{
    public class QueueMonitorService
    {
        private readonly RedisQueueService _queue;
        private readonly EmailService _email;
        private readonly IConfiguration _configuration;

        private DateTime _lastWarning = DateTime.MinValue;

        public QueueMonitorService(
            RedisQueueService queue,
            EmailService email,
            IConfiguration configuration)
        {
            _queue = queue;
            _email = email;
            _configuration = configuration;
        }

        public async Task CheckQueueAsync()
        {
            long count = await _queue.GetQueueLengthAsync();

            Console.WriteLine($"Queue Length : {count}");

            int warning =
                _configuration.GetValue<int>("RedisQueue:Warning");

            int critical =
                _configuration.GetValue<int>("RedisQueue:Critical");

            int emergency =
                _configuration.GetValue<int>("RedisQueue:Emergency");

            if (count >= emergency)
            {
                await SendEmail(
                    "EMERGENCY",
                    count);

                return;
            }

            if (count >= critical)
            {
                await SendEmail(
                    "CRITICAL",
                    count);

                return;
            }

            if (count >= warning)
            {
                await SendEmail(
                    "WARNING",
                    count);
            }
        }

        private async Task SendEmail(
            string level,
            long count)
        {
            if (DateTime.Now.Subtract(_lastWarning).TotalMinutes < 1)
                return;

            _lastWarning = DateTime.Now;

            await _email.SendWarningEmailAsync(
                $"[{level}] Redis Queue",
                $"Queue hiện có {count} Job.");
        }
    }
}