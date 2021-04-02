using Mapster;
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
    public interface IIndicatorGroupService
    {
        public ResultModel Create(IndicatorGroupCreateModel model);
        public ResultModel Get(int pageIndex, int pageSize);
        public ResultModel Update(IndicatorGroupViewModel model);
        public ResultModel Delete(IndicatorGroupViewModel model);
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
                indicatorGroup.DateCreated = DateTime.Now;
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

        public ResultModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.IndicatorGroups.Where(_ => _.IsDeleted == false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<IndicatorGroupViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(IndicatorGroupViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicatorGroup = _dbContext.IndicatorGroups.Find(model.Id);
                if (indicatorGroup == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found indicator group: {0}", model.Name);
                }
                else
                {
                    Copy(model, indicatorGroup);
                    indicatorGroup.DateUpdated = DateTime.Now;
                    _dbContext.IndicatorGroups.Update(indicatorGroup);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = indicatorGroup.Adapt<IndicatorGroupViewModel>();
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

        public ResultModel Delete(IndicatorGroupViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var indicatorGroup = _dbContext.IndicatorGroups.Find(model.Id);
                if (indicatorGroup == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found indicator group: {0}", model.Name);
                }
                else
                {
                    indicatorGroup.IsDeleted = true;
                    indicatorGroup.DateUpdated = DateTime.Now;
                    _dbContext.IndicatorGroups.Update(indicatorGroup);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = indicatorGroup.Adapt<IndicatorGroupViewModel>();
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

        private void Copy(IndicatorGroupViewModel source, IndicatorGroup dest)
        {
            dest.Name = source.Name;
            dest.CreatedBy = source.CreatedBy;
        }
    }
}
