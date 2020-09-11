using System;
using System.Net;
using System.Net.Http;
using ApplicationInsightsTelemetryEnhancer22;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication22
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
            services.AddHttpClient("StackOverflowClient", options =>
            {
                options.BaseAddress = new Uri("https://api.stackexchange.com/");
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            //It is not required to add the OperationIdHeader service. Without it, the header will be the default "Operation-Id"

            //You can set a custom header by adding it to your appsettings.json file, and sending the configuration section to the AddOperationIdHeader extension method:
            //services.AddOperationIdHeader(Configuration.GetSection("OperationIdHeader"));

            //You can set a custom header by hard coding it with the options action:
            //services.AddOperationIdHeader(options => 
            //{
            //    options.HeaderName = "MyHeader";
            //});

            //To add request + response body to dependencies with default values, use AddDependencyTelemetryEnhancer without any parameters
            //services.AddDependencyTelemetryEnhancer();

            //To add request + response body to dependencies with custom keys, you can use the options action.
            //services.AddDependencyTelemetryEnhancer(options =>
            //{
            //    options.RequestPropertyKey = "RequestOptions";
            //    options.ResponsePropertyKey = "ResponseOptions";
            //});

            //To add request + response body to dependencies with custom keys, you can use a cunfiguration section.
            services.AddDependencyTelemetryEnhancer(Configuration.GetSection("DependencyTelemetryEnhancer"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Add this line to add Application Insights Operation Id as a response header to your app
            app.UseOperationIdHeader();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
