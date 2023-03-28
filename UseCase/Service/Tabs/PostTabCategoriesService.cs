using Core.Model;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace UseCase.Service.Tabs
{
    public partial class PostTabCategoriesService : BrowserTabService, IPostTabService
    {
        public ILogger<PostTabCategoriesService> Logger { get; }
        public KijijiActionHelper KijijiActionHelper { get; }
        private By AdTitleLocaltor = By.Id("AdTitleForm");
        private By NextButtonLocaltor = By.XPath("//*[text()='Next']");

        private List<string> DefaultPostLocations;
        public PostTabCategoriesService(IBrowserManagerService browserManagerService, 
            IConfiguration configuration,
            ILogger<PostTabCategoriesService> logger,
            KijijiActionHelper kijijiActionHelper)
            : base(KijijiBrowserTabs.PostNew, browserManagerService, configuration, configuration["KijijiConfig:PostAd:Url"])
        {
            Logger = logger;
            KijijiActionHelper = kijijiActionHelper;
            DefaultPostLocations = Configuration.GetSection("KijijiConfig:PostAd:DefaultPostLocations").Get<List<string>>() ?? new List<string>();
        }

        public async Task SubmitCategories(Post existingPost)
        {
            await Switch();
            var adDetails = JsonConvert.DeserializeObject<AdDetails>(existingPost.AdDetailJson);
            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation($"InputTitle {JsonConvert.SerializeObject(existingPost.AdDetailJson)}");
                var inputTitle = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementIsVisible(AdTitleLocaltor));
                inputTitle.SendKeys(adDetails.AdTitle);

                var nextButton = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementIsVisible(NextButtonLocaltor));
                nextButton.Click();
                return adDetails;
            }, existingPost, StepType.InputTitle);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation($"SelectCategories {JsonConvert.SerializeObject(existingPost.AdDetailJson)}");
                if (adDetails.Categories.Any())
                {
                    SelectCategories(adDetails);
                }
                return adDetails;
            }, existingPost, StepType.SelectCategories);
        }

        private void SelectCategories(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            foreach (var category in adDetails.Categories)
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                var categoryButtons =
                    WebWaiter
                            .Until(SeleniumExtras
                                .WaitHelpers
                                .ExpectedConditions
                                .VisibilityOfAllElementsLocatedBy(By.CssSelector("span[class*='categoryName']")));
                
                if (!categoryButtons.Any())
                {
                    throw new Exception("Could not find categoryButtons");
                }
                categoryButtons.Where(b => b.Text == category).LastOrDefault()?.Click();
            }
            foreach (var location in DefaultPostLocations)
            {
                ClickLocation(location);
            }
            ClickGo();
        }

        private void ClickLocation(string location)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var element = WebDriver.FindElements(By.XPath($"//*[text()='{location}']"));
            if (!element.Any())
            {
                Logger.LogInformation($"Warning, could not find AdGlobalSetting-location {location}");
                return;
            }
            element.First().Click();
        }

        private void ClickGo()
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var element = WebDriver.FindElements(By.Id("LocUpdate"));
            if (!element.Any())
            {
                Logger.LogInformation("Warning, could not found Go button");
                return;
            }
            element.First().Click();
        }
    }
}
