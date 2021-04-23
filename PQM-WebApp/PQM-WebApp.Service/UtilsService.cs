using Mapster;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface IUtilsService
    {
        ResultModel ImportProvince(IEnumerable<ProvinceModel> models);
        ResultModel ImportDistrict(IEnumerable<DistrictModel> models);
    }

    public class UtilsService : IUtilsService
    {
        private readonly AppDBContext _dbContext;

        public UtilsService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel ImportDistrict(IEnumerable<DistrictModel> models)
        {
            var rs = new ResultModel();
            try
            {
                var districts = models.Adapt<List<District>>();
                var provinces = _dbContext.Provinces.AsEnumerable();
                if (provinces == null || provinces.Count() == 0)
                {
                    throw new Exception("No province for reference.");
                }
                //
                foreach (var district in districts)
                {
                    var province = provinces.FirstOrDefault(_ => _.Code == district.ParentCode);
                    if (province == null)
                    {
                        districts.Remove(district);
                    }
                    district.ProvinceId = province.Id;
                }
                _dbContext.Districts.AddRange(districts);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = districts.Adapt<List<DistrictModel>>();
                    return rs;
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

        public ResultModel ImportProvince(IEnumerable<ProvinceModel> models)
        {
            var rs = new ResultModel();
            try
            {
                var provinces = models.Adapt<List<Province>>();
                //
                _dbContext.Provinces.AddRange(provinces);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = provinces.Adapt<List<ProvinceModel>>();
                    return rs;
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
    }
}
