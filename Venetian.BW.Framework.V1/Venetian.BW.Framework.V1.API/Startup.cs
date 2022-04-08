using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.CommonUtility.HttpPollyExtensions;
using Venetian.BW.Framework.V1.CommonUtility.KafkaUtility;

namespace Venetian.BW.Framework.V1.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            services.AddControllers();
            //Version Control
            services.AddApiVersioning(c =>
            {
                c.DefaultApiVersion = new ApiVersion(1, 0);

            });
            services.AddVersionedApiExplorer(c =>
            {
                c.GroupNameFormat = "'v'VVV";
                c.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.IgnoreObsoleteActions();

                //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "venetian_bw_api.xml");
                //c.IncludeXmlComments(filePath);
            });
            //Add Polly HttpClient Utility
            services.AddSingleton<BwHttpClient>();

            services.Configure<AppSetting>(Configuration.GetSection("Setting"));


            //Custome Config for HttpClient Polly
            services.AddPollyHttpClient("Service1", options => {
                options.TimeoutTime = 60; 
                options.RetryCount = 3;
                options.CircuitBreakerOpenFallCount = 2;
                options.CircuitBreakerDownTime = 100;
            });
            services.AddPollyHttpClient("Service2", options => {
                options.TimeoutTime = 60; 
                options.RetryCount = 3;
                options.CircuitBreakerOpenFallCount = 2;
                options.CircuitBreakerDownTime = 100;
            });

            //register kafka service to DI container
            services.AddSingleton<IKafkaService, KafkaService>();

            //health Check
            services.AddHealthChecks()
           .AddSqlServer(Configuration["Setting:DbConnection"]);
     
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                    c.DocumentTitle = "Venetian BW Swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //http请求的中间件            
            app.UseHttpMetrics();

            app.UseAuthorization();

            // default endpoint: /healthmetrics
            app.UseHealthChecks("/healthcheck");

            app.UseEndpoints(endpoints =>
            {
                //映射监控地址为  /metrics                
                endpoints.MapMetrics();
                endpoints.MapControllers();
            });
        }

        public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            readonly IApiVersionDescriptionProvider provider;

            public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
              this.provider = provider;

            public void Configure(SwaggerGenOptions options)
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                      description.GroupName,
                        new OpenApiInfo()
                        {
                            Title = $"Venetian BW API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString(),
                        });
                }
            }
        }
    }
}
