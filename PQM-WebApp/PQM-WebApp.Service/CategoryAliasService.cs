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
    public interface ICategoryAliasService
    {
        ResultModel Create(CategoryAliasCreateModel model);
        PagingModel Get(Guid categoryId, string category, int pageIndex, int pageSize);
        ResultModel Update(CategoryAliasViewModel model);
        ResultModel Delete(CategoryAliasViewModel model);
    }

    public class CategoryAliasService : ICategoryAliasService
    {
        private readonly AppDBContext _dbContext;

        public CategoryAliasService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(CategoryAliasCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var categoryAlias = model.Adapt<CategoryAlias>();
                _dbContext.CategoryAliases.Add(categoryAlias);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = categoryAlias.Adapt<CategoryAliasViewModel>();
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
        
        public PagingModel Get(Guid categoryId, string category, int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                IQueryable<CategoryAlias> filter;

                if (categoryId == Guid.Empty && category == null)
                {
                    filter = _dbContext.CategoryAliases.AsSoftDelete(false);
                }
                else if (categoryId != Guid.Empty && category != null)
                {
                    filter = _dbContext.CategoryAliases.AsSoftDelete(false).Where(s => s.CategoryId == categoryId && s.Category == category);
                }
                else
                {
                    filter = _dbContext.CategoryAliases.AsSoftDelete(false).Where(s => categoryId != Guid.Empty ? s.CategoryId == categoryId : s.Category == category);
                }
                
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.PageData(pageIndex, pageSize).Adapt<IEnumerable<CategoryAliasViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(CategoryAliasViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var categoryAlias = _dbContext.CategoryAliases.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (categoryAlias == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found alias {0}", model.Alias);
                }
                else
                {
                    model.Adapt(categoryAlias);
                    categoryAlias.DateUpdated = DateTime.Now;
                    _dbContext.CategoryAliases.Update(categoryAlias);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = categoryAlias.Adapt<CategoryAliasViewModel>();
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
        
        public ResultModel Delete(CategoryAliasViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var categoryAlias = _dbContext.CategoryAliases.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (categoryAlias == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found alias {0}", model.Alias);
                }
                else
                {
                    categoryAlias.IsDeleted = true;
                    categoryAlias.DateUpdated = DateTime.Now;
                    _dbContext.CategoryAliases.Update(categoryAlias);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = categoryAlias.Adapt<CategoryAliasViewModel>();
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
    }
}
