namespace CRUD.Services
{
    public class EmailService
    {
        public void SendWelcomeEmail(string email)
        {
            Console.WriteLine($"[{DateTime.Now}] Đang gửi email tới: {email}");

            // Giả lập gửi email mất 5 giây
            Thread.Sleep(5000);

            Console.WriteLine($"[{DateTime.Now}] Đã gửi email thành công tới: {email}");
        }
    }
}
