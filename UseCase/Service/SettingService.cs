using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Entities;
using Infastructure.Repositories;

namespace UseCase.Service
{
    public class SettingService : ISettingService
    {
        private readonly IKijijiPostingService kijijiPostingService;
        public ISettingRepository SettingRepository { get; }

        public SettingService(ISettingRepository settingRepository, IKijijiPostingService kijijiPostingService)
        {
            this.kijijiPostingService = kijijiPostingService ?? throw new ArgumentNullException(nameof(kijijiPostingService));
            SettingRepository = settingRepository;
        }
        public async Task<Setting> GetSettingAsync()
        {
            return await SettingRepository.Read();
        }

        public async Task UpdateSettingAsync(Setting setting)
        {
            await SettingRepository.Update(setting);
        }
    }
}
