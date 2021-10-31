using Mapster;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using System;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using PQM_WebApp.Service.Utils;

namespace PQM_WebApp.Service
{
    public interface IIndicatorService
    {
        public ResultModel Create(IndicatorCreateModel model);
        public ResultModel Get(int pageIndex, int pageSize);
        public ResultModel Update(IndicatorViewModel model);
        public ResultModel Delete(IndicatorViewModel model);
        
        public ResultModel GetValue(int year, int quater, int? month, string provinceCode, string districtCode, string indicatorCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
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
                indicator.DateCreated = DateTime.Now;
                var indicatorGroup = _dbContext.IndicatorGroups.AsSoftDelete(false).FirstOrDefault(s => s.Id == indicator.IndicatorGroupId);
                if (indicatorGroup == null)
                {
                    throw new Exception("No indicator group for reference.");
                }
                indicator.IndicatorGroup = indicatorGroup;
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
                rs.Error.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }

        public ResultModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.Indicators.AsSoftDelete(false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<IndicatorViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(IndicatorViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicator = _dbContext.Indicators.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (indicator == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found indicator: {0}", model.Name);
                }
                else
                {
                    Copy(model, indicator);
                    indicator.DateUpdated = DateTime.Now;
                    var indicatorGroups = _dbContext.IndicatorGroups.AsEnumerable();
                    var indicatorGroup = indicatorGroups.FirstOrDefault(s => s.Id == indicator.IndicatorGroupId && s.IsDeleted == false);
                    if (indicatorGroup == null)
                    {
                        throw new Exception("No indicator group for reference.");
                    }
                    indicator.IndicatorGroup = indicatorGroup;
                    _dbContext.Indicators.Update(indicator);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = indicator.Adapt<IndicatorViewModel>();
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

        public ResultModel Delete(IndicatorViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicator = _dbContext.Indicators.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (indicator == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found indicator: {0}", model.Name);
                }
                else
                {
                    indicator.IsDeleted = true;
                    indicator.DateUpdated = DateTime.Now;
                    indicator.IndicatorGroup = null;
                    _dbContext.Indicators.Update(indicator);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = indicator.Adapt<IndicatorViewModel>();
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

        public ResultModel GetValue(int year, int quater, int? month, string provinceCode, string districtCode, string indicatorCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            throw new NotImplementedException();
        }

        private void Copy(IndicatorViewModel source, Indicator dest)
        {
            dest.Code = source.Code;
            dest.Name = source.Name;
            dest.Description = source.Description;
            dest.Order = source.Order;
            dest.IsTotal = source.IsTotal;
            dest.CreatedBy = source.CreatedBy;
            dest.IndicatorGroupId = source.IndicatorGroupId;
        }
    }
}
