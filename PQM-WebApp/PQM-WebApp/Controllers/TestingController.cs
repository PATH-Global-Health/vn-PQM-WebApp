using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Service;

namespace PQM_WebApp.Controllers
{
    public class TestingController : Controller
    {
        private readonly ITestingService _testingService;

        public TestingController(ITestingService testingService)
        {
            _testingService = testingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Indicators()
        //{
        //    var rs = _testingService.GetIndicators();
        //    if (rs.Succeed)
        //    {
        //        return Ok(rs.Data);
        //    }
        //    return BadRequest(rs.ErrorMessage);
        //}
    }
}