using Mapster;
using Microsoft.EntityFrameworkCore;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface ILocationService
    {
        PagingModel GetProvinces(int pageIndex, int pageSize);
        ResultModel CreateProvince(ProvinceCreateModel model);
        ResultModel UpdateProvince(ProvinceModel model);
        ResultModel DeleteProvince(ProvinceModel model);

        ResultModel GetDistricts(string provinceCode);
        ResultModel CreateDistrict(DistrictCreateModel model);
        ResultModel UpdateDistrict(DistrictModel model);
        ResultModel DeleteDistrict(DistrictModel model);

        ResultModel GetSites(Guid districtId);
        ResultModel GetSites(int pageIndex, int pageSize, string proviceCode = null, string districtCode = null);
        ResultModel CreateSite(SiteCreateModel model);
        ResultModel UpdateSite(SiteViewModel model);
        ResultModel DeleteSite(SiteViewModel model);
    }

    public class LocationService : ILocationService
    {
        private readonly AppDBContext _dbContext;

        public LocationService(AppDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public PagingModel GetProvinces(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                /*var filter = _dbContext.Provinces.Where(_ => _.IsDeleted == false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.PageData(pageIndex, pageSize).Adapt<IEnumerable<ProvinceModel>>();
                result.Succeed = true;*/
                var filter = _dbContext.Provinces.AsSoftDelete(false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<ProvinceModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel CreateProvince(ProvinceCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var province = model.Adapt<Province>();
                province.Id = Guid.NewGuid();
                _dbContext.Provinces.Add(province);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = province.Adapt<ProvinceModel>();
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateProvince(ProvinceModel model)
        {
            var rs = new ResultModel();
            try
            {
                var province = _dbContext.Provinces.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (province == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found province: {0}", model.Name);
                }
                else
                {
                    CopyProvince(model, province);
                    province.DateUpdated = DateTime.Now;
                    _dbContext.Provinces.Update(province);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = province.Adapt<ProvinceModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteProvince(ProvinceModel model)
        {
            var rs = new ResultModel();
            try
            {
                var province = _dbContext.Provinces.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (province == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found province: {0}", model.Name);
                }
                else
                {
                    province.IsDeleted = true;
                    province.DateUpdated = DateTime.Now;
                    if (province.Districts != null)
                    {
                        foreach(var district in province.Districts)
                        {
                            district.IsDeleted = true;
                            if (district.Sites != null)
                            {
                                foreach(var site in district.Sites)
                                {
                                    site.IsDeleted = true;
                                }
                            }
                        }
                    }
                    _dbContext.Provinces.Update(province);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = province.Adapt<ProvinceModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel GetDistricts(string provinceCode)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Districts.AsSoftDelete(false)
                                                 .Where(_ => _.ParentCode == provinceCode);
                result.Data = filter.AsEnumerable().Adapt<IEnumerable<DistrictModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel CreateDistrict(DistrictCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var district = model.Adapt<District>();
                var province = _dbContext.Provinces.AsSoftDelete(false).FirstOrDefault(s => s.Code == district.ParentCode);
                if (province == null)
                {
                    throw new Exception("No province for reference.");
                }

                district.ProvinceId = province.Id;
                district.Province = province;
                district.Id = Guid.NewGuid();
                district.DateCreated = DateTime.Now;
                _dbContext.Districts.Add(district);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = district.Adapt<DistrictModel>();
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateDistrict(DistrictModel model)
        {
            var rs = new ResultModel();
            try
            {
                var district = _dbContext.Districts.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (district == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found district: {0}", model.Name);
                }
                else
                {
                    CopyDistrict(model, district);
                    district.DateUpdated = DateTime.Now;
                    var provinces = _dbContext.Provinces.AsSoftDelete(false).AsEnumerable();
                    var province = provinces.FirstOrDefault(s => s.Code == district.ParentCode);
                    if (province == null)
                    {
                        throw new Exception("No province for reference.");
                    }
                    district.ProvinceId = province.Id;
                    district.Province = province;

                    _dbContext.Districts.Update(district);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = district.Adapt<DistrictModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteDistrict(DistrictModel model)
        {
            var rs = new ResultModel();
            try
            {
                var district = _dbContext.Districts.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (district == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found district: {0}", model.Name);
                }
                else
                {
                    district.IsDeleted = true;
                    district.DateUpdated = DateTime.Now;
                    if (district.Sites != null)
                    {
                        foreach(var site in district.Sites)
                        {
                            site.IsDeleted = true;
                        }
                    }
                    _dbContext.Districts.Update(district);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = district.Adapt<DistrictModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel GetSites(Guid districtId)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Sites.AsSoftDelete(false)
                                                 .Where(_ => _.DistrictId == districtId);
                result.Data = filter.AsEnumerable().Adapt<IEnumerable<SiteViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel GetSites(int pageIndex, int pageSize, string proviceCode = null, string districtCode = null)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Sites.Include(s => s.District).ThenInclude(s => s.Province).AsSoftDelete(false)
                                            .Where(w => (proviceCode == null || w.District.Province.Code == proviceCode)
                                                     && (districtCode == null || w.District.Code == districtCode))
                                            .Skip(pageIndex * pageSize)
                                            .Take(pageSize);
                result.Data = filter.AsEnumerable().Adapt<IEnumerable<SiteViewModel>>();
                result.Succeed = true;
            }
            catch (Exception ex)
            {
                ex.Adapt(result);
            }
            return result;
        }

        public ResultModel CreateSite(SiteCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = model.Adapt<Site>();
                var district = _dbContext.Districts.AsSoftDelete(false).FirstOrDefault(s => s.Id == site.DistrictId);
                if (district == null)
                {
                    throw new Exception("No district for reference.");
                }

                var siteType = _dbContext.SiteTypes.AsSoftDelete(false).FirstOrDefault(s => s.Id == site.SiteTypeId);
                if (siteType == null)
                {
                    throw new Exception("No site type for reference.");
                }

                site.Id = Guid.NewGuid();
                site.DateCreated = DateTime.Now;
                site.District = district;
                site.SiteType = siteType;
                _dbContext.Sites.Add(site);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = site.Adapt<SiteViewModel>();
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateSite(SiteViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = _dbContext.Sites.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (site == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found site: {0}", model.Name);
                }
                else
                {
                    CopySite(model, site);
                    site.DateUpdated = DateTime.Now;
                    var district = _dbContext.Districts.AsSoftDelete(false).FirstOrDefault(s => s.Id == site.DistrictId);
                    if (district == null)
                    {
                        throw new Exception("No district for reference.");
                    }

                    var siteType = _dbContext.SiteTypes.AsSoftDelete(false).FirstOrDefault(s => s.Id == site.SiteTypeId);
                    if (siteType == null)
                    {
                        throw new Exception("No site type for reference.");
                    }
                    site.District = district;
                    site.SiteType = siteType;
                    _dbContext.Sites.Update(site);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = site.Adapt<SiteViewModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteSite(SiteViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = _dbContext.Sites.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (site == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found site: {0}", model.Name);
                }
                else
                {
                    site.IsDeleted = true;
                    site.DateUpdated = DateTime.Now;
                    _dbContext.Sites.Update(site);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = site.Adapt<SiteViewModel>();
                    }
                }
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        private void CopyProvince(ProvinceModel source, Province dest)
        {
            dest.Name = source.Name;
            dest.NameWithType = source.NameWithType;
            dest.Slug = source.Slug;
            dest.Code = source.Code;
            dest.Type = source.Type;
        }

        private void CopyDistrict(DistrictModel source, District dest)
        {
            dest.Code = source.Code;
            dest.Name = source.Name;
            dest.Type = source.Type;
            dest.NameWithType = source.NameWithType;
            dest.Path = source.Path;
            dest.PathWithType = source.PathWithType;
            dest.Slug = source.Slug;
            dest.ParentCode = source.ParentCode;
            dest.CreatedBy = source.CreatedBy;
        }

        private void CopySite(SiteViewModel source, Site site)
        {
            site.Name = source.Name;
            site.CreatedBy = source.CreatedBy;
            site.Code = source.Code;
            site.Order = source.Order;
            site.DistrictId = source.DistrictId;
            site.SiteTypeId = source.SiteTypeId;
            site.Lat = source.Lat;
            site.Lng = source.Lng;
        }

        
    }
}
