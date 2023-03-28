using Core.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace UseCase.Service.Tabs
{
    public partial class PostTabCategoriesService
    {
        private By AdTitleLocator = By.Id("postad-title");
        private By DescriptionLocaltor = By.Id("pstad-descrptn");
        private By FileInputWrapper = By.ClassName("imageUploadButtonWrapper");
        private By ChangeLocationButtonLocator = By.XPath("//*[text()='Change']");
        private By LocationLocator = By.Id("location");
        private By LocationFirstLocator = By.Id("LocationSelector-item-0");
        private By addressLocator = By.Id("servicesLocationInput");
        private By addressLocatorFirst = By.Id("LocationSelector-item-0");
        private By PriceLocator = By.Id("PriceAmount");
        private By PostButtonLocator = By.CssSelector("button[type='submit']");
        private By companyLocator = By.Id("company_s");
        private By carYearLocator = By.Id("caryear_i");
        private By carKmLocator = By.Id("carmileageinkms_i");
        private By selectBasicPackage = By.CssSelector("button[data-qa-id='package-0-bottom-select']");
        private By termAndConditions = By.CssSelector("span[class='checkbox-label']");
        private By PricePleaseContactLocator = By.XPath("//*[text()='Please Contact']");
        private By phoneLocator = By.Id("PhoneNumber");


        public async Task<bool> InputAdDetails(Post post)
        {
            var adDetails = JsonConvert.DeserializeObject<AdDetails>(post.AdDetailJson);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation($"InputDescription {JsonConvert.SerializeObject(post.AdDetailJson)}");
                if (adDetails.Description == null) {return adDetails;}
                InputDesciption(adDetails);
                return adDetails;
            }, post, StepType.InputDescription);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectAdtype} {JsonConvert.SerializeObject(post.AdDetailJson)}");
                if (adDetails.AdType == null) return adDetails;
                SelectAdtype(adDetails);
                return adDetails;
            }, post, StepType.SelectAdtype);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectForSaleBy} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.ForSaleBy == null) return adDetails;
                SelectForSaleBy(adDetails);
                return adDetails;
            }, post, StepType.SelectForSaleBy);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectMoreInfo} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.MoreInfo == null) {return adDetails;}
                SelectMoreInfo(adDetails);
                return adDetails;
            }, post, StepType.SelectMoreInfo);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectMoreInfo} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.MoreInfo == null) { return adDetails; }
                SelectMoreInfo(adDetails);
                return adDetails;
            }, post, StepType.SelectMoreInfo);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectFulfillment} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (!adDetails.Fulfillments.Any()) return adDetails;
                SelectFulfillment(adDetails);
                return adDetails;
            }, post, StepType.SelectFulfillment);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectPayment} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (!adDetails.Payments.Any()) return adDetails;
                SelectPayment(adDetails);
                return adDetails;
            }, post, StepType.SelectPayment);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectTags} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (!adDetails.Tags.Any()) return adDetails;
                SelectTags(adDetails);
                return adDetails;
            }, post, StepType.SelectTags);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.TryInputPicture} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (!adDetails.InputPicturePaths.Any()) return adDetails;
                TryInputPicture(adDetails);
                return adDetails;
            }, post, StepType.TryInputPicture);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputLocation} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.Location == null) return adDetails;
                InputLocation(adDetails.Location);
                return adDetails;
            }, post, StepType.InputLocation);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputAddress} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.Address == null) return adDetails;
                InputAddress(adDetails);
                return adDetails;
            }, post, StepType.InputAddress);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputPrice} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.Price.HasValue)
                {
                    InputPrice(adDetails);
                }
                else
                {
                    InputPricePleaseContact();
                }
                return adDetails;
            }, post, StepType.InputPrice);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputCompany} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.Company == null) return adDetails;
                InputCompany(adDetails);
                return adDetails;
            }, post, StepType.InputCompany);


            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputPhone} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.PhoneNumber == string.Empty) return adDetails;
                InputPhone(adDetails);
                return adDetails;
            }, post, StepType.InputPhone);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputDynamicInputs} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (!adDetails.DynamicTextOptions.Any()) return adDetails;
                InputDynamicInputs(adDetails);
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation("InputDynamicInputs");
                InputDynamicSelectOptions(adDetails);
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation("InputDynamicSelectOptions");
                return adDetails;
            }, post, StepType.InputDynamicInputs);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputCarYear} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.CarYear == null) return adDetails;
                InputCarYear(adDetails);
                return adDetails;
            }, post, StepType.InputCarYear);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.InputCarKm} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (adDetails.CarKm == null) return adDetails;
                InputCarKm(adDetails);
                return adDetails;
            }, post, StepType.InputCarKm);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SelectBasicPakage} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                SelectBasicPakage();
                return adDetails;
            }, post, StepType.SelectBasicPakage);

            await KijijiActionHelper.ExecuteAndSaveResult(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.ActiveTermAndCondition} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                ActiveTermAndCondition();
                return adDetails;
            }, post, StepType.ActiveTermAndCondition);

            return await KijijiActionHelper.ExecuteAndSaveResult<bool>(() =>
            {
                Thread.Sleep(SleepIntervalBetweenEachAcion);
                Logger.LogInformation(
                    $"{StepType.SubmitPost} {JsonConvert.SerializeObject(post.AdDetailJson)}");

                if (TestMode)
                {
                    Logger.LogInformation("App is in testing node, so no ad will be actual posted");
                    return true;
                }

                Post();
                return CheckIfPostedSuccess();
            }, post, StepType.SubmitPost);
        }
        

        private void InputPricePleaseContact()
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var elements = WebDriver.FindElements(PricePleaseContactLocator);
            if (!elements.Any())
            {
                Logger.LogInformation("InputPricePleaseContact did not find any elements");
                return;
            }
            elements.First().Click();
        }


        private void ActiveTermAndCondition()
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var elements = WebDriver.FindElements(termAndConditions);
            if (elements.Any())
            {
                elements.First().Click();
            }
        }

        private void InputAdTitle(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var element = WebDriver.FindElements(AdTitleLocator);
            if (!element.Any())
            {
                Logger.LogInformation("Could not dound AdTitleLocator");
            }
            element.First().Clear();
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            element.First().SendKeys(adDetails.AdTitle);
        }

        private bool CheckIfPostedSuccess()
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            WebDriver.Navigate().Refresh();
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var postedText = WebDriver.FindElements(By.XPath("//*[text()='You have successfully posted your ad!']"));
            if (!postedText.Any())
            {
                throw new Exception("Sumited failed");
            }
            return true;
        }

        private void SelectBasicPakage()
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var elements = WebDriver.FindElements(selectBasicPackage);
            if (elements.Any())
            {
                elements.First().Click();
            }
        }

        private void InputDynamicInputs(AdDetails adDetails)
        {
            foreach (var option in adDetails.DynamicTextOptions)
            {
                var elements = WebDriver.FindElements(By.XPath($"//*[text()='{option}']"));
                if (elements.Any())
                {
                    elements.First().Click();
                }
            }
        }

        private void InputDynamicSelectOptions(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var selects = WebDriver.FindElements(By.TagName("select"));
            if (selects.Any())
            {
                foreach (var select in selects)
                {
                    try
                    {
                        var selectElement = new SelectElement(select);
                        foreach (var dynamicText in adDetails.DynamicTextOptions)
                        {
                            Logger.LogInformation($"Trying to input {select.GetAttribute("name")} - {dynamicText}");
                            foreach (IWebElement element in selectElement.Options)
                            {
                                if (element.Text.Equals(dynamicText))
                                {
                                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                                    element.Click();
                                    Logger.LogInformation($"Selected |{dynamicText}| on {select.GetAttribute("name")}");
                                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogInformation($"Skip {select.GetAttribute("name")}, {e.Message}, {e.StackTrace}");
                    }
                }
            }
            Thread.Sleep(SleepIntervalBetweenEachAcion);
        }

        private void InputPhone(AdDetails adDetails)
        {
            var element = WebWaiter
                .Until(SeleniumExtras
                    .WaitHelpers
                    .ExpectedConditions
                    .ElementIsVisible(phoneLocator));
            element.SendKeys(adDetails.PhoneNumber);
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
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var elements = WebDriver.FindElements(carYearLocator);
            if (elements.Any())
            {
                elements.First().SendKeys(adDetails.CarYear);
            }
        }

        private void InputCarKm(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var elements = WebDriver.FindElements(carKmLocator);
            if (!elements.Any())
            {
                Logger.LogInformation("Could not found any InputCarKm so skip");
                return;
            }
            elements.First().SendKeys(adDetails.CarKm);
        }

        private void InputAddress(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var addressElements = WebDriver.FindElements(addressLocator);
            if (!addressElements.Any())
            {
                Logger.LogInformation("Could not found any InputAddress so skip");
                return;
            }
            addressElements.First().Clear();
            addressElements.First().SendKeys(adDetails.Address);
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var addressElementFirst = WebWaiter
                           .Until(SeleniumExtras
                               .WaitHelpers
                               .ExpectedConditions
                               .ElementToBeClickable(addressLocatorFirst));
            addressElementFirst.Click();
        }

        private void SelectTags(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
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
                Thread.Sleep(SleepIntervalBetweenEachAcion);
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
                Thread.Sleep(SleepIntervalBetweenEachAcion);
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
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var adtype = WebWaiter
            .Until(SeleniumExtras
                .WaitHelpers
                .ExpectedConditions
                .ElementIsVisible(By.XPath($"//*[text()='{adDetails.AdType}']")));
            adtype.Click();
        }

        private void SelectForSaleBy(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var forSaleBy = WebWaiter
            .Until(SeleniumExtras
                .WaitHelpers
                .ExpectedConditions
                .ElementIsVisible(By.XPath($"//*[text()='{adDetails.ForSaleBy}']")));
            forSaleBy.Click();
        }

        private void SelectMoreInfo(AdDetails adDetails)
        {
            var moreInforExist = WebDriver.FindElements(By.Id("moreinfo_s"));
            if (moreInforExist.Any())
            {
                var select = WebWaiter
            .Until(SeleniumExtras
                .WaitHelpers
                .ExpectedConditions
                .ElementIsVisible(By.Id("moreinfo_s")));
                select.Click();

                Thread.Sleep(SleepIntervalBetweenEachAcion);
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
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var post = WebDriver.FindElements(PostButtonLocator);
            if (post.Any())
            {
                post.First().Click();
            }
        }

        private void InputPrice(AdDetails adDetails)
        {
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var price = WebWaiter
            .Until(SeleniumExtras
                .WaitHelpers
                .ExpectedConditions
                .ElementIsVisible(PriceLocator));
            price.SendKeys(adDetails.Price.ToString());
        }

        private void TryInputPicture(AdDetails adDetails)
        {
            var fileInputWrapper = WebDriver.FindElement(FileInputWrapper);
            var updatefile = fileInputWrapper.FindElement(By.TagName("input"));
            foreach (var path in adDetails.InputPicturePaths)
            {
                try
                {
                    updatefile.SendKeys(path);
                    Thread.Sleep(SleepIntervalBetweenEachAcion);
                }
                catch (Exception e)
                {
                    Logger.LogInformation($"Error upload images, file {path}");
                }
            }
            Thread.Sleep(SleepIntervalBetweenEachAcion);
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
            var changeButton = WebDriver.FindElements(ChangeLocationButtonLocator);
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
            Thread.Sleep(SleepIntervalBetweenEachAcion);
            var locationFirst = WebDriver.FindElements(LocationFirstLocator);
            if (locationFirst.Any())
            {
                locationFirst.First().Click();
            }
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
