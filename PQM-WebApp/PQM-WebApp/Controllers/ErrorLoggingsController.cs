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
    public class ErrorLoggingsController : ControllerBase
    {
        private readonly IErrorLoggingService _errorLoggingService;

        public ErrorLoggingsController(IErrorLoggingService errorLoggingService)
        {
            _errorLoggingService = errorLoggingService;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10, DateTime? from = null, DateTime? to = null, string code = null)
        {
            var rs = _errorLoggingService.Get(pageIndex, pageSize, from, to, code);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Error);
        }
    }
}