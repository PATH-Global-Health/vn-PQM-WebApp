using Mapster;
using Microsoft.EntityFrameworkCore;
using PQM_WebApp.Data;
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
        ResultModel GetDistricts(string provinceCode);
    }

    public class LocationService : ILocationService
    {
        private readonly AppDBContext _dBContext;

        public LocationService(AppDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public PagingModel GetProvinces(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dBContext.Provinces.Where(_ => _.IsDeleted == false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.PageData(pageIndex, pageSize).Adapt<IEnumerable<ProvinceModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel GetDistricts(string provinceCode)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dBContext.Districts.Where(_ => _.IsDeleted == false)
                                                 .Where(_ => _.ParentCode == provinceCode);
                result.Data = filter.AsNoTracking().AsEnumerable().Adapt<IEnumerable<DistrictModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
