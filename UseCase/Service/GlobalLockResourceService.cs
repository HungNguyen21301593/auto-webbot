using Core.Model;
using Entities;
using Infastructure.Repositories;
using Newtonsoft.Json;
using UseCase.Service.Tabs;

namespace UseCase.Service
{
    public class GlobalLockResourceService
    {
        public static object WebDriverLockResource = new();
    }
}
