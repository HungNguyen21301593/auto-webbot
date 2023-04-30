using Core.Model;
using Entities;
using FireBaseAuthenticator.KijijiHelperServices;
using FireBaseAuthenticator.Model;
using Infastructure.Repositories;

namespace UseCase.Service
{
    public class DeviceInfoChart : IDeviceInfoChart
    {
        private readonly IDeviceRegistrationRepository deviceRegistrationRepository;
        private readonly IDeviceRegistrationService deviceRegistrationService;

        public DeviceInfoChart(IDeviceRegistrationRepository deviceRegistrationRepository, IDeviceRegistrationService deviceRegistrationService)
        {
            this.deviceRegistrationRepository = deviceRegistrationRepository;
            this.deviceRegistrationService = deviceRegistrationService;
        }

        public async Task<DeviceStatus> Verify(bool doCheckFireBase = false)
        {
            var allRecentInfos = await deviceRegistrationRepository.ReadAll(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            var latestDeviceInfo = allRecentInfos.MaxBy(d => d.Created);
            if (!doCheckFireBase && latestDeviceInfo is not null)
            {
                return new DeviceStatus(latestDeviceInfo);
            }
            var deviceInfo = await deviceRegistrationService.GetDeviceInformation();
            await Save(deviceInfo);
            return new DeviceStatus(deviceInfo);
        }

        public async Task<DeviceStatus> UpdateRemainingPostAndSaveDeviceInfo()
        {
            var deviceInfo = await deviceRegistrationService.UpdateNumberOfAllowAds();
            await Save(deviceInfo);
            return new DeviceStatus(deviceInfo);
        }

        private async Task Save(V3 deviceinfo)
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
