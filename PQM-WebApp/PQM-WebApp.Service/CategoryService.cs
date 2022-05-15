using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service.Utils;

namespace PQM_WebApp.Service
{
    public interface ICategoryService
    {
        public ResultModel Get(string category, string provinceCode = null, string districtCode = null, DateTime? from = null, DateTime? to = null);
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDBContext _dbContext;

        public CategoryService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private ResultModel GetAgeGroups(DateTime? from = null, DateTime? to = null)
        {
            var result = new ResultModel();
            try
            {
                result.Data = _dbContext.AgeGroups.AsSoftDelete(false)
                                                 .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                                                 .OrderBy(x => x.DateCreated).ToList()
                                                 .Select(s => new
                                                 {
                                                     name = s.Name,
                                                     code = s.Name,
                                                     created_at = s.DateCreated,
                                                     updated_at = s.DateUpdated,
                                                     from = s.From,
                                                     to = s.To,
                                                 }).ToList();

                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        private ResultModel GetProvinces(DateTime? from = null, DateTime? to = null)
        {
            var rs = new ResultModel();
            try
            {
                rs.Data = _dbContext.Provinces.AsSoftDelete(false)
                                                    .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                                                    .Select(s => new
                                                    {
                                                        name = s.Name,
                                                        code = s.Code,
                                                        created_at = s.DateCreated,
                                                        updated_at = s.DateUpdated
                                                    }).ToList();
                rs.Succeed = true;
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
            }
            return rs;
        }

        private ResultModel GetDistricts(string province_code = null, DateTime? from = null, DateTime? to = null)
        {
            var rs = new ResultModel();
            try
            {
                rs.Data = _dbContext.Districts.AsSoftDelete(false)
                                                    .Where(w => w.Province.Code == province_code || province_code == null)
                                                    .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                                                    .Select(s => new
                                                    {
                                                        name = s.Name,
                                                        code = s.Code,
                                                        created_at = s.DateCreated,
                                                        updated_at = s.DateUpdated,
                                                        name_with_type = s.NameWithType,
                                                        path = s.Path,
                                                        path_with_type = s.PathWithType,
                                                        province_code = s.Province.Code
                                                    }).ToList();
                rs.Succeed = true;
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
            }
            return rs;
        }

        private ResultModel GetSites(string provinceCode = null, string districtCode = null, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var filter = _dbContext.Sites.Include(s => s.District).ThenInclude(s => s.Province).AsSoftDelete(false)
                                            .Where(w => (provinceCode == null || w.District.Province.Code == provinceCode)
                                                     && (districtCode == null || w.District.Code == districtCode)
                                                     && (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)));

                var data = filter.Select(s => new
                {
                    name = s.Name,
                    code = s.Code,
                    created_at = s.DateCreated,
                    updated_at = s.DateUpdated,
                    lat = s.Lat,
                    lng = s.Lng,
                    district_code = s.District.Code,
                    district_name = s.District.Name,
                    province_code = s.District.Province.Code,
                    province_name = s.District.Province.Name,
                    site_type = s.SiteType.Name,
                }).ToList();
                return new ResultModel
                {
                    Data = data,
                    Succeed = true,
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message
                    }
                };
            }
        }

        private ResultModel GetKeyPopulations(DateTime? from = null, DateTime? to = null)
        {
            var result = new ResultModel();
            try
            {
                result.Data = _dbContext.KeyPopulations.AsSoftDelete(false)
                    .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                    .OrderBy(x => x.DateCreated).ToList()
                    .Select(s => new
                    {
                        name = s.Name,
                        code = s.Name,
                        created_at = s.DateCreated,
                        updated_at = s.DateUpdated
                    })
                    .ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        private ResultModel GetGenders(DateTime? from = null, DateTime? to = null)
        {
            var result = new ResultModel();
            try
            {
                result.Data = _dbContext.Gender.AsSoftDelete(false)
                    .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                    .OrderBy(x => x.DateCreated).ToList()
                    .Select(s => new
                    {
                        name = s.Name,
                        code = s.Name,
                        created_at = s.DateCreated,
                        updated_at = s.DateUpdated
                    })
                    .ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        private ResultModel GetIndicators(DateTime? from = null, DateTime? to = null)
        {
            var result = new PagingModel();
            try
            {
                result.Data = _dbContext.Indicators.AsSoftDelete(false)
                                        .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                .OrderBy(x => x.DateCreated).ToList()
                    .Select(s => new
                    {
                        name = s.Name,
                        code = s.Code,
                        created_at = s.DateCreated,
                        updated_at = s.DateUpdated,
                        description = s.Description
                    })
                    .ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        private ResultModel GetSiteTypes(DateTime? from = null, DateTime? to = null)
        {
            var result = new ResultModel();
            try
            {
                result.Data = _dbContext.SiteTypes.AsSoftDelete(false)
                    .Where(w => (from == null || (w.DateUpdated == null && w.DateCreated >= from.Value) || (w.DateUpdated != null && w.DateUpdated >= from.Value))
                                                     && (to == null || (w.DateUpdated == null && w.DateCreated <= to.Value) || (w.DateUpdated != null && w.DateUpdated <= to.Value)))
                .OrderBy(x => x.DateCreated).ToList()
                    .Select(s => new
                    {
                        name = s.Name,
                        code = s.Name,
                        created_at = s.DateCreated,
                        updated_at = s.DateUpdated
                    })
                    .ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Get(string category, string provinceCode = null, string districtCode = null, DateTime? from = null, DateTime? to = null)
        {
            var rs = new ResultModel { Succeed = true };
            if (string.IsNullOrEmpty(category))
            {
                return rs;
            }
            category = category.ToLower();
            switch (category)
            {
                case "province":
                    rs = GetProvinces(from, to);
                    break;
                case "district":
                    rs = GetDistricts(provinceCode, from, to);
                    break;
                case "site":
                    rs = GetSites(provinceCode, districtCode, from, to);
                    break;
                case "agegroup":
                    rs = GetAgeGroups(from, to);
                    break;
                case "keypopulation":
                    rs = GetKeyPopulations(from, to);
                    break;
                case "sex":
                    rs = GetGenders(from, to);
                    break;
                case "indicator":
                    rs = GetIndicators(from, to);
                    break;
                case "sitetype":
                    rs = GetSiteTypes(from, to);
                    break;
                default:
                    break;
            }
            return rs;
        }
    }
}
