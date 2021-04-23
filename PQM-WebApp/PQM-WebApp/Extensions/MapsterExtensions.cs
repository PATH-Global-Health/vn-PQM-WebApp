using Mapster;
using Microsoft.AspNetCore.Builder;
using PQM_WebApp.Data.Models;
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
            #endregion
        }
    }
}
