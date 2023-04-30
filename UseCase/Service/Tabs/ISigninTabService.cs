using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCase.Service.Tabs
{
    public interface ISigninTabService
    {
        Task Login(string email, string pass);
        Task LoadForAppStartup();
        Task Logout();
    }
}
