using System.Linq.Expressions;
using Core.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Net;
using Entities;
using Infastructure.Repositories;
using Microsoft.Extensions.Logging;
using NLog.Fluent;

namespace UseCase.Service.Tabs
{
    public class ReadAdTabService : BrowserTabService, IReadAdTabService
    {
        public ILogger<ReadAdTabService> Logger { get; }
        private readonly KijijiActionHelper _kijijiActionHelper;
        private By AdItemsLocator = By.CssSelector("tr[class*='item-']");
        private By AdTitlesLocator = By.CssSelector("div[class*='titleAndPrice']");
        private By AdPageLocator = By.CssSelector("td[class*='pageCell']");

        private By SearchAdByTitleLocator;
        private By EditAdLocator = By.CssSelector("button[class*=editButton]");
        private By categoriesLocators1 = By.CssSelector("span[class*='breadcrumb-']");
        private By adtitleLocator = By.CssSelector("h1[class*='title']");
        private By desLocator = By.Id("pstad-descrptn");
        private By tagsLocators = By.CssSelector("li[class*='tagItem']");
        private By addressLocator = By.Id("servicesLocationInput");
        private By locationLocator = By.CssSelector("div[class*='locationText-']");
        private By imageLocators = By.CssSelector("#MediaUploadedImages .image-area .image");
        private By companyLocator = By.Id("company_s");
        private By typeLocator = By.Id("type_s");
        private By carYearLocator = By.Id("caryear_i");
        private By carKmLocator = By.Id("carmileageinkms_i");
        private List<By> dynamicLabelsLocators = new()
        {
            By.CssSelector("div[data-qa-id='active-listings-stat-line']"),
            By.CssSelector("span[class*='noLabelValue']"),
            By.CssSelector("dd[class*='attributeValue']"),
            By.CssSelector("div[class*='line-'")
        };
        private By priceLocators = By.Id("PriceAmount");
        private By phoneLocator = By.Id("PhoneNumber");

        public ReadAdTabService(IBrowserManagerService browserManagerService, 
            IConfiguration configuration,
            ILogger<ReadAdTabService> logger,
            KijijiActionHelper kijijiActionHelper)
            : base(KijijiBrowserTabs.AllAds, browserManagerService, configuration, configuration["KijijiConfig:ReadAd:Url"])
        {
            Logger = logger;
            _kijijiActionHelper = kijijiActionHelper;
        }

        public async Task<List<string>> ReadAllAdTitlesExceedPage(long exceedPage)
        {
            await Switch();
            var result = new List<string>();
            var aditems = WebDriver.FindElements(AdItemsLocator);
            foreach (var aditem in aditems)
            {
                var pageElement = aditem.FindElement(AdPageLocator);
                var span = pageElement.FindElement(By.TagName("span"));
                var pageText = span.Text;
                var isNumber = int.TryParse(pageText, out var page);
                if (isNumber && page <= exceedPage)
                {
                    continue;
                }
                var titleElement = aditem.FindElement(AdTitlesLocator);
                result.Add(titleElement.Text);
            }
            return result;
        }

        public async Task<bool> SearchAdTitle(string title)
        {
            try
            {
                await Switch();
                SearchAdByTitleLocator = By.XPath($"//a[contains(text(),\"{title}\")]");
                var adTitleElement = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementToBeClickable(SearchAdByTitleLocator));
                return adTitleElement is not null;
            }
            catch (Exception e)
            {
                Logger.LogWarning($"There was an error finding active ad with title {title}");
                return false;
            }
        }

        public async Task<AdDetails> ReadAdContentByTitle(string title, Post post)
        {
            await Switch();
            SearchAdByTitleLocator = By.XPath($"//a[contains(text(),\"{title}\")]");
            var adTitleElement = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementToBeClickable(SearchAdByTitleLocator));
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            adTitleElement.Click();

