using Core.Model;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;
using UseCase.Service;
using UseCase.Service.Tabs;

namespace UseCase.Jobs
{
    public class ReadAllActiveAdsJob : IJob
    {
        public ISettingRepository SettingRepository { get; }
        public IKijijiPostingService KijijiPostingService { get; }
        public ILogger<ReadAllActiveAdsJob> Logger { get; }

        public ReadAllActiveAdsJob(ISettingRepository settingRepository, 
            IKijijiPostingService kijijiPostingService,
            ILogger<ReadAllActiveAdsJob> logger)
        {
            SettingRepository = settingRepository;
            KijijiPostingService = kijijiPostingService;
            Logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = Guid.NewGuid();
            //Logger.LogInformation($"Job {GetType().Name} started | Id: {jobId}");
            var setting = await SettingRepository.Read();
            var newTrigger =
                ConstructTrigger(context.Trigger.GetTriggerBuilder(), setting.ReadInterval);
            await context.Scheduler.RescheduleJob(newTrigger.Key, newTrigger);
            //Logger.LogInformation($"Job {GetType().Name} rescheduled with {setting.ReadInterval}");

            KijijiPostingService.Execute(KijijiExecuteType.ReadAds, new ExecuteParams{Page = setting.PageToTrigger});
            //Logger.LogInformation($"Job executed {GetType().Name} | Id: {jobId}");
        }

        private static ITrigger ConstructTrigger(TriggerBuilder triggerBuilder, long interval)
        {
            return triggerBuilder.WithSimpleSchedule(s => s
                    .RepeatForever()
                    .WithInterval(TimeSpan.FromMinutes(interval)))
                .Build();
        }
    }
}
