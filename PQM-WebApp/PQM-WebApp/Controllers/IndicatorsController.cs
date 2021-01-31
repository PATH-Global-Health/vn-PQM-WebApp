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
    public class IndicatorsController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;
        private readonly IPrEPService _prEPService;

        public IndicatorsController(IIndicatorService indicatorService, IPrEPService prEPService)
        {
            _indicatorService = indicatorService;
            _prEPService = prEPService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var rs = _indicatorService.Get();
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Create([FromBody]IndicatorCreateModel model)
        {
            var rs = _indicatorService.Create(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpGet("/api/PrEP/indicators")]
        public IActionResult Indicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _prEPService.GetIndicators(year, quater, month, provinceCode, districtCode, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
    }
}