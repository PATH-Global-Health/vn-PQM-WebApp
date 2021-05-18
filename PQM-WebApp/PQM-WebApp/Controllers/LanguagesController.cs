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
    /// CRUD Language
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="languageService"></param>
        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Get languages
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of languages with name and code (no dictionary)</returns>
        [HttpGet]
        public IActionResult Languages(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _languageService.Get(pageIndex, pageSize);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Get language detail (with dictionary)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Detail")]
        public IActionResult LanguageDetail(Guid id)
        {
            var rs = _languageService.Get(id);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Create a language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateLanguage(LanguageCreateModel model)
        {
            var rs = _languageService.CreateLanguage(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Update a language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpdateLanguage(LanguageViewModel model)
        {
            var rs = _languageService.UpdateLanguage(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }

        /// <summary>
        /// Delete a language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteLanguage(LanguageViewModel model)
        {
            var rs = _languageService.DeleteLanguage(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.ErrorMessage);
        }
    }
}
