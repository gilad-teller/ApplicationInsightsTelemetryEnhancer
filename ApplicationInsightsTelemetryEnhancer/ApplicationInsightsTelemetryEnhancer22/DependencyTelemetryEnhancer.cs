using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ApplicationInsightsTelemetryEnhancer22
{
    public class DependencyTelemetryEnhancer : ITelemetryInitializer
    {
        private DependencyTelemetryEnhancerOptions _options;

        public DependencyTelemetryEnhancer(IOptions<DependencyTelemetryEnhancerOptions> options)
        {
            _options = options.Value;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is DependencyTelemetry dependencyTelemetry && dependencyTelemetry.TryGetOperationDetail("HttpResponse", out object reponseObject))
            {
                HttpResponseMessage response = reponseObject as HttpResponseMessage;
                HttpContent content = response.RequestMessage.Content;
                if (content != null)
                {
                    string request = content.ReadAsStringAsync().Result;
                    dependencyTelemetry.Properties.Add(_options.RequestPropertyKey, request);
                }
                string dataResposne = response?.Content?.ReadAsStringAsync().Result;
                if (!string.IsNullOrWhiteSpace(dataResposne))
                {
                    dependencyTelemetry.Properties.Add(_options.ResponsePropertyKey, dataResposne);
                }
            }
        }
    }

    public static class DependencyTelemetryEnhancerExtension
    {
        public static IServiceCollection AddDependencyTelemetryEnhancer(this IServiceCollection services, Action<DependencyTelemetryEnhancerOptions> options = default)
        {
            options = options ?? (opts => new DependencyTelemetryEnhancerOptions());
            services.Configure(options);
            return services.AddSingleton<ITelemetryInitializer, DependencyTelemetryEnhancer>();
        }

        public static IServiceCollection AddDependencyTelemetryEnhancer(this IServiceCollection services, IConfiguration configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }
            services.Configure<DependencyTelemetryEnhancerOptions>(configurationSection);
            return services.AddSingleton<ITelemetryInitializer, DependencyTelemetryEnhancer>();
        }
    }

    public class DependencyTelemetryEnhancerOptions
    {
        public string RequestPropertyKey { get; set; } = "Request";
        public string ResponsePropertyKey { get; set; } = "Response";
    }
}
