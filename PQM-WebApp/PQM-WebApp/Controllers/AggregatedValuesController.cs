using Microsoft.AspNetCore.Http;
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
        public IActionResult Provinces(int year, int month, string indicatorCode, string groupBy)
        {
            var rs = _aggregatedService.GetAggregatedValues(year, month, indicatorCode, groupBy);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
