using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Quartz;

namespace UseCase.Jobs
{
    public class JobManagerService : IJobManagerService
    {
        private readonly ISettingRepository settingRepository;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly ILogger<JobManagerService> logger;
        private Dictionary<string,Type> Jobs;

        public JobManagerService(ISettingRepository settingRepository, 
            ISchedulerFactory schedulerFactory, 
            ILogger<JobManagerService> logger)
        {
            this.settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            this.schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Jobs = new Dictionary<string, Type> {
                {nameof(ReadAllActiveAdsJob), typeof(ReadAllActiveAdsJob) },
                {nameof(RePostAdByTitleJob), typeof(RePostAdByTitleJob) },
                {nameof(WebDriverStatusJob), typeof(WebDriverStatusJob) },
            };
        }


        public async Task ReScheduleJobsWithLatestSetting()
        {
            var newSetting = await settingRepository.Read();
            var schedulers = await schedulerFactory.GetAllSchedulers();
            foreach (var scheduler in schedulers)
            {
                await scheduler.Shutdown(true);
            }
            logger.LogInformation($"All jobs shutdown");

            var random = new Random();
            foreach (var job in Jobs)
            {
                var scheduler = await schedulerFactory.GetScheduler();
                var jobKey = new JobKey(job.Key);
                var jobDetails = JobBuilder.Create(job.Value)
                    .WithIdentity(jobKey)
                    .Build();
                var interval = GetTimeSpanByJobKey(jobKey, newSetting);
                if (interval is null)
                {
                    logger.LogInformation($"Job {jobKey.Name} has no interval setup, so ignored");
                    continue;
                }
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(job.Key)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(interval.Value)
                        .RepeatForever())
                    .Build();
                await scheduler.ScheduleJob(jobDetails, trigger);
                var randomValue = random.Next(1, 10);
                await scheduler.StartDelayed(TimeSpan.FromMinutes(randomValue));
                logger.LogInformation($"Job {jobKey.Name} is updated with inteval: {interval}, will be started after {randomValue} minutes");
            }
        }

        private TimeSpan? GetTimeSpanByJobKey(JobKey jobKey, Setting? newSetting)
        {
            if (jobKey.Name == nameof(ReadAllActiveAdsJob))
            {
                return TimeSpan.FromMinutes(newSetting?.ReadInterval ?? 5);
            }

            if (jobKey.Name == nameof(RePostAdByTitleJob))
            {
                return TimeSpan.FromMinutes(newSetting?.RePostInterval ?? 5);
            }

            if (jobKey.Name == nameof(WebDriverStatusJob))
            {
                return TimeSpan.FromMinutes(1);
            }
            return null;
        }

        
    }
}
