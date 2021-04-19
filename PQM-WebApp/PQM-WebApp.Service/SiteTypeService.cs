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
    public interface ISiteTypeService
    {
        ResultModel Create(SiteTypeCreateModel model);
        PagingModel Get(int pageIndex, int pageSize);
        ResultModel Update(SiteTypeViewModel model);
        ResultModel Delete(SiteTypeViewModel model);
    }

    public class SiteTypeService : ISiteTypeService
    {
        private readonly AppDBContext _dbContext;

        public SiteTypeService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(SiteTypeCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var siteType = model.Adapt<SiteType>();
                siteType.Id = Guid.NewGuid();
                siteType.DateCreated = DateTime.Now;
                _dbContext.SiteTypes.Add(siteType);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = siteType.Adapt<SiteTypeViewModel>();
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

        public PagingModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.SiteTypes.AsSoftDelete(false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.PageData(pageIndex, pageSize).Adapt<IEnumerable<SiteTypeViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(SiteTypeViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var siteType = _dbContext.SiteTypes.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (siteType == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found site type: {0}", model.Name);
                }
                else
                {
                    Copy(model, siteType);
                    siteType.DateUpdated = DateTime.Now;
                    _dbContext.SiteTypes.Update(siteType);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = siteType.Adapt<SiteTypeViewModel>();
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

        public ResultModel Delete(SiteTypeViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var siteType = _dbContext.SiteTypes.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (siteType == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found site type: {0}", model.Name);
                }
                else
                {
                    siteType.IsDeleted = true;
                    siteType.DateUpdated = DateTime.Now;
                    _dbContext.SiteTypes.Update(siteType);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = siteType.Adapt<SiteTypeViewModel>();
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

        private void Copy(SiteTypeViewModel source, SiteType dest)
        {
            dest.Name = source.Name;
            dest.CreatedBy = source.CreatedBy;
        }
    }
}
