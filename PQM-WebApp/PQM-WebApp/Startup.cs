using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PQM_WebApp.Extensions;

namespace PQM_WebApp
{
    public class Startup
    {
        public Startup()
        {
            var configuration = new ConfigurationBuilder()
                              .AddJsonFile("appsettings.json")
                              .AddEnvironmentVariables()
                              .Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddBusinessServices();
            services.ConfigDbContext(Configuration["ConnectionStrings:DbConnection"]);
            services.ConfigJwt(Configuration["Jwt:Key"], Configuration["Jwt:Issuer"], Configuration["Jwt:Issuer"]);
            services.ConfigSwagger();
            services.AddElasticsearch(Configuration["elasticsearch:url"], Configuration["elasticsearch:username"], Configuration["elasticsearch:password"], Configuration["elasticsearch:index"]);
            services.ConfigCors();

            Console.WriteLine("PQM-Core");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors("AllowAll");
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
            });

            app.UseMapsterConfig();
        }
    }
}
