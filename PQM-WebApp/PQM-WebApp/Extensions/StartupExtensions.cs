using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using NSwag;
using NSwag.Generation.Processors.Security;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
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
            services.AddTransient<IIndicatorGroupService, IndicatorGroupService>();
            services.AddTransient<IIndicatorService, IndicatorService>();
            services.AddTransient<IUtilsService, UtilsService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IAggregatedValueService, AggregatedValueService>();
            services.AddTransient<ITestingService, TestingService>();
            services.AddTransient<ITreatmentService, TreatmentService>();
            services.AddTransient<IAgeGroupService, AgeGroupService>();
        }

        public static void ConfigDbContext(this IServiceCollection services, string dbConnection)
        {

            services.AddDbContext<AppDBContext>(options =>
                                                       options.UseLazyLoadingProxies().UseSqlServer(dbConnection));
        }

        public static void ConfigSwagger(this IServiceCollection services)
        {
            services.AddOpenApiDocument(document =>
            {
                document.Title = "PQM API";
                document.Version = "1.0";
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        private static void CreateIndex(IElasticClient client)
        {
            var getResponse = client.Indices.Get("indicatorvalue");
            if (!getResponse.IsValid)
            {
                var createIndexResponse = client.Indices.Create("indicatorvalue",
                                                                    index => index.Settings(s => s.Analysis(a => a.Analyzers(aa => aa.Standard("default", sa => sa.StopWords("_none_")))))
                                                                                  .Map<IndicatorElasticModel>(x => x.AutoMap())
                                                            );
            }
        }

        public static void AddElasticsearch(this IServiceCollection services, string url, string username, string password)
        {
            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex("indicatorvalue")
                .BasicAuthentication(username, password)
                .PrettyJson()
                .EnableDebugMode()
                //.ThrowExceptions(alwaysThrow: true)
                .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => { return true; });


            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            CreateIndex(client);
        }
    }
}
