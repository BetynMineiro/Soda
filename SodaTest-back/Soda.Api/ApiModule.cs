using Microsoft.AspNetCore.Mvc;
using Soda.Api.Filters;
using Soda.Api.Middleware;
using Soda.Application;
using Soda.CrossCutting;
using Soda.Domain;
using Soda.Postgres.EF.Adapter;

namespace Soda.Api;

public static class ApiModule
{
    public static void ConfigureFilters(this MvcOptions options)
    {
        options.Filters.Add<NotificationResultFilter>();
        options.Filters.Add<AcceptHeaderMiddleware>();
        options.Filters.Add(new ProducesAttribute("application/json"));
    }

    private static void ConfigureAppSettingsApi(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(resolver => configuration.GetAppSettingsApiConfig());
    }

    public static void ConfigureApiServicesLayer(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.ConfigureAppSettingsApi(configuration);

        //logs and Metrics
        services.ConfigureLogging(environment);
        services.ConfigureMetrics();

        //dataBases layer
        services.ConfigurePostgresEfAdapterLayer(configuration, environment);
        
        //application domain
        services.ConfigureDomainLayer();
        
        //application layer
        services.ConfigureApplicationLayer();
    }
}