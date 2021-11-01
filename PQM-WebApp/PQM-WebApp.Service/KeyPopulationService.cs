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
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PQM_WebApp.Service
{
    public interface IKeyPopulationService
    {
        ResultModel Create(KeyPopulationCreateModel model);
        PagingModel Get(int pageIndex, int pageSize);
        ResultModel Update(KeyPopulationViewModel model);
        ResultModel Delete(KeyPopulationViewModel model);
    }

    public class KeyPopulationService : IKeyPopulationService
    {
        private readonly AppDBContext _dbContext;

        public KeyPopulationService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(KeyPopulationCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                if (CheckExist(null, model.Name))
                {
                    throw new Exception("Name is existed");
                }
                var keyPopulation = model.Adapt<KeyPopulation>();
                keyPopulation.Id = Guid.NewGuid();
                keyPopulation.DateCreated = DateTime.Now;
                _dbContext.KeyPopulations.Add(keyPopulation);
                rs.Succeed = _dbContext.SaveChanges() > 0;
                if (rs.Succeed)
                {
                    rs.Data = keyPopulation.Adapt<KeyPopulationViewModel>();
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
                var filter = _dbContext.KeyPopulations.AsSoftDelete(false).OrderBy(x => x.DateCreated);
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<KeyPopulationViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Update(KeyPopulationViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var keyPopulation = _dbContext.KeyPopulations.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (keyPopulation == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found key population: {0}", model.Name);
                }
                else
                {

                    if (CheckExist(keyPopulation.Id, model.Name))
                    {
                        throw new Exception("Name is existed");
                    }
                    Copy(model, keyPopulation);
                    keyPopulation.DateUpdated = DateTime.Now;
                    _dbContext.KeyPopulations.Update(keyPopulation);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = keyPopulation.Adapt<KeyPopulationViewModel>();
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

        public ResultModel Delete(KeyPopulationViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var keyPopulation = _dbContext.KeyPopulations.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (keyPopulation == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found key population: {0}", model.Name);
                }
                else
                {
                    keyPopulation.IsDeleted = true;
                    keyPopulation.DateUpdated = DateTime.Now;
                    _dbContext.KeyPopulations.Update(keyPopulation);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = keyPopulation.Adapt<KeyPopulationViewModel>();
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

        private void Copy(KeyPopulationViewModel source, KeyPopulation dest)
        {
            dest.Name = source.Name;
            dest.CreatedBy = source.CreatedBy;
            dest.Order = source.Order;
        }
        private bool CheckExist(Guid? curId, string newName)
        {
            return _dbContext.KeyPopulations.FirstOrDefault(x => x.Id != curId && x.Name == newName) != null;
        }
    }
}
