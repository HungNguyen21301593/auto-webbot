using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UseCase.Jobs;

namespace UseCase.Service
{
    public class SettingService : ISettingService
    {
        private readonly IJobManagerService jobManagerService;
        private readonly IPostRepository postRepository;
        private readonly ILogger<SettingService> logger;

        public ISettingRepository SettingRepository { get; }

        public SettingService(ISettingRepository settingRepository, IJobManagerService jobManagerService,
            IPostRepository postRepository, ILogger<SettingService> logger)
        {
            SettingRepository = settingRepository;
            this.jobManagerService = jobManagerService ?? throw new ArgumentNullException(nameof(jobManagerService));
            this.postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Setting> GetSettingAsync()
        {
            return await SettingRepository.Read();
        }

        public async Task UpdateSettingAsync(Setting setting)
        {
            await SettingRepository.Update(setting);
            await RefreshAllFailedOrOnOnGoingPost();
            await jobManagerService.ReScheduleJobsWithLatestSetting();
            logger.LogInformation($"Seting is updated, {JsonConvert.SerializeObject(setting)}");
        }

        private async Task RefreshAllFailedOrOnOnGoingPost()
        {
            var allFailedOrOnGoingPosts = await postRepository.GetAllFailedOrOnGoingPost();
            foreach (var allFailedOrOnGoingPost in allFailedOrOnGoingPosts)
            {
                allFailedOrOnGoingPost.Status = AdStatus.New;
                allFailedOrOnGoingPost.stepLogs = new List<StepLog>();
            }
            await postRepository.UpdateRange(allFailedOrOnGoingPosts);
        }
    }
}
