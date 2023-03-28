using Core.Model;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace UseCase.Service.Tabs
{
    public class SigninTabService : BrowserTabService, ISigninTabService
    {
        private readonly By _emailLocator;
        private readonly By _passLocator;
        private readonly By _submitLocator;
        private readonly By _SuccessTextLocator;
        public SigninTabService(IBrowserManagerService browserManagerService, IConfiguration configuration)
            : base(KijijiBrowserTabs.Signin, browserManagerService, configuration, configuration["KijijiConfig:SignIn:Url"])
        {
            _emailLocator = By.Id(Configuration["KijijiConfig:SignIn:Locators:EmailId"]);
            _passLocator = By.Id(Configuration["KijijiConfig:SignIn:Locators:PassId"]);
            _submitLocator = By.CssSelector(Configuration["KijijiConfig:SignIn:Locators:SubmitCssPath"]);

            _SuccessTextLocator =
                By.XPath($"//*[text()=\"{Configuration["KijijiConfig:SignIn:Locators:LoginSuccessText1"]}\"]|" +
                         $"//*[text()=\"{Configuration["KijijiConfig:SignIn:Locators:LoginSuccessText2"]}\"]");
        }

        public async Task Login(string email, string pass)
        {
            await Switch();
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            if (WebDriver.Url.Equals(Configuration["KijijiConfig:SignIn:UrlToCheckIfLoginSuccessFully"]))
            {
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
    }
}
