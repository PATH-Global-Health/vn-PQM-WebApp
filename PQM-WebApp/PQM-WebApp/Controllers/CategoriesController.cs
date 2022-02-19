using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categorySerivce;

        public CategoriesController(ICategoryService categorySerivce)
        {
            _categorySerivce = categorySerivce;
        }

        public IActionResult Get(string category, string province_code = null, string district_code = null, DateTime? from = null, DateTime? to = null)
            => Ok(_categorySerivce.Get(category, province_code, district_code, from, to).Data);
    }
}