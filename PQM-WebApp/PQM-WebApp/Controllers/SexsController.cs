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
    public class SexsController : ControllerBase
    {
        private readonly ISexService _sexService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sexService"></param>
        public SexsController(ISexService sexService)
        {
            _sexService = sexService;
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
            var rs = _sexService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Create a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(SexCreateModel model)
        {
            var rs = _sexService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Update a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(SexViewModel model)
        {
            var rs = _sexService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Delete a sex
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(SexViewModel model)
        {
            var rs = _sexService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
