using Core.Model;
using Entities;
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

        public IDeviceRegistrationService DeviceRegistrationService { get; }

        public DeviceController(IDeviceRegistrationService deviceRegistrationService, IDeviceInfoChart deviceInfoChart)
        {
            DeviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            this.deviceInfoChart = deviceInfoChart;
        }

        [HttpGet]
        [Route("get")]
        public async Task<V3> Get()
        {
            return await DeviceRegistrationService.GetDeviceInformation();
        }

        [HttpPost]
        [Route("verify")]
        public async Task<DeviceStatus> Verify(bool doCheckFireBase)
        {
            return await deviceInfoChart.Verify(doCheckFireBase);
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
