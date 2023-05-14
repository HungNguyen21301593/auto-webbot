using Core.Model;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using Quartz;
using UseCase.Service;
using UseCase.Service.Tabs;

namespace UseCase.Jobs
{
    public class WebDriverStatusJob : IJob
    {
        public ILogger<WebDriverStatusJob> Logger { get; }
        public IBrowserManagerService BrowserManagerService { get; }
        public static readonly JobKey Key = new JobKey(nameof(WebDriverStatusJob));

        public WebDriverStatusJob(ILogger<WebDriverStatusJob> Logger, IBrowserManagerService browserManagerService)
        {
            this.Logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
            BrowserManagerService = browserManagerService ?? throw new ArgumentNullException(nameof(browserManagerService));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.Run(() =>
                {
                    var driver = BrowserManagerService.GetDriver();
                    var url = driver.Url;
                    Logger.LogInformation($"Driver is on, url: {url}");
                });
            }
            catch (Exception)
            {
                Logger.LogInformation("Driver is off");
            }
        }
    }
}
