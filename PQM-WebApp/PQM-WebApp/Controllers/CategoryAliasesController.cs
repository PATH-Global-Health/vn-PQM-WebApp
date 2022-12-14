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
    /// CRUD category alias
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAliasesController : ControllerBase
    {
        private readonly ICategoryAliasService _categoryAliasService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryAliasService"></param>
        public CategoryAliasesController(ICategoryAliasService categoryAliasService)
        {
            _categoryAliasService = categoryAliasService;
        }

        /// <summary>
        /// Get a list of category alias
        /// </summary>
        /// <param name="categoryId">Category ID. This parameter is optional</param>
        /// <param name="category">Category Name. This parameter is optional</param>
        /// <param name="pageIndex">Page Index. This parameter is optional</param>
        /// <param name="pageSize">Page Size. This parameter is optional</param>
        /// <returns></returns>
        /// <remarks>
        /// In case pageIndex = 0 and pageSize = MaxValue, the system will return all items
        /// </remarks>
        [HttpGet]
        public IActionResult Get(Guid categoryId = default(Guid), string category = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _categoryAliasService.Get(categoryId, category, pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Create a category alias
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(CategoryAliasCreateModel model)
        {
            var rs = _categoryAliasService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a category alias
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult Update(CategoryAliasViewModel model)
        {
            var rs = _categoryAliasService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a category alias
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(CategoryAliasViewModel model)
        {
            var rs = _categoryAliasService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}
