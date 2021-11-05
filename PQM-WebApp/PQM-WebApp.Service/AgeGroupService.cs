using Mapster;
using Microsoft.EntityFrameworkCore;
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
    public interface IAgeGroupService
    {
        public ResultModel Create(AgeGroupCreateModel model);
        //public ResultModel Get();

        public PagingModel Get(int pageIndex, int pageSize);
        public ResultModel Update(AgeGroupViewModel model);
        public ResultModel Delete(AgeGroupViewModel model);
    }

    public class AgeGroupService : IAgeGroupService
    {
        private readonly AppDBContext _dbContext;

        public AgeGroupService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(AgeGroupCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                if (CheckExist(null, model.Name))
                {
                    throw new Exception("Name is existed");
                }
                if (model.From > model.To)
                {
                    throw new Exception("Error value inputs");
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new Exception("Name value is null");
                }
                var ageGroup = model.Adapt<AgeGroup>();
                ageGroup.Id = Guid.NewGuid();
                ageGroup.DateCreated = DateTime.Now;
                _dbContext.AgeGroups.Add(ageGroup);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = ageGroup.Adapt<AgeGroupViewModel>();
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

        /*public ResultModel Get()
        {
            var rs = new ResultModel();
            try
            {
                rs.Succeed = true;
                rs.Data = _dbContext.AgeGroups
                    .Where(s => s.IsDeleted == false)
                    .Select(s => s.Adapt<AgeGroupViewModel>()).ToList();
                return rs;
            }
            catch (Exception e)
            {
                rs.Succeed = false;
                rs.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                return rs;
            }
        }*/

        public PagingModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.AgeGroups.AsSoftDelete(false).OrderBy(x => x.DateCreated);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<AgeGroupViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(AgeGroupViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var ageGroup = _dbContext.AgeGroups.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (ageGroup == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found age group: {0}", model.Name);
                }
                else
                {
                    if (CheckExist(model.Id, model.Name))
                    {
                        throw new Exception("Name is existed");
                    }
                    if (model.From > model.To)
                    {
                        throw new Exception("Error value inputs");
                    }
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        throw new Exception("Name value is null");
                    }
                    Copy(model, ageGroup);
                    ageGroup.DateUpdated = DateTime.Now;
                    _dbContext.AgeGroups.Update(ageGroup);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = ageGroup.Adapt<AgeGroupViewModel>();
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

        public ResultModel Delete(AgeGroupViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var ageGroup = _dbContext.AgeGroups.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (ageGroup == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found age group: {0}", model.Name);
                }
                else
                {
                    ageGroup.IsDeleted = true;
                    ageGroup.DateUpdated = DateTime.Now;
                    _dbContext.AgeGroups.Update(ageGroup);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = ageGroup.Adapt<AgeGroupViewModel>();
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

        private void Copy(AgeGroupViewModel source, AgeGroup dest)
        {
            dest.Name = source.Name;
            dest.From = source.From;
            dest.To = source.To;
            dest.Order = source.Order;
        }
        private bool CheckExist(Guid? curId, string newName)
        {
            return _dbContext.AgeGroups.FirstOrDefault(x => x.Id != curId && x.Name == newName) != null;
        }
    }
}
