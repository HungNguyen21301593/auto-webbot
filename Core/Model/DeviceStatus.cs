using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public V3 DeviceInfo { get; set; }
        public bool Status => DeviceInfo.IsVerified();
        public bool IsNotExpired => DeviceInfo.IsNotExpired();
        public bool IsVipDevice => DeviceInfo.IsVipDevice();
        public bool IsRePostLimitIsValid => DeviceInfo.IsRePostLimitIsValid();

    }
}
