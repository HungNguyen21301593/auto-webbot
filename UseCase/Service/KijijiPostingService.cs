using Core.Model;
using Entities;
using FireBaseAuthenticator.KijijiHelperServices;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using UseCase.Service.Tabs;

namespace UseCase.Service
{
    public partial class KijijiPostingService : IKijijiPostingService
    {
        private readonly IFireBaseLoggingService fireBaseLoggingService;
        private IBrowserManagerService BrowserManager { get; }
        public ISigninTabService SigninService { get; }
        private IReadAdTabService ReadAdTabService { get; }
        private IDeleteTabService DeleteTabService { get; }
        public IPostTabService PostTabService { get; }
        public IRequestRepository RequestRepository { get; }
        public IPostRepository PostRepository { get; }
        public ISettingRepository SettingRepository { get; }
        public ILogger<KijijiPostingService> Logger { get; }
        public IStepLogRepository StepLogRepository { get; }
        public IDeviceInfoChart DeviceInfoChart { get; }
        public GlobalLockResourceService GlobalLockResourceService { get; }

        public KijijiPostingService(IBrowserManagerService browserManager,
            ISigninTabService signinService,
            IReadAdTabService readAdTabService,
            IDeleteTabService deleteTabService,
            IPostTabService postTabService,
            IRequestRepository requestRepository,
            IPostRepository postRepository,
            ISettingRepository settingRepository,
            ILogger<KijijiPostingService> logger,
            IStepLogRepository stepLogRepository,
            IDeviceInfoChart deviceInfoChart,
            IFireBaseLoggingService fireBaseLoggingService,
            GlobalLockResourceService globalLockResourceService)
        {
            this.fireBaseLoggingService = fireBaseLoggingService ?? throw new ArgumentNullException(nameof(fireBaseLoggingService));
            ReadAdTabService = readAdTabService;
            BrowserManager = browserManager ?? throw new ArgumentNullException(nameof(browserManager));
            SigninService = signinService ?? throw new ArgumentNullException(nameof(signinService));
            DeleteTabService = deleteTabService ?? throw new ArgumentNullException(nameof(deleteTabService));
            PostTabService = postTabService ?? throw new ArgumentNullException(nameof(postTabService));
            RequestRepository = requestRepository;
            PostRepository = postRepository;
            SettingRepository = settingRepository;
            Logger = logger;
            StepLogRepository = stepLogRepository;
            DeviceInfoChart = deviceInfoChart ?? throw new ArgumentNullException(nameof(deviceInfoChart));
            GlobalLockResourceService = globalLockResourceService;
        }

        public void Execute(KijijiExecuteType kijijiExecuteType, ExecuteParams @params)
        {
            lock (GlobalLockResourceService.WebDriverLockResource)
            {
                try
                {
                    var verify = DeviceInfoChart.Verify().Result;
                    if (!verify.IsVerified)
                    {
                        Logger.LogInformation("The current device is not verified or expired. So return");
                        return;
                    }

                    SetupTabs();
                    switch (kijijiExecuteType)
                    {
                        case KijijiExecuteType.ReadAds:
                            TryLogInAndLogOutIfNewAccountSetting(@params.Setting).Wait();
                            ReadAllAdExceedPage(0).Wait();
                            break;
                        case KijijiExecuteType.PostAdByTitle:
                            TryLogInAndLogOutIfNewAccountSetting(@params.Setting).Wait();
                            StartRepostingWithTitle(@params.Post, @params.Setting).Wait();
                            break;
                        case KijijiExecuteType.Startup:
                            SigninService.LoadForAppStartup().Wait();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(kijijiExecuteType), kijijiExecuteType, null);
                    }
                }
                catch (Exception e)
                {
                    fireBaseLoggingService.LogError(e.Message);
                    Logger.LogInformation(e.Message);
                }
            }
        }
    }
}
