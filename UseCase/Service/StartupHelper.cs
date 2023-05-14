using Core.Model;
using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UseCase.Service
{
    public class StartupHelper : IStartupHelper
    {
        private readonly IConfiguration configuration;
        private readonly IBrowserManagerService browserManagerService;
        private readonly ILogger<StartupHelper> logger;
        private readonly ISettingRepository settingRepository;
        private readonly IPostRepository postRepository;
        private readonly DataContext dbContext;
        private readonly IDeviceInfoChart deviceInfoChart;
        private readonly IKijijiPostingService kijijiPostingService;

        public StartupHelper(IConfiguration configuration,
            IBrowserManagerService browserManagerService,
            ILogger<StartupHelper> logger,
            ISettingRepository settingRepository,
            IPostRepository postRepository,
            DataContext dbContext,
            IDeviceInfoChart deviceInfoChart,
            IKijijiPostingService kijijiPostingService)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.browserManagerService = browserManagerService;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            this.postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.deviceInfoChart = deviceInfoChart ?? throw new ArgumentNullException(nameof(deviceInfoChart));
            this.kijijiPostingService = kijijiPostingService ?? throw new ArgumentNullException(nameof(kijijiPostingService));
        }
        public async Task Initialize()
        {
            await dbContext.Database.EnsureCreatedAsync();
            await TryInitBrowsers();
            await AddSettingIfNotExisted();
            await RefreshAllFailedOrOnOnGoingPost();
            await VerifyDevice();
            await SignInAsInit();
        }

        private async Task AddSettingIfNotExisted()
        {
            var existSetting = await settingRepository.Read();
            if (existSetting is not null)
            {
                return;
            }
            if (configuration is null)
            {
                return;
            }
            var newSetting = new Setting
            {
                Id = Guid.NewGuid(),
                KijijiEmail = configuration["KijijiAccount:Email"],
                KijijiPassword = configuration["KijijiAccount:Pass"],
                RegistrationId = Guid.Parse(configuration["RegistrationId"]),
                PageToTrigger = long.Parse(configuration["PageToTrigger"]),
                SleepInterval = int.Parse(configuration["RemoteDriver:SleepInterval"]),
                ReadInterval = int.Parse(configuration["RemoteDriver:ReadInterval"]),
                RePostInterval = int.Parse(configuration["RemoteDriver:RePostInterval"]),
                AdActiveInterval = int.Parse(configuration["RemoteDriver:AdActiveInterval"]),
                WaitAfterDeleteInterval = int.Parse(configuration["RemoteDriver:WaitAfterDeleteInterval"]),
            };
            await settingRepository.Create(newSetting);
        }

        private async Task TryInitBrowsers()
        {
            try
            {
               
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
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

        private async Task VerifyDevice()
        {
            await deviceInfoChart.Verify(doCheckFireBase: true);
        }

        private async Task SignInAsInit()
        {
            kijijiPostingService.Execute(KijijiExecuteType.Startup);
        }
    }
}
