using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        private readonly IUtilsService _utilsService;

        public UtilsController(IUtilsService utilsService)
        {
            _utilsService = utilsService;
        }

        [HttpPost("Province")]
        public IActionResult Create([FromBody] IEnumerable<ProvinceModel> models)
        {
            var rs = _utilsService.ImportProvince(models);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPost("District")]
        public IActionResult Create([FromBody] IEnumerable<DistrictModel> models)
        {
            var rs = _utilsService.ImportDistrict(models);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPut]
        public IActionResult FixVLUnsupressed()
        {
            return Ok(_utilsService.FixVLUnsupressed());
        }
    }
}
