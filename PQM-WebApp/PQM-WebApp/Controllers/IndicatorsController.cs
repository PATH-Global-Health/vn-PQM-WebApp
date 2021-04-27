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

        /// <summary>
        /// Get indicators
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        /// <remarks>
        /// In case pageIndex = 0 and pageSize = MaxValue, the system will return all items
        /// </remarks>
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _indicatorService.Get(pageIndex, pageSize);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Create an indicator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] IndicatorCreateModel model)
        {
            var rs = _indicatorService.Create(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Update an indicator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(IndicatorViewModel model)
        {
            var rs = _indicatorService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Delete an indicator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(IndicatorViewModel model)
        {
            var rs = _indicatorService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
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

        [HttpPut("ImportByExcel")]
        [Consumes("multipart/form-data")]
        public IActionResult ImportByExcel([FromForm] IFormFile file, [FromQuery] string type)
        {
            _indicatorService.ImportExcel(file, type);
            return Ok();
        }

        [HttpPost("/api/AggregateData")]
        public IActionResult ImportAggregateData([FromBody] List<IndicatorImportModel> aggregateData)
        {
            var rs = _indicatorService.ImportIndicator(aggregateData);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
    }
}