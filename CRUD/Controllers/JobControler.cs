using CRUD.Services;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobControler : ControllerBase
    {
        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            BackgroundJob.Enqueue<EmailService>( (x) => x.SendWelcomeEmail("tungdev@gmail.com"));

            return Ok("Đã đưa email vào hàng đợi.");
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            var scheduledDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduledDateTime);
            BackgroundJob.Schedule(() => Console.WriteLine("Scheduled job trigger"), TimeSpan.FromMinutes(1));

            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var scheduledDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduledDateTime);
            var jobid = BackgroundJob.Schedule(() => Console.WriteLine("Scheduled job 2 trigger"), dateTimeOffset);

            var job2ID = BackgroundJob.ContinueJobWith(jobid, () => Console.WriteLine("continuation job 2 triggerd"));
            var job3ID = BackgroundJob.ContinueJobWith(job2ID, () => Console.WriteLine("continuation job 3 triggerd"));
            var job4ID = BackgroundJob.ContinueJobWith(job3ID, () => Console.WriteLine("continuation job 4 triggerd"));

            return Ok();
        }

        //public static void TestJob()
        //{
        //    Console.WriteLine($"Job chạy lúc: {DateTime.Now}");
        //}

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {

            //RecurringJob.AddOrUpdate(
            //    "TestRecurringJob",
            //    () => TestJob(),
            //    Cron.MinuteInterval(5));

            return Ok();
        }


    }
}
