using Entities;

namespace Infastructure.Repositories
{
    public interface IDeviceRegistrationRepository
    {
        Task<DeviceRegistration> Create(DeviceRegistration? deviceRegistration);
        Task<List<DeviceRegistration>> ReadAll(DateTime fromDate, DateTime toDate);
    }
}
