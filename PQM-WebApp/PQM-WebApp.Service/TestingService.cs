using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface ITestingService
    {
        ResultModel GetHealthMap(int year, int quater, int? month, string provinceCode, string districtCode);
    }

    public class TestingService : ITestingService
    {
        private AppDBContext _dbContext { get; set; }

        public TestingService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel GetHealthMap(int year, int quarter, int? month, string provinceCode, string districtCode)
        {
            try
            {
                var indicatorName = "HTS_TST_Recency";

                var fromMonth = quarter == 1 ? 1 : quarter == 2 ? 4 : quarter == 3 ? 7 : 10;
                var toMonth = quarter == 1 ? 3 : quarter == 2 ? 6 : quarter == 3 ? 9 : 12;
                var isMonth = month != null;

                var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || d.Code == districtCode)).Select(s => s.Id);
                var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
                var aggregatedValues = _dbContext.AggregatedValues.Where(w => w.Year == year
                                                                           && w.Quarter == quarter
                                                                           && (!isMonth || (fromMonth <= w.Month && w.Month <= toMonth))
                                                                           && sites.Contains(w.SiteId)
                                                                           && w.Indicator.Name == indicatorName).ToList();
                var rs = aggregatedValues.GroupBy(f => f.Site.District)
                    .Select(s => new
                    {
                        Lat = s.Key.Lat,
                        Lon = s.Key.Lng,
                        Count = s.Sum(f => f.Numerator),
                    })
                    .ToList();
                return new ResultModel()
                {
                    Succeed = true,
                    Data = rs,
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message
                    }
                };
            }
        }
    }
}
