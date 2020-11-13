using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationInsightsTelemetryEnhancer50;

namespace WebApplication50
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

            services.AddControllers();
            services.AddHttpClient("StackOverflowClient", options =>
            {
                options.BaseAddress = new Uri("https://api.stackexchange.com/");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication50", Version = "v1" });
            });

            //You must add Application Insights Telemetry seperately from the other features.
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

            //It is not required to add the OperationIdHeader service. Without it, the header will be the default "Operation-Id"

            //You can set a custom header by adding it to your appsettings.json file, and sending the configuration section to the AddOperationIdHeader extension method:
            //services.AddOperationIdHeader(Configuration.GetSection("OperationIdHeader"));

            //You can set a custom header by hard coding it with the options action:
            //services.AddOperationIdHeader(options =>
            //{
            //    options.HeaderName = "HeaderOptions";
            //});

            //To add request + response body to dependencies with default values, use AddDependencyTelemetryEnhancer without any parameters
            services.AddDependencyTelemetryEnhancer();

            //To add request + response body to dependencies with custom keys, you can use a cunfiguration section.
            //services.AddDependencyTelemetryEnhancer(Configuration.GetSection("DependencyTelemetryEnhancer"));

            //To add request + response body to dependencies with custom keys, you can use the options action.
            //services.AddDependencyTelemetryEnhancer(options =>
            //{
            //    options.RequestPropertyKey = "RequestOptions";
            //    options.ResponsePropertyKey = "ResponseOptions";
            //});

            //It is not required to add the RequestTelemetryEnhancer service. Without it, the request and response keys will be the default "Request" and "Response"

            //To set custom keys to your request telemetry, you can use a configuration section.
            //services.AddRequestTelemetryEnhancer(Configuration.GetSection("RequestTelemetryEnhancer"));

            //To set custom keys to your request telemetry, you can use the options action.
            //services.AddRequestTelemetryEnhancer(options =>
            //{
            //    options.RequestPropertyKey = "RequestOptions";
            //    options.ResponsePropertyKey = "ResponseOptions";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication50 v1"));
            }

            //Add this line to add Application Insights Operation Id as a response header to your app. You can customize it in the ConfigureServices method.
            app.UseOperationIdHeader();

            //Add this line to add the request and response body to your request telemetry.You can customize it in the ConfigureServices method.
            app.UseRequestTelemetryEnhancer();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
