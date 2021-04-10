using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ApplicationInsightsTelemetryEnhancer50
{
    public class OperationIdHeader
    {
        private readonly RequestDelegate _next;
        private readonly OperationIdHeaderOptions _options;

        public OperationIdHeader(RequestDelegate next, IOptions<OperationIdHeaderOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Response.OnStarting(async state =>
            {
                if (state is HttpContext completedContext)
                {
                    completedContext.Response.Headers.Add(_options.HeaderName, System.Diagnostics.Activity.Current?.RootId);
                    await Task.CompletedTask;
                }
            }, httpContext);
            await _next(httpContext);
        }
    }

    public static class OperationIdHeaderExtension
    {
        public static IServiceCollection AddOperationIdHeader(this IServiceCollection services, Action<OperationIdHeaderOptions> options = default)
        {
            options ??= (opts => new OperationIdHeaderOptions());
            services.Configure(options);
            return services;
        }

        public static IServiceCollection AddOperationIdHeader(this IServiceCollection services, IConfiguration configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }
            services.Configure<OperationIdHeaderOptions>(configurationSection);
            return services;
        }

        public static IApplicationBuilder UseOperationIdHeader(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OperationIdHeader>();
        }
    }

    public class OperationIdHeaderOptions
    {
        public string HeaderName { get; set; } = "Operation-Id";
    }
}
