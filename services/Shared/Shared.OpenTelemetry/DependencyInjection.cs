using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.OpenTelemetry;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(x =>
                x.AddService(configuration["ServiceName"] ?? Assembly.GetEntryAssembly()!.GetName().Name!))
            .WithMetrics(x => x
                .AddRuntimeInstrumentation()
                .AddMeter(
                    "Microsoft.AspNetCore.Hosting",
                    "Microsoft.AspNetCore.Server.Kestrel",
                    "System.Net.Http"
                )
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
            )
            .WithTracing(x => x
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
            );

        var useOtlpExporter = !string.IsNullOrEmpty(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (!useOtlpExporter) return services;
        services.ConfigureOpenTelemetryLoggerProvider(options => options.AddOtlpExporter());
        services.ConfigureOpenTelemetryMeterProvider(options => options.AddOtlpExporter());
        services.ConfigureOpenTelemetryTracerProvider(options => options.AddOtlpExporter());

        return services;
    }

    public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder loggingBuilder)
    {
        return loggingBuilder.AddOpenTelemetry(x =>
        {
            x.IncludeScopes = true;
            x.IncludeFormattedMessage = true;
        });
    }

    public static IEndpointRouteBuilder MapHealthChecks(this IEndpointRouteBuilder builder)
    {
        builder.MapHealthChecks("/health");
        builder.MapHealthChecks("/alive", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
        });
        return builder;
    }
}