﻿using System;
using System.Collections.Generic;
using System.Text;

namespace auto_webbot.Pages.Post
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using global::auto_webbot.Model;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;

    namespace auto_webbot.Pages.Post
    {
        public class AdDetailInputPage
        {
            public IWebDriver webDriver { get; set; }
            public AppSetting config { get; set; }
            public AdDetailInputPage(IWebDriver webDriver, AppSetting config)
            {
                this.webDriver = webDriver;
                this.config = config;
            }
            private WebDriverWait WebWaiter => new WebDriverWait(webDriver, TimeSpan.FromSeconds(20));
            private By DescriptionLocaltor = By.Id("pstad-descrptn");
            private By FileInputWrapper = By.ClassName("imageUploadButtonWrapper");
            private By ChangeLocationButtonLocator = By.XPath("//*[text()='Change']");
            private By LocationLocator = By.Id("location");
            private By addressLocator = By.Id("servicesLocationInput");
            private By addressLocatorFirst = By.Id("LocationSelector-item-0");
            private By PriceLocator = By.Id("PriceAmount");
            private By PostButtonLocator = By.CssSelector("button[type='submit']");
            private By companyLocator = By.Id("company_s");
            private By carYearLocator = By.Id("caryear_i");
            private By carKmLocator = By.Id("carmileageinkms_i");
            private By selectBasicPackage = By.CssSelector("button[data-qa-id='package-0-bottom-select']");

            public void InputAdDetails(AdDetails adDetails)
            {
                if (adDetails.Description != null)
                {
                    Thread.Sleep(1000);
                    InputDesciption(adDetails);
                    Console.WriteLine("InputDesciption");
                }
                if (adDetails.AdType != null)
                {
                    Thread.Sleep(1000);
                    SelectAdtype(adDetails);
                    Console.WriteLine("SelectAdtype");
                }
                if (adDetails.ForSaleBy != null)
                {
                    Thread.Sleep(1000);
                    SelectForSaleBy(adDetails);
                    Console.WriteLine("SelectForSaleBy");
                }
                if (adDetails.MoreInfo != null)
                {
                    Thread.Sleep(1000);
                    SelectMoreInfo(adDetails);
                    Console.WriteLine("SelectMoreInfo");
                }
                if (adDetails.Fulfillments.Any())
                {
                    Thread.Sleep(1000);
                    SelectFulfillment(adDetails);
                    Console.WriteLine("SelectFulfillment");
                }
                if (adDetails.Payments.Any())
                {
                    Thread.Sleep(1000);
                    SelectPayment(adDetails);
                    Console.WriteLine("SelectFulfillment");
                }
                if (adDetails.Tags.Any())
                {
                    Thread.Sleep(1000);
                    SelectTags(adDetails);
                    Console.WriteLine("SelectTags");
                }
                if (adDetails.InputPicturePaths.Any())
                {
                    Thread.Sleep(1000);
                    InputPicture(adDetails);
                    Console.WriteLine("InputPicture");
                }
                if (adDetails.Location != null)
                {
                    Thread.Sleep(1000);
                    InputLocation(adDetails.Location);
                    Console.WriteLine("InputLocation");
                }
                if (adDetails.Address != null)
                {
                    Thread.Sleep(1000);
                    InputAddress(adDetails);
                    Console.WriteLine("InputAddress");
                }
                if (adDetails.Price != 0)
                {
                    Thread.Sleep(1000);
                    InputPrice(adDetails);
                    Console.WriteLine("InputPrice");
                }
                if (adDetails.Company != null)
                {
                    Thread.Sleep(1000);
                    InputCompany(adDetails);
                    Console.WriteLine("InputCompany");
                }
                if (adDetails.DynamicTextOptions.Any())
                {
                    InputDynamicInputs(adDetails);
                    Thread.Sleep(1000);
                    Console.WriteLine("InputDynamicInputs");
                    InputDynamicSelectOptions(adDetails);
                    Thread.Sleep(1000);
                    Console.WriteLine("InputDynamicSelectOptions");
                }

                if (adDetails.CarYear != null)
                {
                    InputCarYear(adDetails);
                    Thread.Sleep(1000);
                    Console.WriteLine("InputCarYear");
                }

                if (adDetails.CarKm != null)
                {
                    InputCarKm(adDetails);
                    Thread.Sleep(1000);
                    Console.WriteLine("InputCarKm");
                }

                Thread.Sleep(1000);
                SelectBasicPakage();
                Thread.Sleep(1000);
                if (config.Mode == Mode.test)
                {
                    Console.WriteLine("App is in testing node, so no ad will be actual posted");
                }
                else
                {
                    Post();
                }
            }

            private void SelectBasicPakage()
            {
                var elements = webDriver.FindElements(selectBasicPackage);
                if (elements.Any())
                {
                    elements.First().Click();
                }
            }

            private void InputDynamicInputs(AdDetails adDetails)
            {
                foreach (var option in adDetails.DynamicTextOptions)
                {
                    var elements = webDriver.FindElements(By.XPath($"//*[text()='{option}']"));
                    if (elements.Any())
                    {
                        elements.First().Click();
                    }
                }
            }

            private void InputDynamicSelectOptions(AdDetails adDetails)
            {
                Thread.Sleep(1000);
                var selects = webDriver.FindElements(By.TagName("select"));
                if (selects.Any())
                {
                    foreach (var select in selects)
                    {
                        try
                        {
                            var selectElement = new SelectElement(select);
                            foreach (var dynamicText in adDetails.DynamicTextOptions)
                            {
                                Console.WriteLine($"Trying to input {select.GetAttribute("name")} - {dynamicText}");
                                foreach (IWebElement element in selectElement.Options)
                                {
                                    if (element.Text.Equals(dynamicText))
                                    {
                                        Thread.Sleep(1000);
                                        element.Click();
                                        Console.WriteLine($"Selected |{dynamicText}| on {select.GetAttribute("name")}");
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Skip {select.GetAttribute("name")}, {e.Message}, {e.StackTrace}");
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            private void InputCompany(AdDetails adDetails)
            {
                var element = WebWaiter
                                .Until(SeleniumExtras
                                    .WaitHelpers
                                    .ExpectedConditions
                                    .ElementIsVisible(companyLocator));
                element.SendKeys(adDetails.Company);
            }

            private void InputCarYear(AdDetails adDetails)
            {
                Thread.Sleep(1000);
                var elements = webDriver.FindElements(carYearLocator);
                if (elements.Any())
                {
                    elements.First().SendKeys(adDetails.CarYear);
                }
            }

            private void InputCarKm(AdDetails adDetails)
            {
                Thread.Sleep(1000);
                var elements = webDriver.FindElements(carKmLocator);
                if (elements.Any())
                {
                    elements.First().SendKeys(adDetails.CarKm);
                }
            }

            private void InputAddress(AdDetails adDetails)
            {
                var addressElement = WebWaiter
                                .Until(SeleniumExtras
                                    .WaitHelpers
                                    .ExpectedConditions
                                    .ElementIsVisible(addressLocator));
                addressElement.SendKeys(adDetails.Address);
                Thread.Sleep(2000);
                var addressElementFirst = WebWaiter
                               .Until(SeleniumExtras
                                   .WaitHelpers
                                   .ExpectedConditions
                                   .ElementToBeClickable(addressLocatorFirst));
                addressElementFirst.Click();
            }

            private void SelectTags(AdDetails adDetails)
            {
                Thread.Sleep(2000);
                var inputTag = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(By.Id("pstad-tagsInput")));
                foreach (var tag in adDetails.Tags)
                {
                    inputTag.SendKeys(tag);
                    inputTag.SendKeys(Keys.Enter);
                }
            }

            private void SelectPayment(AdDetails adDetails)
            {
                foreach (var payment in adDetails.Payments)
                {
                    Thread.Sleep(2000);
                    var Payment = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementIsVisible(By.XPath($"//*[text()='{payment}']")));
                    Payment.Click();
                }
            }

            private void SelectFulfillment(AdDetails adDetails)
            {
                foreach (var Fulfillment in adDetails.Fulfillments)
                {
                    Thread.Sleep(2000);
                    var input = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementIsVisible(By.XPath($"//*[text()='{Fulfillment}']")));
                    input.Click();
                }
            }

            private void SelectAdtype(AdDetails adDetails)
            {
                Thread.Sleep(2000);
                var adtype = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(By.XPath($"//*[text()='{adDetails.AdType}']")));
                adtype.Click();
            }

            private void SelectForSaleBy(AdDetails adDetails)
            {
                Thread.Sleep(2000);
                var forSaleBy = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(By.XPath($"//*[text()='{adDetails.ForSaleBy}']")));
                forSaleBy.Click();
            }

            private void SelectMoreInfo(AdDetails adDetails)
            {
                var moreInforExist = webDriver.FindElements(By.Id("moreinfo_s"));
                if (moreInforExist.Any())
                {
                    var select = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(By.Id("moreinfo_s")));
                    select.Click();

                    Thread.Sleep(2000);
                    var MoreInfo = WebWaiter
                    .Until(SeleniumExtras
                        .WaitHelpers
                        .ExpectedConditions
                        .ElementIsVisible(By.XPath($"//*[text()='{adDetails.MoreInfo}']")));
                    MoreInfo.Click();
                }
            }

            private void Post()
            {
                Thread.Sleep(2000);
                var post = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(PostButtonLocator));
                post.Click();
            }

            private void InputPrice(AdDetails adDetails)
            {
                Thread.Sleep(2000);
                var price = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(PriceLocator));
                price.SendKeys(adDetails.Price.ToString());
            }

            private void InputPicture(AdDetails adDetails)
            {
                var fileInputWrapper = webDriver.FindElement(FileInputWrapper);
                var updatefile = fileInputWrapper.FindElement(By.TagName("input"));

                foreach (var path in adDetails.InputPicturePaths)
                {
                    updatefile.SendKeys(path);
                    Thread.Sleep(2000);
                }
                Thread.Sleep(2000);
            }

            private void InputDesciption(AdDetails adDetails)
            {
                var description = WebWaiter
                                .Until(SeleniumExtras
                                    .WaitHelpers
                                    .ExpectedConditions
                                    .ElementIsVisible(DescriptionLocaltor));
                description.SendKeys(adDetails.Description);
            }

            private void InputLocation(string locationPrefix)
            {
                InputLocatonOnce(locationPrefix);
            }

            private void InputLocatonOnce(string locationText)
            {
                var changeButton = webDriver.FindElements(ChangeLocationButtonLocator);
                if (changeButton.Any())
                {
                    changeButton.First().Click();
                    CleanLocaltion();
                }
                var location = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(LocationLocator));
                location.SendKeys(locationText);
            }

            private void CleanLocaltion()
            {
                var location = WebWaiter
                    .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(LocationLocator));
                location.Clear();
            }
        }
    }

}