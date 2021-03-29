using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mapster;

namespace PQM_WebApp.Service
{
    public interface ISexService
    {
        ResultModel Create(SexCreateModel model);
        PagingModel Get(int pageIndex, int pageSize);
        ResultModel Update(SexViewModel model);
        ResultModel Delete(SexViewModel model);
    }

    public class SexService : ISexService
    {
        private readonly AppDBContext _dbContext;

        public SexService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(SexCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var sex = model.Adapt<Sex>();
                sex.Id = Guid.NewGuid();
                _dbContext.Sex.Add(sex);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = sex.Adapt<SexViewModel>();
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
                var filter = _dbContext.Sex.Where(_ => _.IsDeleted == false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<SexViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(SexViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var sex = _dbContext.Sex.Find(model.Id);
                if (sex == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found sex: {0}", model.Name);
                }
                else
                {
                    Copy(model, sex);
                    sex.DateUpdated = DateTime.Now;
                    _dbContext.Sex.Update(sex);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = sex.Adapt<SexViewModel>();
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

        public ResultModel Delete(SexViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var sex = _dbContext.Sex.Find(model.Id);
                if (sex == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found sex: {0}", model.Name);
                }
                else
                {
                    sex.IsDeleted = true;
                    sex.DateUpdated = DateTime.Now;
                    _dbContext.Sex.Update(sex);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = sex.Adapt<SexViewModel>();
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

        private void Copy(SexViewModel source, Sex dest)
        {
            dest.Name = source.Name;
            dest.CreatedBy = source.CreatedBy;
            dest.Order = source.Order;
        }
    }
}
