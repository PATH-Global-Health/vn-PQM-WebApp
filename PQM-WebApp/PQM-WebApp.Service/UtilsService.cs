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
        bool FixVLUnsupressed();
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

        public bool FixVLUnsupressed()
        {
            var id = Guid.Parse("3dcb48cd-666a-4e04-8900-07f90289b320");
            var txcurr = Guid.Parse("3dcb48cd-666a-4e04-8900-07f90289b380");
            var vls = _dbContext.AggregatedValues.Where(s => s.IndicatorId == id);
            var s = 0;
            var f = 0;
            foreach (var vl in vls)
            {
                var curr = _dbContext.AggregatedValues.FirstOrDefault(w =>
                       w.IndicatorId == txcurr
                    && w.SiteId == vl.SiteId
                    && w.GenderId == vl.GenderId
                    && w.KeyPopulation == vl.KeyPopulation
                    && w.AgeGroupId == vl.AgeGroupId
                    && w.Year == vl.Year
                    && w.Month == vl.Month
                );
                if (curr != null && curr.Numerator >= vl.Numerator)
                {
                    vl.Denominator = curr.Numerator;
                    _dbContext.AggregatedValues.Update(vl);
                    s++;
                }
                else
                {
                    f++;
                }
            }
            _dbContext.SaveChanges();
            return true;
        }
    }
}
