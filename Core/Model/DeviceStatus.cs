using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using FireBaseAuthenticator.Extensions;
using FireBaseAuthenticator.Model;

namespace Core.Model
{
    public class DeviceStatus
    {
        public DeviceStatus(V3 deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }
        public DeviceStatus(DeviceRegistration deviceInfo)
        {
            DeviceInfo = new V3
            {
                ExpiredDate = deviceInfo.ExpiredDate,
                RemainingPostLimit = deviceInfo.RemainingPostLimit,
                UserName = deviceInfo.UserName,
            };
        }
        public V3 DeviceInfo { get; set; }
        public bool IsVerified => DeviceInfo.IsVerified();
        public bool IsNotExpired => DeviceInfo.IsNotExpired();
        public bool IsVipDevice => DeviceInfo.IsVipDevice();
        public bool IsRePostLimitIsValid => DeviceInfo.IsRePostLimitIsValid();

    }
}
