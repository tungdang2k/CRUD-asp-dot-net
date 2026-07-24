using CRUD.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobControler : ControllerBase
    {
        private readonly RedisQueueService _queue;

        public JobControler(RedisQueueService queue)
        {
            _queue = queue;
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

        [HttpGet]
        [Route("QueueLength")]
        public async Task<IActionResult> QueueLength()
        {
            var count = await _queue.GetQueueLengthAsync();

            return Ok(new
            {
                Queue = count
            });
        }

    }
}
