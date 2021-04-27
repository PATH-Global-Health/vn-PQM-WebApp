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

        /// <summary>
        /// Get list of age groups
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns></returns>
        /// <remarks>
        /// In case pageIndex = 0 and pageSize = MaxValue, the system will return all items
        /// </remarks>
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _ageGroupService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Create an AgeGroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(AgeGroupCreateModel model)
        {
            var rs = _ageGroupService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Update an AgeGroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(AgeGroupViewModel model)
        {
            var rs = _ageGroupService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Delete an AgeGroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
