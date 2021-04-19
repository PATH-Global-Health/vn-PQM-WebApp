using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class GendersController : ControllerBase
    {
        private readonly IGenderService _GenderService;

        public GendersController(IGenderService GenderService)
        {
            _GenderService = GenderService;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _GenderService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Create(GenderCreateModel model)
        {
            var rs = _GenderService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Update(GenderViewModel model)
        {
            var rs = _GenderService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(GenderViewModel model)
        {
            var rs = _GenderService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
