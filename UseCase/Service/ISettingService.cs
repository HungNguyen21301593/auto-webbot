using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace UseCase.Service
{
    public interface ISettingService
    {
        Task<Setting> GetSettingAsync();
        Task UpdateSettingAsync(Setting setting);
    }
}
