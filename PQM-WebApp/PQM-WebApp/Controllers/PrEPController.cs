using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    public class PrEPController : Controller
    {
        private readonly IPrEPService _prEPService;

        public PrEPController(IPrEPService prEPService)
        {
            _prEPService = prEPService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Indicator(string name)
        {
            var rs = _prEPService.GetIndicator(name);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.ErrorMessage);
        }
    }
}