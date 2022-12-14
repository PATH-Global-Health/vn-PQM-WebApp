using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
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

        [HttpGet("Variables")]
        public IActionResult Variables(int year, int quarter, int? month
            , string indicatorGroup, string indicator, string groupBy
            , string provinceCode, string districtCode)
        {
            var rs = _aggregatedService.GetAggregatedValues(year, quarter, month, indicatorGroup, indicator, groupBy, provinceCode, districtCode);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error);
        }

        [HttpGet("")]
        public IActionResult Get(int? pageIndex = 0, int? pageSize = int.MaxValue
            , string period = null, int? year = null, int? quarter = null, int? month = null
            , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
            , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null)
        {
            var rs = _aggregatedService.Get(pageIndex, pageSize
                , period, year, quarter, month
                , indicatorId, ageGroupId, genderId, keyPopulationId
                , provinceId, districId, siteId, indicatorGroupId);
            if (rs.Succeed) return Ok(rs);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPost("/api/AggregatedValues/Create")]
        public IActionResult Create(IndicatorImportModel aggregatedValue)
        {
            var rs = _aggregatedService.Create(aggregatedValue);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPut("/api/AggregatedValues/Update")]
        public IActionResult Update(AggregatedValueUpdateModel aggregatedValue)
        {
            var rs = _aggregatedService.Update(aggregatedValue);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpDelete("/api/AggregatedValues/Delete")]
        public IActionResult Delete(Guid id)
        {
            var rs = _aggregatedService.Delete(id);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPost("/api/AggregatedValues/ImportByExcel")]
        [Consumes("multipart/form-data")]
        public IActionResult ImportByExcel([FromForm] IFormFile file)
        {
            var username = User.Claims.FirstOrDefault(s => s.Type == "Username").Value;
            var rs = _aggregatedService.ImportExcel(file, username);
            return Ok(rs);
        }

        [HttpPost("/api/AggregatedValues/Import")]
        public IActionResult ImportAggregateData([FromBody] List<IndicatorImportModel> aggregateData)
        {
            var username = User.Claims.FirstOrDefault(s => s.Type == "Username").Value;
            var rs = _aggregatedService.ImportIndicator(aggregateData, username: username);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPost("/api/AggregatedValues/ImportV2")]
        public IActionResult ImportAggregateDataV2([FromBody] AggregatedData aggregateData)
        {
            var rs = _aggregatedService.ImportIndicator(aggregateData, "admin");
            if (rs.Succeed)
            {
                return Ok(rs);
            }
            return BadRequest(rs);
        }

        [HttpPost("/api/AggregatedValues/PopulateData")]
        public IActionResult PopulateData(string indicator, int year, int month, int? day = null, bool all = false, bool makeDeletion = false)
        {
            return Ok(_aggregatedService.PopulateData(indicator, year, month, day, all, makeDeletion));
        }

        [HttpGet("/api/AggregatedValues/ChartData")]
        public IActionResult GetChartData(string indicator, int year, int quarter, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var rs = _aggregatedService.GetChartData(indicator, year, quarter, provinceCode, districtCode, month, ageGroups, keyPopulations, genders, clinnics);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet("/api/AggregatedValues/IndicatorValues")]
        public IActionResult GetIndicatorValues(string provinceCode, string districtCode
                                              , string indicatorGroup, string indicatorCode
                                              , int year, int? quarter = null, int? month = null
                                              , string ageGroups = "", string genders = "", string keyPopulations = "", string sites = "")
        {
            var rs = _aggregatedService.GetIndicatorValues(provinceCode, districtCode, indicatorGroup, indicatorCode, year, quarter, month, ageGroups, keyPopulations, genders, sites);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet("/api/AggregatedValues/ClearAll")]
        public IActionResult ClearAll()
        {
            _aggregatedService.ClearAll();
            return Ok();
        }

        [HttpPost("/api/AggregatedValues/Recall")]
        public IActionResult Recall([FromBody] RecallModel model)
        {
            return Ok(_aggregatedService.Recall(model));
        }

        [HttpGet("/api/AggregatedValues/CheckVersion")]
        public IActionResult CheckVersion()
        {
            return Ok(new { v = "1.0.0" });
        }
    }
}