using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelsController : ControllerBase
    {
        private readonly IExcelService _excelService;

        public ExcelsController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        public IActionResult ExportExamBook(int? pageIndex = 0, int? pageSize = int.MaxValue
            , string period = null, int? year = null, int? quarter = null, int? month = null
            , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
            , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null)
        {
            var result = _excelService.GetAggregatedData(pageIndex, pageSize
                , period, year, quarter, month
                , indicatorId, ageGroupId, genderId, keyPopulationId
                , provinceId, districId, siteId, indicatorGroupId);
            if (result.Succeed)
            {
                var fileBytes = result.Data as byte[];
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"data.xlsx");
            }
            return BadRequest(result.Error.ErrorMessage);
        }
    }
}