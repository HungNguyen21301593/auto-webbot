using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase.Service
{
    public interface IBrowserManagerService
    {
        IWebDriver GetDriver();
        WebDriverWait GetDriverWaiter();
        void OpenNewTabAndSetName(string tabName);
        string GetTabByName(string tabName);
        void Restart();
    }
}
