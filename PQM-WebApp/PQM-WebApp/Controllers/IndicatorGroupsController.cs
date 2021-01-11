using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorGroupsController : ControllerBase
    {
        private readonly IIndicatorGroupService _indicatorGroupService;

        public IndicatorGroupsController(IIndicatorGroupService indicatorGroupService)
        {
            _indicatorGroupService = indicatorGroupService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var rs = _indicatorGroupService.Get();
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Create([FromBody]IndicatorGroupCreateModel model)
        {
            var rs = _indicatorGroupService.Create(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}