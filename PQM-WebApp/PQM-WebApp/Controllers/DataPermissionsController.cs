using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PQM_WebApp.Controllers
{
    [Route("api/[controller]")]
    public class DataPermissionsController : Controller
    {
        private readonly IDataPermissionService _dataPermissionService;

        public DataPermissionsController(IDataPermissionService dataPermissionService)
        {
            _dataPermissionService = dataPermissionService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]DataPermissionCreateModel dataPermission)
        {
            var rs = _dataPermissionService.Create(dataPermission);
            if (rs.Succeed)
            {
                return Ok(rs.Data);
            }
            return BadRequest(rs.Error);
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = int.MaxValue, string username = null)
        {
            var rs = _dataPermissionService.Get(pageIndex, pageSize, username);
            if (rs.Succeed)
            {
                return Ok(rs);
            }
            return BadRequest(rs.Error);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var rs = _dataPermissionService.Delete(id);
            if (rs.Succeed)
            {
                return Ok(rs);
            }
            return BadRequest(rs.Error);
        }
    }
}
