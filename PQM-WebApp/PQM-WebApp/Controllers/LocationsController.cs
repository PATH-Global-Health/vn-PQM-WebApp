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
    /// CRUD province, district and site
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="locationService"></param>
        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Get provinces
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        [HttpGet("Provinces")]
        public IActionResult Provinces(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var rs = _locationService.GetProvinces(pageIndex, pageSize);
            return Ok(rs.Data);
        }

        /// <summary>
        /// Create a province
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Provinces")]
        public IActionResult CreateProvince(ProvinceCreateModel model)
        {
            var rs = _locationService.CreateProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a province
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Provinces")]
        public IActionResult UpdateProvince(ProvinceModel model)
        {
            var rs = _locationService.UpdateProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a province
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Provinces")]
        public IActionResult DeleteProvince(ProvinceModel model)
        {
            var rs = _locationService.DeleteProvince(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Get districts of a province
        /// </summary>
        /// <param name="provinceCode">Province Code</param>
        /// <returns></returns>
        [HttpGet("Districts")]
        public IActionResult Districts(string provinceCode)
        {
            var rs = _locationService.GetDistricts(provinceCode);
            return Ok(rs.Data);
        }

        /// <summary>
        /// Create a district
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Districts")]
        public IActionResult CreateDistrict(DistrictCreateModel model)
        {
            var rs = _locationService.CreateDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a district
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Districts")]
        public IActionResult UpdateDistrict(DistrictModel model)
        {
            var rs = _locationService.UpdateDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a district
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Districts")]
        public IActionResult DeleteDistrict(DistrictModel model)
        {
            var rs = _locationService.DeleteDistrict(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Get sites
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [HttpGet("Sites")]
        public IActionResult Sites(Guid districtId)
        {
            var rs = _locationService.GetSites(districtId);
            return Ok(rs.Data);
        }

        /// <summary>
        /// Create a site
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Sites")]
        public IActionResult CreateSite(SiteCreateModel model)
        {
            var rs = _locationService.CreateSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Update a site
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Sites")]
        public IActionResult UpdateSite(SiteViewModel model)
        {
            var rs = _locationService.UpdateSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        /// <summary>
        /// Delete a site
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("Sites")]
        public IActionResult DeleteSite(SiteViewModel model)
        {
            var rs = _locationService.DeleteSite(model);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpGet("Sites/ByCode")]
        public IActionResult GetSitesByCode(int pageIndex = 0, int pageSize = 10, string provinceCode = null, string districtCode = null, Guid? siteTypeId = null)
        {
            var rs = _locationService.GetSites(pageIndex, pageSize, provinceCode, districtCode, siteTypeId);
            if (rs.Succeed)
                return Ok(rs);
            return BadRequest(rs.Error.ErrorMessage);
        }

        [HttpPost("Sites/Import")]
        public IActionResult ImportSites([FromBody]List<SiteCreateModel> sites)
        {
            var rs = _locationService.ImportSites(sites);
            if (rs.Succeed)
                return Ok(rs.Data);
            return BadRequest(rs.Error.ErrorMessage);
        }
    }
}
