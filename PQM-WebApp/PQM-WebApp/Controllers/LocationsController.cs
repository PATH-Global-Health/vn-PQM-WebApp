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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("Provinces")]
        public IActionResult Provinces(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _locationService.GetProvinces(pageIndex, pageSize);
            return Ok(rs.Data);
        }

        [HttpPost("Provinces")]
        public IActionResult CreateProvince(ProvinceCreateModel model)
        {
            var rs = _locationService.CreateProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPut("Provinces")]
        public IActionResult UpdateProvince(ProvinceModel model)
        {
            var rs = _locationService.UpdateProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpDelete("Provinces")]
        public IActionResult DeleteProvince(ProvinceModel model)
        {
            var rs = _locationService.DeleteProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet("Districts")]
        public IActionResult Districts(string provinceCode)
        {
            var rs = _locationService.GetDistricts(provinceCode);
            return Ok(rs.Data);
        }

        [HttpPost("Districts")]
        public IActionResult CreateDistrict(DistrictCreateModel model)
        {
            var rs = _locationService.CreateDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPut("Districts")]
        public IActionResult UpdateDistrict(DistrictModel model)
        {
            var rs = _locationService.UpdateDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpDelete("Districts")]
        public IActionResult DeleteDistrict(DistrictModel model)
        {
            var rs = _locationService.DeleteDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet("Sites")]
        public IActionResult Sites(Guid districtId)
        {
            var rs = _locationService.GetSites(districtId);
            return Ok(rs.Data);
        }

        [HttpPost("Sites")]
        public IActionResult CreateSite(SiteCreateModel model)
        {
            var rs = _locationService.CreateSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPut("Sites")]
        public IActionResult UpdateSite(SiteViewModel model)
        {
            var rs = _locationService.UpdateSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpDelete("Sites")]
        public IActionResult DeleteSite(SiteViewModel model)
        {
            var rs = _locationService.DeleteSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}
