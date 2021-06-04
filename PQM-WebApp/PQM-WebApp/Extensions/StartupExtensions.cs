using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PQM_WebApp.Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigJwt(this IServiceCollection services, string key, string issuer, string audience)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwtconfig =>
                {
                    jwtconfig.SaveToken = true;
                    jwtconfig.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        RequireSignedTokens = true,
                        ValidIssuer = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidAudience = string.IsNullOrEmpty(audience) ? issuer : audience,
                    };

                });
        }

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
            services.AddTransient<IThresholdSettingService, ThresholdSettingService>();
            services.AddTransient<IAgeGroupService, AgeGroupService>();
            services.AddTransient<IKeyPopulationService, KeyPopulationService>();
            services.AddTransient<IGenderService, GenderService>();
            services.AddTransient<IIndicatorGroupService, IndicatorGroupService>();
            services.AddTransient<ISiteTypeService, SiteTypeService>();
            services.AddTransient<ICategoryAliasService, CategoryAliasService>();
            services.AddTransient<IDataPermissionService, DataPermissionService>();
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

        private static void CreateIndex(IElasticClient client, string index)
        {
            var getResponse = client.Indices.Get(index);
            if (!getResponse.IsValid)
            {
                var createIndexResponse = client.Indices
                    .Create(index,
                            index => index.Settings(s => s.Analysis(a => a.Analyzers(aa => aa.Standard("default", sa => sa.StopWords("_none_")))))
                                            .Map<IndicatorElasticModel>(_ => _.AutoMap()
                                                                              .Properties(ps => ps.GeoPoint(s => s.Name(n => n.Location))
                                                                                                  .Date(s => s.Name(n => n.Date))
                                                                              )
                                                                       )
                    );
            }
        }

        public static void AddElasticsearch(this IServiceCollection services, string url, string username, string password, string index)
        {
            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(index)
                .BasicAuthentication(username, password)
                .PrettyJson()
                .EnableDebugMode()
                //.ThrowExceptions(alwaysThrow: true)
                .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => { return true; });


            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            CreateIndex(client, index);
        }

        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        }
    }
}
