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
        public IDeviceRegistrationService DeviceRegistrationService { get; }
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
            IDeviceRegistrationService deviceRegistrationService,
            GlobalLockResourceService globalLockResourceService)
        {
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
            DeviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            GlobalLockResourceService = globalLockResourceService;
        }

        public void Execute(KijijiExecuteType kijijiExecuteType, ExecuteParams @params)
        {
            lock (GlobalLockResourceService.WebDriverLockResource)
            {
                try
                {
                    switch (kijijiExecuteType)
                    {
                        case KijijiExecuteType.ReadAds:
                            ReadAllAdExceedPage(@params.Page ?? throw new ArgumentNullException(nameof(@params.Page))).Wait();
                            break;
                        case KijijiExecuteType.PostAdByTitle:
                            StartRepostingWithTitle(@params.Post ?? throw new ArgumentNullException(nameof(@params.Post))).Wait();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(kijijiExecuteType), kijijiExecuteType, null);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogInformation(e.Message);
                }
            }
        }
    }
}
