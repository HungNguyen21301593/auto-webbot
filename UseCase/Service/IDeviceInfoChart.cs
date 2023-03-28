using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using FireBaseAuthenticator.Model;

namespace UseCase.Service
{
    public interface IDeviceInfoChart
    {
        Task<List<DeviceRegistration>> ReadAll(DateTime fromDate, DateTime toDate);
        Task Save(V3 deviceInfo);
    }
}
