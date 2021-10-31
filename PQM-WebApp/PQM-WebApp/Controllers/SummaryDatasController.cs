using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.ViewModels;

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryDatasController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create([FromBody]ISVCreateModel model)
        {
            return Ok();
        }
    }
}