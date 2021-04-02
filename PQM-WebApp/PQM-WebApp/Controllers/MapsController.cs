using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    public class MapsController : Controller
    {
        private readonly ITestingService _testingService;

        public MapsController(ITestingService testingService)
        {
            _testingService = testingService;
        }

        [HttpGet]
        public IActionResult Get(int year, int quarter, int? month, string provinceCode, string districtCode)
        {
            return Ok(_testingService.GetHealthMap(year, quarter, month, provinceCode, districtCode));
        }
    }
}
