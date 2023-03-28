using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using FireBaseAuthenticator.Model;
using Infastructure.Repositories;

namespace UseCase.Service
{
    public class DeviceInfoChart : IDeviceInfoChart
    {
        private readonly IDeviceRegistrationRepository deviceRegistrationRepository;

        public DeviceInfoChart(IDeviceRegistrationRepository deviceRegistrationRepository)
        {
            this.deviceRegistrationRepository = deviceRegistrationRepository;
        }

        public async Task Save(V3 deviceinfo)
        {
            var deviceRegistration = new DeviceRegistration
            {
                Id = Guid.NewGuid(),
                UserName = deviceinfo.UserName,
                RemainingPostLimit = deviceinfo.RemainingPostLimit,
                ExpiredDate = deviceinfo.ExpiredDate,
                Created = DateTime.UtcNow
            };
            await deviceRegistrationRepository.Create(deviceRegistration);
        }

        public async Task<List<DeviceRegistration>> ReadAll(DateTime fromDate, DateTime toDate)
        {
            return await deviceRegistrationRepository.ReadAll(fromDate, toDate);
        }
    }
}
