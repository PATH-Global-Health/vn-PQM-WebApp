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
    public class AgeGroupsController : ControllerBase
    {
        private readonly IAgeGroupService _ageGroupService;
        public AgeGroupsController(IAgeGroupService ageGroupService)
        {
            _ageGroupService = ageGroupService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var rs = _ageGroupService.Get();
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Create(AgeGroupCreateModel model)
        {
            var rs = _ageGroupService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Update(AgeGroupViewModel model)
        {
            var rs = _ageGroupService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(AgeGroupViewModel model)
        {
            var rs = _ageGroupService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
