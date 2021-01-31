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
    public interface IIndicatorService
    {
        public ResultModel Create(IndicatorCreateModel model);
        public ResultModel Get();
    }

    public class IndicatorService : IIndicatorService
    {
        private readonly AppDBContext _dbContext;

        public IndicatorService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(IndicatorCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicator = model.Adapt<Indicator>();
                indicator.Id = Guid.NewGuid();
                _dbContext.Indicators.Add(indicator);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = indicator.Adapt<IndicatorViewModel>();
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
                rs.Data = _dbContext.Indicators.Select(s => s.Adapt<IndicatorViewModel>()).ToList();
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
