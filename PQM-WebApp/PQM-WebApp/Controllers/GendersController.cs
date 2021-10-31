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
    /// CRUD sex
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly IGenderService _GenderService;

        public GendersController(IGenderService GenderService)
        {
            _GenderService = GenderService;
        }

        /// <summary>
        /// Get sexes
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _GenderService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Create a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(GenderCreateModel model)
        {
            var rs = _GenderService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(GenderViewModel model)
        {
            var rs = _GenderService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(GenderViewModel model)
        {
            var rs = _GenderService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}
