using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        public DataContext DataContext { get; }

        public SettingRepository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public async Task<Setting> Create(Setting setting)
        {
            DataContext.Settings.Add(setting);
            await DataContext.SaveChangesAsync();
            return setting;
        }

        public async Task<Setting> Read()
        {
            var setting = await DataContext.Settings.FirstOrDefaultAsync();
            return setting;
        }

        public async Task<Setting> Update(Setting setting)
        {
            var existingSetting = await DataContext.Settings.SingleAsync();
            if (setting == null)
            {
                throw new Exception("Could not found setting");
            }

            existingSetting.PageToTrigger = setting.PageToTrigger;
            existingSetting.KijijiEmail = setting.KijijiEmail;
            existingSetting.KijijiPassword = setting.KijijiPassword;
            existingSetting.RegistrationId = setting.RegistrationId;
            await DataContext.SaveChangesAsync();
            return existingSetting;
        }
    }
}
