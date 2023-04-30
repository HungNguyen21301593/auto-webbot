using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace UseCase.Service
{
    public class BrowserManagerService : IBrowserManagerService
    {
        public IConfiguration Configuration { get; }
        public ILogger<BrowserManagerService> Logger { get; }

        private readonly object driverLock = new();
        private IWebDriver? Driver { get; set; }
        private Dictionary<string, string> TabNamesDictionary = new();
        private readonly List<string> arguments = new()
        {
            "--disable-notifications",
            "--start-maximized",
            //"--incognito",
            "--ignore-ssl-errors",
            "--ignore-certificate-errors",
            //"--disable-extensions",
            //"--headless",
            "no-sandbox",
            "--disable-gpu",
            "--disable-logging",
            "--disable-popup-blocking",
            "disable-blink-features=AutomationControlled",
            "--disable-dev-shm-usage",
            "--log-level=3",
            "--disable-application-cache",
            "enable-features=NetworkServiceInProcess",
            "--disable-features=NetworkService",
            };

        public BrowserManagerService(IConfiguration configuration, ILogger<BrowserManagerService> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IWebDriver GetDriver()
        {
            lock (driverLock)
            {
                return Driver ?? SetupNewInstance();
            }
        }

        public WebDriverWait GetDriverWaiter()
        {
            var driver = GetDriver();
            return new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }


        private IWebDriver SetupNewInstance()
        {
            try
            {
                new DriverManager().SetUpDriver(new ChromeConfig(), "101.0.4951.41");
            }
            catch (Exception)
            {
            }
            var options = new ChromeOptions();
            options.AddExcludedArgument("enable-automation");
            options.AddArguments(arguments);
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            var settings = new RemoteSessionSettings(options);
            var remoteDriverUrl = Environment.GetEnvironmentVariable("REMOTE_DRIVER_URL");
            Logger.LogInformation($"Found REMOTE_DRIVER_URL: {remoteDriverUrl}");
            var webdriverUrl = remoteDriverUrl ?? Configuration["RemoveDriver:Url"];
            Logger.LogInformation($"Connecting to webdriver server {webdriverUrl}");
            var remoteDriver = new RemoteWebDriver(new Uri(webdriverUrl ?? string.Empty), settings);
            Logger.LogInformation($"Connected to webdriver server {webdriverUrl}...");
            Driver = remoteDriver ?? throw new ArgumentNullException($"Could not init web diver");
            return remoteDriver;
        }

        public void OpenNewTabAndSetName(string tabName)
        {
            var jsExecutor = Driver as IJavaScriptExecutor;
            if (jsExecutor is null)
            {
                throw new InvalidOperationException("Could not convert webdriver to jsExecutor");
            }

            if (TabNamesDictionary.ContainsKey(tabName))
            {
                return;
            }
            jsExecutor.ExecuteScript("window.open();");
            var tabHashCode = Driver?.WindowHandles.Last();
            TabNamesDictionary.Add(tabName, tabHashCode);
        }

        public string GetTabByName(string tabName)
        {
            TabNamesDictionary.TryGetValue(tabName, out var tab);
            if (tab is null)
            {
                throw new InvalidOperationException($"Could not get tab {tabName}");
            }
            return tab;
        }

        public void Restart()
        {
            Driver?.Quit();
            TabNamesDictionary.Clear();
            Driver = SetupNewInstance();
        }
    }
}
