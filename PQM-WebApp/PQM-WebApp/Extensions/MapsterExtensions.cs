using Mapster;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Models;
using PQM_WebApp.Data.Utils;
using PQM_WebApp.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PQM_WebApp.Extensions
{
    public static class MapsterExtensions
    {
        public static void UseMapsterConfig(this IApplicationBuilder app)
        {
            // kế thừa mapping
            TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
            #region Exception
            TypeAdapterConfig.GlobalSettings.ForType<Exception, ResultModel>()
                                                .Map(dest => dest.Succeed, src => false)
                                                .Map(dest => dest.Error.ErrorMessage, 
                                                        src => src.InnerException != null ? src.InnerException.Message : src.Message
                                                        + Environment.NewLine
                                                        + src.StackTrace);
            // Language mapping
            TypeAdapterConfig.GlobalSettings.ForType<Language, LanguageViewModel>()
                                                .Map(dest => dest.Dictionary, src => JsonConvert.DeserializeObject<IEnumerable<LanguageDictionaryViewModel>>(src.Dictionary != null ? src.Dictionary.Dictionary : "[]"));
            TypeAdapterConfig.GlobalSettings.ForType<LanguageCreateModel, Language>()
                                                .Map(dest => dest.Dictionary, src => ConvertUtil.ConvertToDictionary(src.Dictionary));
            #endregion
        }
    }
}
