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
    public interface ILanguageService
    {
        ResultModel CreateLanguage(LanguageCreateModel model);

        PagingModel Get(int pageIndex, int pageSize);

        ResultModel Get(Guid languageId);

        ResultModel UpdateLanguage(LanguageViewModel model);

        ResultModel DeleteLanguage(LanguageViewModel model);
    }

    public class LanguageService : ILanguageService
    {
        private readonly AppDBContext _dbContext;

        public LanguageService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel CreateLanguage(LanguageCreateModel model)
        {
            var rs = new ResultModel();
            try
            {
                var language = _dbContext.Languages.AsSoftDelete(false).FirstOrDefault(s => s.Code == model.Code);
                if (language != null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("The language {0} already existed", model.Code);
                }
                else
                {
                    language = model.Adapt<Language>();
                    language.Id = Guid.NewGuid();
                    language.DateCreated = DateTime.Now;
                    if (language.Dictionary != null)
                    {
                        language.Dictionary.LanguageId = language.Id;
                    }
                    _dbContext.Languages.Add(language);
                    if (language.Dictionary != null)
                    {
                        _dbContext.LanguageDictionaries.Add(language.Dictionary);
                    }

                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = language.Adapt<LanguageViewModel>();
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

        public PagingModel Get(int pageIndex, int pageSize)
        {
            var result = new PagingModel();
            try
            {
                IQueryable<Language> filter;

                filter = _dbContext.Languages.AsSoftDelete(false);

                result.PageCount = filter.PageCount(pageSize);
                result.Data = filter.PageData(pageIndex, pageSize).Adapt<IEnumerable<LanguageViewModel>>();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel Get(Guid languageId)
        {
            var rs = new ResultModel();
            try
            {
                var language = _dbContext.Languages.AsSoftDelete(false).FirstOrDefault(s => s.Id == languageId);
                if (language == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found language: {0}", languageId);
                }
                else
                {
                    language.Dictionary = _dbContext.LanguageDictionaries.AsSoftDelete(false).FirstOrDefault(s => s.LanguageId == languageId);
                    rs.Succeed = true;
                    rs.Data = language.Adapt<LanguageViewModel>();
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

        public ResultModel UpdateLanguage(LanguageViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var language = _dbContext.Languages.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (language == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found language: {0}", model.Name);
                }
                else
                {
                    var languageList = _dbContext.Languages.AsSoftDelete(false).Where(s => s.Code == model.Code).AsEnumerable<Language>().ToList();
                    if (languageList.Count >= 2)
                    {
                        rs.Succeed = false;
                        rs.ErrorMessage = string.Format("The language {0} already existed", model.Code);
                    }
                    else
                    {
                        model.Adapt(language);
                        language.DateUpdated = DateTime.Now;
                        _dbContext.Languages.Update(language);

                        var dictionary = _dbContext.LanguageDictionaries.FirstOrDefault(s => s.LanguageId == language.Id);
                        if (dictionary == null && language.Dictionary != null)
                        {
                            dictionary = language.Dictionary;
                            _dbContext.LanguageDictionaries.Add(dictionary);
                        }
                        else if (dictionary != null && language.Dictionary == null)
                        {
                            _dbContext.LanguageDictionaries.Remove(dictionary);
                        }
                        else
                        {
                            dictionary.Dictionary = language.Dictionary.Dictionary;
                            dictionary.DateUpdated = DateTime.Now;
                            _dbContext.LanguageDictionaries.Update(dictionary);
                        }
                        rs.Succeed = _dbContext.SaveChanges() > 0;
                        if (rs.Succeed)
                        {
                            rs.Data = language.Adapt<LanguageViewModel>();
                        }
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

        public ResultModel DeleteLanguage(LanguageViewModel model)
        {
            var rs = new ResultModel();
            try
            {
                var language = _dbContext.Languages.AsSoftDelete(false).FirstOrDefault(s => s.Id == model.Id);
                if (language == null)
                {
                    rs.Succeed = false;
                    rs.ErrorMessage = string.Format("Not found language: {0}", model.Name);
                }
                else
                {
                    language.IsDeleted = true;
                    language.DateUpdated = DateTime.Now;
                    _dbContext.Languages.Update(language);

                    var dictionary = _dbContext.LanguageDictionaries.AsSoftDelete(false).FirstOrDefault(s => s.LanguageId == model.Id);
                    if (dictionary != null)
                    {
                        dictionary.IsDeleted = true;
                        dictionary.DateUpdated = DateTime.Now;
                        _dbContext.LanguageDictionaries.Update(dictionary);
                    }

                    rs.Succeed = _dbContext.SaveChanges() > 0;
                    if (rs.Succeed)
                    {
                        rs.Data = language.Adapt<LanguageViewModel>();
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
     }
}
