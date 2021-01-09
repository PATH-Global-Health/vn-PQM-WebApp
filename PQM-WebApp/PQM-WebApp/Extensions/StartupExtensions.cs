using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PQM_WebApp.Data;
using PQM_WebApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PQM_WebApp.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IPrEPService, PrEPService>();
        }

        public static void ConfigDbContext(this IServiceCollection services, string dbConnection)
        {

            services.AddDbContext<AppDBContext>(options =>
                                                       options.UseLazyLoadingProxies().UseSqlServer(dbConnection));
        }

    }
}
