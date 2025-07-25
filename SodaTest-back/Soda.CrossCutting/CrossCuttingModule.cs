using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Soda.CrossCutting.Configurations;

namespace Soda.CrossCutting;

public static class CrossCuttingModule
{
    public static AppSettingsConfig GetAppSettingsApiConfig(this IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection("Soda");
        var settingsApi = settingsSection.Get<AppSettingsConfig>();
        return settingsApi;
    }

    public static void ConfigureLogging(this IServiceCollection services, IHostEnvironment environment)
    {
        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownApp";
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Is(LogEventLevel.Information)
            .WriteTo.OpenTelemetry(options =>
            {
                options.Protocol = Serilog.Sinks.OpenTelemetry.OtlpProtocol.Grpc;
                options.ResourceAttributes["service.name"] = applicationName;
                options.ResourceAttributes["deployment.environment"] = environment.EnvironmentName;
            })
            .CreateLogger();

        services.AddSingleton(Log.Logger);
    }

    public static void ConfigureMetrics(this IServiceCollection services)
    {
        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownApp";

        services.AddOpenTelemetry()
            .WithTracing(tracer =>
            {
                tracer
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: applicationName))
                    .AddOtlpExporter(otlp => { otlp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(applicationName))
                    .AddOtlpExporter(otlp => { otlp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc; });
            });
    }
}