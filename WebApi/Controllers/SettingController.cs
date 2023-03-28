using Entities;
using Microsoft.AspNetCore.Mvc;
using UseCase.Service;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        public ISettingService SettingService { get; }

        public SettingController(ISettingService settingService)
        {
            SettingService = settingService;
        }

        [HttpGet("{id}")]
        public async Task<Setting> Get(int id)
        {
            return await SettingService.GetSettingAsync();
        }

        [HttpPut("{id}")]
        public async Task Put(int id, Setting setting)
        {
            await SettingService.UpdateSettingAsync(setting);
        }
    }
}
