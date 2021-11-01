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
    public interface IGenderService
    {
        ResultModel Create(GenderCreateModel model);
        PagingModel Get(int pageIndex, int pageSize);
        ResultModel Update(GenderViewModel model);
        ResultModel Delete(GenderViewModel model);
    }

    public class GenderService : IGenderService
    {
        private readonly AppDBContext _dbContext;

        public GenderService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(GenderCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new Exception("Name value is null");
                }
                var Gender = model.Adapt<Gender>();
                Gender.Id = Guid.NewGuid();
                Gender.DateCreated = DateTime.Now;
                _dbContext.Gender.Add(Gender);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = Gender.Adapt<GenderViewModel>();
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

        public PagingModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.Gender.Where(_ => _.IsDeleted == false);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<GenderViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(GenderViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var gender = _dbContext.Gender.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (gender == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found Gender: {0}", model.Name);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        throw new Exception("Name value is null");
                    }
                    Copy(model, gender);
                    gender.DateUpdated = DateTime.Now;
                    _dbContext.Gender.Update(gender);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = gender.Adapt<GenderViewModel>();
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

        public ResultModel Delete(GenderViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var gender = _dbContext.Gender.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (gender == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found Gender: {0}", model.Name);
                }
                else
                {
                    gender.IsDeleted = true;
                    gender.DateUpdated = DateTime.Now;
                    _dbContext.Gender.Update(gender);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = gender.Adapt<GenderViewModel>();
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

        private void Copy(GenderViewModel source, Gender dest)
        {
            dest.Name = source.Name;
            dest.CreatedBy = source.CreatedBy;
            dest.Order = source.Order;
        }
    }
}
