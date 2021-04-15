﻿using Mapster;
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
                var filter = _dbContext.Provinces.Where(_ => _.IsDeleted == false);
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateProvince(ProvinceModel model)
        {
            var rs = new ResultModel();
            try
            {
                var province = _dbContext.Provinces.Find(model.Id);
                if (province == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found province: {0}", model.Name);
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteProvince(ProvinceModel model)
        {
            var rs = new ResultModel();
            try
            {
                var province = _dbContext.Provinces.Find(model.Id);
                if (province == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found province: {0}", model.Name);
                }
                else
                {
                    province.IsDeleted = true;
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel GetDistricts(string provinceCode)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Districts.Where(_ => _.IsDeleted == false)
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
                var provinces = _dbContext.Provinces.AsEnumerable();
                var province = provinces.FirstOrDefault(s => s.Code == district.ParentCode && s.IsDeleted == false);
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateDistrict(DistrictModel model)
        {
            var rs = new ResultModel();
            try
            {
                var district = _dbContext.Districts.Find(model.Id);
                if (district == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found district: {0}", model.Name);
                }
                else
                {
                    CopyDistrict(model, district);
                    district.DateUpdated = DateTime.Now;
                    var provinces = _dbContext.Provinces.AsEnumerable();
                    var province = provinces.FirstOrDefault(s => s.Code == district.ParentCode && s.IsDeleted == false);
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteDistrict(DistrictModel model)
        {
            var rs = new ResultModel();
            try
            {
                var district = _dbContext.Districts.Find(model.Id);
                if (district == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found district: {0}", model.Name);
                }
                else
                {
                    district.IsDeleted = true;
                    district.DateUpdated = DateTime.Now;
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel GetSites(Guid districtId)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.Sites.Where(_ => _.IsDeleted == false)
                                                 .Where(_ => _.DistrictId == districtId);
                result.Data = filter.AsNoTracking().AsEnumerable().Adapt<IEnumerable<SiteViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel CreateSite(SiteCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = model.Adapt<Site>();
                var districts = _dbContext.Districts.AsEnumerable();
                var district = districts.FirstOrDefault(s => s.Id == site.DistrictId && s.IsDeleted == false);
                if (district == null)
                {
                    throw new Exception("No district for reference.");
                }

                site.Id = Guid.NewGuid();
                site.DateCreated = DateTime.Now;
                site.District = district;
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel UpdateSite(SiteViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = _dbContext.Sites.Find(model.Id);
                if (site == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found site: {0}", model.Name);
                }
                else
                {
                    CopySite(model, site);
                    site.DateUpdated = DateTime.Now;
                    var districts = _dbContext.Districts.AsEnumerable();
                    var district = districts.FirstOrDefault(s => s.Id == site.DistrictId && s.IsDeleted == false);
                    if (district == null)
                    {
                        throw new Exception("No district for reference.");
                    }
                    site.District = district;
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel DeleteSite(SiteViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var site = _dbContext.Sites.Find(model.Id);
                if (site == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found site: {0}", model.Name);
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
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
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
        }
    }
}
