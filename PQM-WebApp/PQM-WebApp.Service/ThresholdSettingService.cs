using System;
using System.Linq;
using Mapster;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;

namespace PQM_WebApp.Service
{
    public interface IThresholdSettingService
    {
        ResultModel Get(int? pageIndex = 0, int? pageSize = 20);
        ResultModel Create(ThresholdSettingCreateModel thresholdSetting);
        ResultModel Update(ThresholdSettingViewModel thresholdSetting);
        ResultModel Delete(Guid id);
    }

    public class ThresholdSettingService : IThresholdSettingService
    {
        private readonly AppDBContext _dbContext;

        public ThresholdSettingService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(ThresholdSettingCreateModel thresholdSetting)
        {
            var rs = new ResultModel();
            try
            {
                var _ = thresholdSetting.Adapt<ThresholdSetting>();
                _dbContext.ThresholdSettings.Add(_);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = _.Adapt<ThresholdSettingViewModel>();
                    return rs;
                }
                return rs;
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
                return rs;
            }
        }

        public ResultModel Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public ResultModel Get(int? pageIndex = 0, int? pageSize = 20)
        {
            var rs = new ResultModel();
            try
            {
                var data = _dbContext.ThresholdSettings.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value)
                                                       .ToList().Select(s => s.Adapt<ThresholdSettingViewModel>()).ToList();
                rs.Succeed = true;
                rs.Data = data;
                return rs;
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
                return rs;
            }
        }

        public ResultModel Update(ThresholdSettingViewModel thresholdSetting)
        {
            throw new NotImplementedException();
        }
    }
}
