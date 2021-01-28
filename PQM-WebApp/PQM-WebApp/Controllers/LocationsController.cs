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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("Provinces")]
        public IActionResult Provinces(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _locationService.GetProvinces(pageIndex, pageSize);
            return Ok(rs);
        }

        [HttpGet("Districts")]
        public IActionResult Districts(string provinceCode)
        {
            var rs = _locationService.GetDistricts(provinceCode);
            return Ok(rs);
        }
    }
}
