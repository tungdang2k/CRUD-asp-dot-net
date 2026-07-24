namespace CRUD.Services
{
    public class QueueWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RedisQueueService _queue;

        public QueueWorkerService(
            IServiceScopeFactory scopeFactory,
            RedisQueueService queue)
        {
            _scopeFactory = scopeFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("===== Queue Worker Started =====");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var job = await _queue.GetJobAsync();

                    if (job == null)
                    {
                        await Task.Delay(3000, stoppingToken);
                        continue;
                    }

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"Nhận Job : {job.JobId}");
                    Console.WriteLine($"File     : {job.FileName}");
                    Console.WriteLine("--------------------------------");

                    using var scope = _scopeFactory.CreateScope();

                    var studentService =
                        scope.ServiceProvider.GetRequiredService<StudentJobService>();

                    await studentService.ImportExcelAsync(job.FilePath);

                    Console.WriteLine("Job hoàn thành.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi Worker");
                    Console.WriteLine(ex.Message);

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
