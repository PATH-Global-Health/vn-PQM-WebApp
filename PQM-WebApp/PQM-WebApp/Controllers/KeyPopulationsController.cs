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
    /// CRUD key populations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KeyPopulationsController : ControllerBase
    {
        private readonly IKeyPopulationService _keyPopulationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyPopulationService"></param>
        public KeyPopulationsController(IKeyPopulationService keyPopulationService)
        {
            _keyPopulationService = keyPopulationService;
        }

        /// <summary>
        /// Get key populations
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _keyPopulationService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Create a key population
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(KeyPopulationCreateModel model)
        {
            var rs = _keyPopulationService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a key population
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(KeyPopulationViewModel model)
        {
            var rs = _keyPopulationService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a key population
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(KeyPopulationViewModel model)
        {
            var rs = _keyPopulationService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}
