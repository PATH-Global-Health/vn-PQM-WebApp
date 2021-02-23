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
        private readonly ITestingService _testingService;
        private readonly ITreatmentService _treatmentService;

        public IndicatorsController(IIndicatorService indicatorService, IPrEPService prEPService, ITestingService testingService, ITreatmentService treatmentService)
        {
            _indicatorService = indicatorService;
            _prEPService = prEPService;
            _testingService = testingService;
            _treatmentService = treatmentService;
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
        public IActionResult PrEPIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _prEPService.GetIndicators(year, quater, month, provinceCode, districtCode, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }

        [HttpGet("/api/Testing/indicators")]
        public IActionResult TestingIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _testingService.GetIndicators(year, quater, month, provinceCode, districtCode, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }

        [HttpGet("/api/Treatment/indicators")]
        public IActionResult TreatmentIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _treatmentService.GetIndicators(year, quater, month, provinceCode, districtCode, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
    }
}