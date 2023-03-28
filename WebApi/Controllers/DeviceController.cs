using Core.Model;
using FireBaseAuthenticator.KijijiHelperServices;
using FireBaseAuthenticator.Model;
using Microsoft.AspNetCore.Mvc;
using UseCase.Service;
using WebApi.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceInfoChart deviceInfoChart;

        public IDeviceRegistrationService AuthenticatorService { get; }

        public DeviceController(IDeviceRegistrationService authenticatorService, IDeviceInfoChart deviceInfoChart)
        {
            AuthenticatorService = authenticatorService ?? throw new ArgumentNullException(nameof(authenticatorService));
            this.deviceInfoChart = deviceInfoChart;
        }

        [HttpGet]
        [Route("get")]
        public async Task<V3> Get()
        {
            return await AuthenticatorService.GetDeviceInformation();
        }

        [HttpPost]
        [Route("verify")]
        public async Task<DeviceStatus> Verify()
        {
            var deviceInfo = await AuthenticatorService.GetDeviceInformation();
            await deviceInfoChart.Save(deviceInfo);
            return new DeviceStatus(deviceInfo);
        }

        [HttpGet]
        [Route("chart")]
        public async Task<List<ChartItemViewModel>> GetChartData(DateTime fromDate, DateTime toDate)
        {
            var infos = await deviceInfoChart.ReadAll(fromDate, toDate);
            return infos
                .Select(x => new ChartItemViewModel
                {
                    Value = x.RemainingPostLimit,
                    Date = new DateTime(x.Created.Year, x.Created.Month, x.Created.Day, x.Created.Hour, x.Created.Minute, 0)
                }).DistinctBy(x => $"{x.Value}|{x.Date}")
                .ToList();
        }
    }
}
