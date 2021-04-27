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
    /// <summary>
    /// CRUD site type
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SiteTypesController : ControllerBase
    {
        private readonly ISiteTypeService _siteTypeService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="siteTypeService"></param>
        public SiteTypesController(ISiteTypeService siteTypeService)
        {
            _siteTypeService = siteTypeService;
        }

        /// <summary>
        /// Get site types
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _siteTypeService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Create a site type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(SiteTypeCreateModel model)
        {
            var rs = _siteTypeService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Update a site type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(SiteTypeViewModel model)
        {
            var rs = _siteTypeService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Delete a site type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(SiteTypeViewModel model)
        {
            var rs = _siteTypeService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
