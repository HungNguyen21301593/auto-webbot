using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories
{
    public class DeviceRegistrationRepository : IDeviceRegistrationRepository
    {
        public DataContext DataContext { get; }

        public DeviceRegistrationRepository(DataContext dataContext)
        {
            DataContext = dataContext;

        }

        public async Task<Request> Create()
        {
            var newRequest = new Request
            {
                Id = Guid.NewGuid(),
            };
            DataContext.Requests.Add(newRequest);
            await DataContext.SaveChangesAsync();
            return newRequest;
        }

        public async Task<DeviceRegistration> Create(DeviceRegistration? deviceRegistration)
        {
            var newDeviceRegistration = deviceRegistration ?? new DeviceRegistration
            {
                Id = Guid.NewGuid(),
            };
            DataContext.DeviceRegistrations.Add(newDeviceRegistration);
            await DataContext.SaveChangesAsync();
            return newDeviceRegistration;
        }

        public async Task<List<DeviceRegistration>> ReadAll(DateTime fromDate, DateTime toDate)
        {
            var deviceInfors = await DataContext.DeviceRegistrations
                .Where(di => fromDate <= di.Created && di.Created <= toDate)
                .OrderBy(di => di.Created)
                .ToListAsync();
            return deviceInfors;
        }
    }
}
