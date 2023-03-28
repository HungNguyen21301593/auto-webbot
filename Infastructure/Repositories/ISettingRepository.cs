using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;

namespace Infastructure.Repositories
{
    public interface ISettingRepository
    {
        Task<Setting> Create(Setting setting);

        Task<Setting> Read();

        Task<Setting> Update(Setting coreRequest);
    }
}
