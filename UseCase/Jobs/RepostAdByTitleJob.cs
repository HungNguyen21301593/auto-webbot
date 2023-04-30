using Core.Model;
using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using UseCase.Service;
using UseCase.Service.Tabs;

namespace UseCase.Jobs
{
    public class RePostAdByTitleJob : IJob
    {
        public ISettingRepository SettingRepository { get; }
        public IKijijiPostingService KijijiPostingService { get; }
        public IPostRepository PostRepository { get; }
        public ILogger<RePostAdByTitleJob> Logger { get; }

        public RePostAdByTitleJob(ISettingRepository settingRepository,
            IKijijiPostingService kijijiPostingService,
            IPostRepository postRepository,
            ILogger<RePostAdByTitleJob> logger)
        {
            SettingRepository = settingRepository;
            KijijiPostingService = kijijiPostingService;
            PostRepository = postRepository;
            Logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = Guid.NewGuid();
            //Logger.LogInformation($"Job {GetType().Name} started | Id: {jobId}");
            var setting = await SettingRepository.Read();
            if (setting is null) { return; }
            var newTrigger =
                ConstructTrigger(context.Trigger.GetTriggerBuilder(), setting.RePostInterval);
            await context.Scheduler.RescheduleJob(newTrigger.Key, newTrigger);
            //Logger.LogInformation($"Job rescheduled {GetType().Name} with {setting.RePostInterval}");
            var post = await PostRepository.GetNextAdToPost();
            if (post is null)
            {
                Logger.LogInformation($"There is no active post available, no work will be proceeded");
                Logger.LogInformation($"Job executed {GetType().Name} | Id: {jobId}");
                return;
            }
            var adDetails = JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson);
            Logger.LogInformation($"Found ad with title {adDetails.AdTitle}, proceed repost");
            post.Status = AdStatus.Started;
            await PostRepository.Update(post);
            await Task.Delay(TimeSpan.FromMinutes(setting.AdActiveInterval));
            KijijiPostingService.Execute(KijijiExecuteType.PostAdByTitle, new ExecuteParams
            {
                Post = post,
                Setting = setting
            });
            Logger.LogInformation($"Job executed {GetType().Name} | Id: {jobId}");
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
