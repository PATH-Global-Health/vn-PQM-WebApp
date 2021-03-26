﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregatedValuesController : ControllerBase
    {
        private readonly IAggregatedValueService _aggregatedService;

        public AggregatedValuesController(IAggregatedValueService aggregatedService)
        {
            _aggregatedService = aggregatedService;
        }

        [HttpGet()]
        public IActionResult Provinces(int year, int quarter, int? month, string indicatorGroup, string indicator, string groupBy, string provinceCode, string districtCode)
        {
            var rs = _aggregatedService.GetAggregatedValues(year, quarter, month, indicatorGroup, indicator, groupBy, provinceCode, districtCode);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult PopulateData(string indicator, int year, int month, int? day = null, bool all = false)
        {
            return Ok(_aggregatedService.PopulateData(indicator, year, month, day, all));
        }

        [HttpGet("ChartData")]
        public IActionResult GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _aggregatedService.GetChartData(indicator, year, quater, provinceCode, districtCode, month, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
        [HttpGet("IndicatorValues")]
        public IActionResult GetIndicatorValues(string provinceCode, string districtCode, string indicatorGroup, string indicatorCode, int year, int? quarter = null, int? month = null)
        {
            var rs = _aggregatedService.GetIndicatorValues(provinceCode, districtCode, indicatorGroup, indicatorCode, year, quarter, month);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
    }
}
