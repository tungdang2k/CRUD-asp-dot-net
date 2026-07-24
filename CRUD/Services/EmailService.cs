namespace CRUD.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendWarningEmailAsync(
            string subject,
            string message)
        {
            await Task.Delay(1000);

            _logger.LogWarning("========== EMAIL ==========");
            _logger.LogWarning(subject);
            _logger.LogWarning(message);
            _logger.LogWarning("===========================");

            Console.WriteLine("========== EMAIL ==========");
            Console.WriteLine(subject);
            Console.WriteLine(message);
            Console.WriteLine("===========================");
        }
    }
}