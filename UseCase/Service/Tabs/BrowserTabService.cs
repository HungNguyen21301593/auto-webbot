using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UseCase.Service.Tabs
{
    public class BrowserTabService
    {
        public IConfiguration Configuration { get; }
        public KijijiBrowserTabs BrowserTab;
        public readonly IBrowserManagerService BrowserManagerService;
        public readonly IWebDriver WebDriver;
        public readonly TimeSpan SleepIntervalBetweenEachAcion;
        public readonly bool TestMode;

        public readonly string BaseTabUrl;
        public WebDriverWait WebWaiter { get; set; }

        public BrowserTabService(KijijiBrowserTabs browserTab, IBrowserManagerService browserManagerService, IConfiguration configuration, string? baseTabUrl)
        {
            Configuration = configuration;
            BrowserTab = browserTab;
            BrowserManagerService = browserManagerService;
            WebDriver = BrowserManagerService.GetDriver();
            WebWaiter = BrowserManagerService.GetDriverWaiter();
            SleepIntervalBetweenEachAcion = TimeSpan.FromSeconds(double.Parse(configuration["RemoveDriver:SleepInterval"] ?? "0"));
            TestMode = bool.Parse(Environment.GetEnvironmentVariable("TestMode") ?? configuration["TestMode"] ?? "true");
            BaseTabUrl = baseTabUrl ?? throw new ArgumentNullException(nameof(baseTabUrl));
        }

        public Task<IWebDriver> Switch()
        {
            WebDriver.SwitchTo().Window(BrowserManagerService.GetTabByName(BrowserTab.ToString()));
            WebDriver.Navigate().GoToUrl(BaseTabUrl);
            return Task.FromResult(WebDriver);
        }
    }
}
