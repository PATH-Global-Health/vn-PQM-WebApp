using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.ViewModels;
using PQM_WebApp.Service.Utils;

namespace PQM_WebApp.Service
{
    public interface IDataPermissionService
    {
        public ResultModel Create(DataPermissionCreateModel dataPermission);
        public PagingModel Get(int pageIndex, int pageSize, string username = null);
        public ResultModel Delete(Guid id);
    }

    public class DataPermissionService : IDataPermissionService
    {
        private readonly AppDBContext _dbContext;

        public DataPermissionService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Create(DataPermissionCreateModel dataPermission)
        {
            try
            {
                var existed = _dbContext.DataPermissions
                                        .FirstOrDefault(s
                                            => (dataPermission.Type == DataPermissionType.Read || s.IndicatorId == dataPermission.IndicatorId)
                                            && s.Username == dataPermission.Username
                                            && s.Type == dataPermission.Type
                                            && s.ProvinceId == dataPermission.ProvinceId
                                        );
                if (existed != null)
                {
                    return new ResultModel
                    {
                        Succeed = false,
                        Error = new ErrorModel
                        {
                            ErrorMessage = string.Format("data permission is existed")
                        }
                    };
                }
                var entity = dataPermission.Adapt<DataPermission>();
                _dbContext.DataPermissions.Add(entity);
                _dbContext.SaveChanges();
                return new ResultModel
                {
                    Succeed = true,
                    Data = entity.Adapt<DataPermissionViewModel>()
                };
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message
                    }
                };
            }
        }

        public ResultModel Delete(Guid id)
        {
            var rs = new ResultModel();
            try
            {
                var e = _dbContext.DataPermissions.FirstOrDefault(s => s.Id == id);
                if (e == null)
                {
                    rs.Succeed = false;
                    rs.Error.ErrorMessage = string.Format("Not found data permission: {0}", id);
                }
                else
                {
                    _dbContext.DataPermissions.Remove(e);
                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = new
                        {
                            id,
                        };
                    }
                }
                return rs;
            }
            catch (Exception ex)
            {
                rs.Error.ErrorMessage = ex.Message;
                return rs;
            }
        }

        public PagingModel Get(int pageIndex, int pageSize, string username = null)
        {
            var result = new PagingModel();
            try
            {
                var filter = _dbContext.DataPermissions.AsSoftDelete(false).Where(s => string.IsNullOrEmpty(username) || s.Username == username);
                result.Total = filter.Count();
                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.Skip(pageIndex * pageSize).Take(pageSize).Adapt<IEnumerable<DataPermission>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }
    }
}
