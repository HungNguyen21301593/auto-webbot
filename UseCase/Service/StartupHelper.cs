using Entities;
using Infastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;

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

        public StartupHelper(IConfiguration configuration,
            IBrowserManagerService browserManagerService,
            ILogger<StartupHelper> logger,
            ISettingRepository settingRepository,
            IPostRepository postRepository,
            DataContext dbContext)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.browserManagerService = browserManagerService;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
            this.postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task Initialize()
        {
            await dbContext.Database.EnsureCreatedAsync();
            await TryInitBrowsers();
            await AddSettingIfNotExisted();
            await RefreshAllFailedOrOnOnGoingPost();
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
                SleepInterval = int.Parse(configuration["RemoveDriver:SleepInterval"]),
                ReadInterval = int.Parse(configuration["RemoveDriver:ReadInterval"]),
                RePostInterval = int.Parse(configuration["RemoveDriver:RePostInterval"]),
                AdActiveInterval = int.Parse(configuration["RemoveDriver:AdActiveInterval"]),
                WaitAfterDeleteInterval = int.Parse(configuration["RemoveDriver:WaitAfterDeleteInterval"]),
            };
            await settingRepository.Create(newSetting);
        }

        private async Task TryInitBrowsers()
        {
            try
            {
                browserManagerService.GetDriver();
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
            }
            await postRepository.UpdateRange(allFailedOrOnGoingPosts);
        }
    }
}
