using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Soda.CrossCutting;
using Soda.Domain.Repositories;
using Soda.Postgres.EF.Adapter.Context;
using Soda.Postgres.EF.Adapter.Repository;
using Soda.Postgres.EF.Adapter.UnitOfWork;

namespace Soda.Postgres.EF.Adapter;

public static class PostgresEfAdapterModule
{
    public static void ConfigurePostgresEfAdapterLayer(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var appSettings = configuration.GetAppSettingsApiConfig();

        // context
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(appSettings.PostgresConfig.Connection));

        // repositories
        services.AddScoped<IEmployerRepository, EmployerRepository>();

        // unitOfwork
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
    }
}