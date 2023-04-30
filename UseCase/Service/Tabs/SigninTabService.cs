using Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace UseCase.Service.Tabs
{
    public class SigninTabService : BrowserTabService, ISigninTabService
    {
        private readonly ILogger<SigninTabService> logger;
        private readonly By _emailLocator;
        private readonly By _passLocator;
        private readonly By _submitLocator;
        private readonly By _SuccessTextLocator;
        private readonly By _AvatarLocator;
        private readonly By _LogOutLocator;
        public SigninTabService(IBrowserManagerService browserManagerService, IConfiguration configuration, ILogger<SigninTabService> logger)
            : base(KijijiBrowserTabs.Signin, browserManagerService, configuration, configuration["KijijiConfig:SignIn:Url"])
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailLocator = By.Id(Configuration["KijijiConfig:SignIn:Locators:EmailId"]);
            _passLocator = By.Id(Configuration["KijijiConfig:SignIn:Locators:PassId"]);
            _submitLocator = By.CssSelector(Configuration["KijijiConfig:SignIn:Locators:SubmitCssPath"]);
            _SuccessTextLocator =
                By.XPath($"//*[text()=\"{Configuration["KijijiConfig:SignIn:Locators:LoginSuccessText1"]}\"]|" +
                         $"//*[text()=\"{Configuration["KijijiConfig:SignIn:Locators:LoginSuccessText2"]}\"]");
            _AvatarLocator =
                By.CssSelector(Configuration["KijijiConfig:SignIn:Locators:Avatar"]);
            _LogOutLocator = 
                By.XPath($"//button[contains(text(),\"{Configuration["KijijiConfig:SignIn:Locators:Logout"]}\")]" );
        }

        public async Task Login(string email, string pass)
        {
            await Switch();
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            if (string.IsNullOrWhiteSpace(email)|| string.IsNullOrWhiteSpace(pass))
            {
                logger.LogInformation($"email ${email} or password {pass} is invalid or not set, so return");
                return;
            }
            if (WebDriver.Url.Equals(Configuration["KijijiConfig:SignIn:UrlToCheckIfLoginSuccessFully"]))
            {
                logger.LogInformation($"Login already so skip");
                return;
            }
            var emailElement = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(_emailLocator));
            emailElement.SendKeys(email);

            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var passElement = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(_passLocator));
            passElement.SendKeys(pass);

            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var submitElement = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementToBeClickable(_submitLocator));
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            submitElement.Click();
            WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(_SuccessTextLocator));
           
        }

        public async Task LoadForAppStartup()
        {
            await Switch();
        }

        public async Task Logout()
        {
            await Switch();
            if (!WebDriver.Url.Equals(Configuration["KijijiConfig:SignIn:UrlToCheckIfLoginSuccessFully"]))
            {
                logger.LogInformation($"Not Login yet, so return");
                return;
            }
            WebDriver.Navigate().GoToUrl(Configuration["KijijiConfig:SignIn:LogOutUrl"]);
            var avatar = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementToBeClickable(_AvatarLocator));
            avatar.Click();
            var logout = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementToBeClickable(_LogOutLocator));
            logout.Click();
        }
    }
}