            return await ReadAdContent(post);
        }

        private async Task<AdDetails> ReadAdContent(Post post)
        {
            var adDetails = new AdDetails();
            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadDynamicsTexts(adDetails);
                Logger.LogInformation($"ReadDynamicsTexts {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadDynamicText);
            
            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadAdTitle(adDetails);
                Logger.LogInformation($"ReadAdTitle {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadTitle);

            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var editAd = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementToBeClickable(EditAdLocator));
            editAd.Click();

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                DownloadPics(adDetails);
                Logger.LogInformation($"DownloadPics {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.DownloadPics);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadCategories(adDetails);
                Logger.LogInformation($"ReadCategories {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadCategories);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadAdDescription(adDetails);
                Logger.LogInformation($"ReadAdDescription {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadAdDescription);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadTags(adDetails);
                Logger.LogInformation($"ReadTags {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadTags);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadAddress(adDetails);
                Logger.LogInformation($"ReadAddress {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadAddress);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadLocation(adDetails);
                Logger.LogInformation($"ReadLocation {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadLocation);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadPrice(adDetails);
                Logger.LogInformation($"ReadPrice {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadPrice);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadCompany(adDetails);
                Logger.LogInformation($"ReadCompany {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadCompany);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadTypes(adDetails);
                Logger.LogInformation($"ReadTypes {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadTypes);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadCarYear(adDetails);
                Logger.LogInformation($"ReadCarYear {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadCarYear);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadCarKm(adDetails);
                Logger.LogInformation($"ReadCarKm {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadCarKm);

            await _kijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                ReadPhoneNumber(adDetails);
                Logger.LogInformation($"ReadPhoneNumber {JsonConvert.SerializeObject(adDetails)}");
                return adDetails;
            }, post, StepType.ReadPhoneNumber);

            return await Task.FromResult(adDetails);
        }

        private void ReadDynamicsTexts(AdDetails adDetails)
        {
            foreach (var locator in dynamicLabelsLocators)
            {
                var elements = WebDriver.FindElements(locator);
                if (!elements.Any()) { continue; }
                foreach (var element in elements)
                {
                    var textValue = element.Text;
                    adDetails.DynamicTextOptions.Add(textValue.Trim());
                }
            }
        }

        private void ReadTypes(AdDetails adDetails)
        {
            var types = WebDriver.FindElements(typeLocator);
            if (types.Any())
            {
                adDetails.Type = types.First().GetAttribute("value");
            }
        }

        private void ReadCarYear(AdDetails adDetails)
        {
            var items = WebDriver.FindElements(carYearLocator);
            if (items.Any())
            {
                adDetails.CarYear = items.First().GetAttribute("value");
            }
        }

        private void ReadCarKm(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var items = WebDriver.FindElements(carKmLocator);
            if (items.Any())
            {
                adDetails.CarKm = items.First().GetAttribute("value");
            }
        }

        private void ReadCompany(AdDetails adDetails)
        {
            var company = WebDriver.FindElements(companyLocator);
            if (company.Any())
            {
                adDetails.Company = company.First().GetAttribute("value");
            }
        }

        private void ReadPrice(AdDetails adDetails)
        {
            var prices = WebDriver.FindElements(priceLocators);
            if (!prices.Any()) return;
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var value = prices.First().GetAttribute("value");
            if (value.Equals(string.Empty))
            {
                adDetails.Price = null;
                return;
            }
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            double.TryParse(value, out var result);
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            adDetails.Price = result;
        }

        private void DownloadPics(AdDetails adDetails)
        {
            adDetails.InputPicturePaths = new List<string>();
            var matchedSettings = Configuration.GetSection("KijijiConfig:SpecicalAdtitleSetting").Get<List<string>>()?
                .Where(s => adDetails.AdTitle.ToLower().Contains(s.ToLower()))
                .ToList();

            if (matchedSettings != null && matchedSettings.Any())
            {
                adDetails.InputPicturePaths = UsedSpecialAdSetting(matchedSettings);
                return;
            }
            adDetails.InputPicturePaths = DownloadNormalAdPics();
        }

        private List<string> UsedSpecialAdSetting(List<string> matchedSettings)
        {
            var inputPicturePaths = new List<string>();
            if (matchedSettings.Count() > 1)
            {
                Logger.LogInformation($"Warning, multiple matched SpecialAdSetting, {string.Join(',', matchedSettings)}");
            }

            foreach (var matchedSetting in matchedSettings)
            {
                var directoryInfo = new DirectoryInfo($"{Directory.GetCurrentDirectory()}\\{matchedSetting}");
                var allFiles = directoryInfo.GetFiles();
                inputPicturePaths.AddRange(allFiles.Select(f => f.FullName));
            }
            return inputPicturePaths;
        }

        private List<string> DownloadNormalAdPics()
        {
            var inputPicturePaths = new List<string>();
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var allImages = WebDriver.FindElements(imageLocators);
            if (!allImages.Any()) { return inputPicturePaths; }
            Thread.Sleep(SleepIntervalBetweenEachAcion);

            var urls = new List<string>();
            foreach (var image in allImages)
            {
                var url = image.GetAttribute("data-large-url");
                if (url is null)
                {
                    continue;
                }
                urls.Add(url.Trim());
            }
            using var client = new WebClient();
            foreach (var url in urls.Distinct())
            {
                var fileName = $"{Guid.NewGuid()}.JPG";
                client.DownloadFile(url, fileName);
                var savedPath = Path.GetFullPath(Path.Combine($"{Environment.CurrentDirectory}/images", fileName));
                if (new FileInfo(fileName).Length < 10000)
                {
                    File.Delete(fileName);
                    continue;
                }
                File.Move(fileName, savedPath);
                inputPicturePaths.Add(Path.GetFullPath(savedPath));
            }
            var action = new Actions(WebDriver);
            action.SendKeys(Keys.Escape);
            return inputPicturePaths;
        }

        private void ReadAddress(AdDetails adDetails)
        {
            var items = WebDriver.FindElements(addressLocator);
            if (items.Any())
            {
                adDetails.Address = items.First().Text;
            }
        }

        private void ReadLocation(AdDetails adDetails)
        {
            var items = WebDriver.FindElements(locationLocator);
            if (items.Any())
            {
                adDetails.Location = items.First().Text;
            }
        }

        private void ReadTags(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var tags = WebDriver.FindElements(tagsLocators);
            if (tags.Any())
            {
                adDetails.Tags = new List<string>();
                foreach (var item in tags)
                {
                    adDetails.Tags.Add(item.Text);
                }
            }
        }

        private void ReadAdDescription(AdDetails adDetails)
        {
            var items = WebDriver.FindElements(desLocator);
            if (items.Any())
            {
                adDetails.Description = items.First().Text;
            }
        }

        private void ReadAdTitle(AdDetails adDetails)
        {
            var items = WebDriver.FindElements(adtitleLocator);
            if (items.Any())
            {
                adDetails.AdTitle = items.First().Text;
            }
        }

        private void ReadPhoneNumber(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var phoneNumber = WebDriver.FindElement(phoneLocator);
            adDetails.PhoneNumber = phoneNumber.GetAttribute("value");
        }

        private void ReadCategories(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var categories = WebDriver
                            .FindElements(categoriesLocators1);
            if (!categories.Any())
            {
                Logger.LogInformation("Warning: could not locale categories, try another localtor");
                Logger.LogInformation("Trying className: category -> TagName: strong");
                var forms = WebDriver
                            .FindElements(By.ClassName("category"));
                if (forms.Any())
                {
                    categories = forms.First().FindElements(By.TagName("strong"));
                }
            }

            adDetails.Categories = new List<string>();
            foreach (var categorie in categories)
            {
                adDetails.Categories.Add(categorie.Text);
            }
        }
    }
}
