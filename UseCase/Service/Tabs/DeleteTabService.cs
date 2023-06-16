using Core.Model;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace UseCase.Service.Tabs
{
    public class DeleteTabService : BrowserTabService, IDeleteTabService
    {
        public ILogger<DeleteTabService> Logger { get; }
        public string ActiveListUrl { get; set; }
        public string InActiveListUrl { get; set; }
        public KijijiActionHelper KijijiActionHelper { get; }
        private readonly By DeleteButtonLocator = By.XPath("//*[text()='Delete']");
        private readonly By ReasonToDeleteLocator = By.XPath("//*[text()='Prefer not to say']");
        private readonly By ProceedDeleteLocator = By.XPath($"//button[contains(text(),\"Delete My Ad\")]");
        public DeleteTabService(IBrowserManagerService browserManagerService, 
            IConfiguration configuration,
            ILogger<DeleteTabService> logger,
            KijijiActionHelper kijijiActionHelper)
            : base(KijijiBrowserTabs.Delete, browserManagerService, configuration, configuration["KijijiConfig:DeleteAd:Url"])
        {
            Logger = logger;
            KijijiActionHelper = kijijiActionHelper;
            ActiveListUrl = configuration["KijijiConfig:DeleteAd:Url"]?? "";
            InActiveListUrl = configuration["KijijiConfig:DeleteAd:InActiveUrl"] ?? "";
        }

        public async Task DeleteAdByTitle(Post post, bool activeList = true)
        {
            var adDetails = JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson);
            var title = adDetails.AdTitle;
            await Task.Run(async () =>
            {
                await Switch(activeList ? ActiveListUrl : InActiveListUrl);
                await KijijiActionHelper.ExecuteAndSaveResult(() =>
                {
                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                    Logger.LogInformation(
                        $"{StepType.SearchAdBeforeDelete} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                    var searchAdByTitleLocator = By.XPath($"//a[contains(normalize-space(),\"{title.Trim()}\")]");
                    var adTitleElement = WebWaiter
                        .Until(SeleniumExtras
                            .WaitHelpers
                            .ExpectedConditions
                            .ElementToBeClickable(searchAdByTitleLocator));
                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                    adTitleElement.Click();

                    return adDetails;
                }, post, StepType.SearchAdBeforeDelete);

                await KijijiActionHelper.ExecuteAndSaveResult(() =>
                {
                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                    Logger.LogInformation(
                        $"{StepType.SubmitDelete} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                    ProceedDeleteSingleAd();

                    return adDetails;
                }, post, StepType.SubmitDelete);
            });
        }

        private void ProceedDeleteSingleAd()
        {
            var deleteButton = WebDriver.FindElements(DeleteButtonLocator);
            if (deleteButton.Any())
            {
                deleteButton.First().Click();
            }

            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var reason = WebDriver.FindElements(ReasonToDeleteLocator);
            if (reason.Any())
            {
                reason.First().Click();
            }

            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var proceedDeletes = WebDriver.FindElements(ProceedDeleteLocator);
            if (!proceedDeletes.Any()) return;
            var proceedDelete = proceedDeletes
                .FirstOrDefault(l => l.TagName == "button");
            if (proceedDelete is null)
            {
                throw new Exception("could not find the delete button");
            }

            if (TestMode)
            {
                Logger.LogInformation("App is in testing mode, so no Ad will be deleted");
            }
            else
            {
                proceedDelete.Click();
            }
        }
    }
}
