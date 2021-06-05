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
    /// <summary>
    /// CRUD indicator group
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorGroupsController : ControllerBase
    {
        private readonly IIndicatorGroupService _indicatorGroupService;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="indicatorGroupService"></param>
        public IndicatorGroupsController(IIndicatorGroupService indicatorGroupService)
        {
            _indicatorGroupService = indicatorGroupService;
        }

        /// <summary>
        /// Get indicator groups
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
            var rs = _indicatorGroupService.Get(pageIndex, pageSize);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Create an indicator group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody]IndicatorGroupCreateModel model)
        {
            var rs = _indicatorGroupService.Create(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update an indicator group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(IndicatorGroupViewModel model)
        {
            var rs = _indicatorGroupService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete an indicator group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(IndicatorGroupViewModel model)
        {
            var rs = _indicatorGroupService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}