using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationInsightsTelemetryEnhancer50
{
    public class RequestTelemetryEnhancer
    {
        private readonly RequestDelegate _next;
        private readonly RequestTelemetryEnhancerOptions _options;

        public RequestTelemetryEnhancer(RequestDelegate next, IOptions<RequestTelemetryEnhancerOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            RequestTelemetry requestTelemetry = httpContext.Features.Get<RequestTelemetry>();
            string requestBody = await GetRequestBody(httpContext);
            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                requestTelemetry.Properties.Add(_options.RequestPropertyKey, requestBody);
            }
            string responseBody = await GetResponseBody(httpContext);
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                requestTelemetry.Properties.Add(_options.ResponsePropertyKey, responseBody);
            }
        }

        private async Task<string> GetRequestBody(HttpContext httpContext)
        {
            try
            {
                httpContext.Request.EnableBuffering();
                using StreamReader reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true);
                string requestBody = await reader.ReadToEndAsync();
                httpContext.Request.Body.Position = 0;
                return requestBody;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> GetResponseBody(HttpContext httpContext)
        {
            Stream originalBody = httpContext.Response.Body;
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                httpContext.Response.Body = memoryStream;
                await _next(httpContext);
                if (httpContext.Response.StatusCode == StatusCodes.Status204NoContent)
                {
                    return null;
                }
                memoryStream.Position = 0;
                using StreamReader streamReader = new StreamReader(memoryStream);
                string responseBody = await streamReader.ReadToEndAsync();
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
                return responseBody;
            }
            catch
            {
                return null;
            }
            finally
            {
                httpContext.Response.Body = originalBody;
            }
        }
    }

    public static class RequestTelemetryEnhancerExtension
    {
        public static IServiceCollection AddRequestTelemetryEnhancer(this IServiceCollection services, Action<RequestTelemetryEnhancerOptions> options = default)
        {
            options ??= (opts => new RequestTelemetryEnhancerOptions());
            services.Configure(options);
            return services;
        }

        public static IServiceCollection AddRequestTelemetryEnhancer(this IServiceCollection services, IConfiguration configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }
            services.Configure<RequestTelemetryEnhancerOptions>(configurationSection);
            return services;
        }

        public static IApplicationBuilder UseRequestTelemetryEnhancer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTelemetryEnhancer>();
        }
    }

    public class RequestTelemetryEnhancerOptions
    {
        public string RequestPropertyKey { get; set; } = "Request";
        public string ResponsePropertyKey { get; set; } = "Response";
    }
}
