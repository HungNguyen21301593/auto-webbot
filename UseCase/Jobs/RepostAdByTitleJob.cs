using Core.Model;
using Entities;
using FireBaseAuthenticator.KijijiHelperServices;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using UseCase.Service;
using UseCase.Service.Tabs;
using Util;

namespace UseCase.Jobs
{
    public class RePostAdByTitleJob : IJob
    {
        public ISettingRepository SettingRepository { get; }
        public IKijijiPostingService KijijiPostingService { get; }
        public IPostRepository PostRepository { get; }
        public ILogger<RePostAdByTitleJob> Logger { get; }
        public static readonly JobKey Key = new JobKey(nameof(RePostAdByTitleJob));
        public DataContext DataContext { get; }

        public RePostAdByTitleJob(ISettingRepository settingRepository,
            IKijijiPostingService kijijiPostingService,
            IPostRepository postRepository,
            ILogger<RePostAdByTitleJob> logger,
            DataContext dataContext)
        {
            SettingRepository = settingRepository;
            KijijiPostingService = kijijiPostingService;
            PostRepository = postRepository;
            Logger = logger;
            DataContext = dataContext;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = Guid.NewGuid();
            Logger.LogInformation($"Job started {GetType().Name} | Id: {jobId}");
           
            
            KijijiPostingService.Execute(KijijiExecuteType.PostAdByTitle);
            Logger.LogInformation($"Job executed {GetType().Name} | Id: {jobId}");
        }
    }
}
