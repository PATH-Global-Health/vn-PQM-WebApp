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
    public interface IIndicatorGroupService
    {
        public ResultModel Create(IndicatorGroupCreateModel model);
        public ResultModel Get();
    }

    public class IndicatorGroupService : IIndicatorGroupService
    {
        private readonly AppDBContext _dbContext;

        public IndicatorGroupService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(IndicatorGroupCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicatorGroup = model.Adapt<IndicatorGroup>();
                indicatorGroup.Id = Guid.NewGuid();
                _dbContext.IndicatorGroups.Add(indicatorGroup);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = indicatorGroup.Adapt<IndicatorGroupViewModel>();
                    return rs;
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

        public ResultModel Get()
        {
            var rs = new ResultModel();
            try
            {
                rs.Succeed = true;
                rs.Data = _dbContext.IndicatorGroups.Select(s => s.Adapt<IndicatorGroupViewModel>()).ToList();
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }
    }
}
