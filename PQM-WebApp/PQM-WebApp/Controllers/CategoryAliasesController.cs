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
    public class CategoryAliasesController : ControllerBase
    {
        private readonly ICategoryAliasService _categoryAliasService;

        public CategoryAliasesController(ICategoryAliasService categoryAliasService)
        {
            _categoryAliasService = categoryAliasService;
        }

        [HttpGet]
        public IActionResult Get(string name = null, string category = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _categoryAliasService.Get(name, category, pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Create(CategoryAliasCreateModel model)
        {
            var rs = _categoryAliasService.Create(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Update(CategoryAliasViewModel model)
        {
            var rs = _categoryAliasService.Update(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(CategoryAliasViewModel model)
        {
            var rs = _categoryAliasService.Delete(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
