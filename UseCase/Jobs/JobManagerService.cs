using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;
using static Quartz.Logging.OperationName;

namespace UseCase.Jobs
{
    public class JobManagerService : IJobManagerService
    {
        private readonly ISettingRepository settingRepository;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly ILogger<JobManagerService> logger;
        private Dictionary<JobKey,Type> Jobs;
        private readonly string ScheduleName = "auto-webbot";

        public JobManagerService(ISettingRepository settingRepository, 
            ISchedulerFactory schedulerFactory, 
            ILogger<JobManagerService> logger)
        {
            this.settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            this.schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Jobs = new Dictionary<JobKey, Type> {
                {ReadAllActiveAdsJob.Key, typeof(ReadAllActiveAdsJob) },
                {RePostAdByTitleJob.Key, typeof(RePostAdByTitleJob) },
                {WebDriverStatusJob.Key, typeof(WebDriverStatusJob) },
            };
        }


        public async Task ReScheduleJobsWithLatestSetting()
        {
            var setting = await settingRepository.Read();
            if (setting == null)
            {
                logger.LogInformation($"There is no setting available on db so no job will be scheduled");
                return;
            }
            var guid = Guid.NewGuid();
            var newScheduler = await schedulerFactory.GetScheduler();

            foreach (var job in Jobs)
            {
                var jobDetails = JobBuilder.Create(job.Value)
                    .WithIdentity(job.Key)
                    .Build();
                var interval = GetTimeSpanByJobKey(job.Key, setting);
                if (interval is null)
                {
                    logger.LogInformation($"Job {job.Key.Name} has no interval setup, so ignored");
                    continue;
                }

                var triggerKey = new TriggerKey(job.Key.Name);
                var oldTrigger = await newScheduler.GetTrigger(triggerKey);
                if (oldTrigger != null)
                {
                    var newTrigger = oldTrigger.GetTriggerBuilder()
                        .WithSimpleSchedule(x => x
                            .WithInterval(interval.Value)
                            .RepeatForever())
                        .Build();

                    await newScheduler.RescheduleJob(triggerKey, newTrigger);
                }
                else
                {
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .WithSimpleSchedule(x => x
                            .WithInterval(interval.Value)
                            .RepeatForever())
                        .Build();

                    await newScheduler.ScheduleJob(jobDetails, trigger);
                }

                logger.LogInformation($"Job {job.Key.Name} is updated with interval: {interval}, will start now");
            }
            await newScheduler.Start();
        }

        private TimeSpan? GetTimeSpanByJobKey(JobKey jobKey, Setting? newSetting)
        {
            if (jobKey.Name.Contains(nameof(ReadAllActiveAdsJob)))
            {
                return TimeSpan.FromMinutes(newSetting?.ReadInterval ?? 5);
            }

            if (jobKey.Name.Contains(nameof(RePostAdByTitleJob)))
            {
                return TimeSpan.FromMinutes(newSetting?.RePostInterval ?? 5);
            }

            if (jobKey.Name.Contains(nameof(WebDriverStatusJob)))
            {
                return TimeSpan.FromMinutes(1);
            }
            return null;
        }
    }
}
