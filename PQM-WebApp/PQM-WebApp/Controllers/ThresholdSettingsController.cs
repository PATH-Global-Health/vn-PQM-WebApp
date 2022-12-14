using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThresholdSettingsController : ControllerBase
    {
        private readonly IThresholdSettingService _thresholdSettingService;

        public ThresholdSettingsController(IThresholdSettingService thresholdSettingService)
        {
            _thresholdSettingService = thresholdSettingService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] ThresholdSettingCreateModel thresholdSetting) {
            thresholdSetting.Username = User.Claims.FirstOrDefault(s => s.Type == "Username").Value;
            var rs = _thresholdSettingService.Create(thresholdSetting);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet]
        public IActionResult Get(int? pageIndex = 0, int? pageSize = 20)
        {
            var rs = _thresholdSettingService.Get(pageIndex, pageSize);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}