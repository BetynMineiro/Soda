using Microsoft.Extensions.DependencyInjection;
using Soda.Application.ApplicationServices.NotificationService;
using Soda.Application.ApplicationServices.Services.Employers;

namespace Soda.Application;

public static class ApplicationModule
{
    public static void ConfigureApplicationLayer(this IServiceCollection services)
    {
        //Notification
        services.AddScoped<NotificationServiceContext>()
            ;
        //Services
        services.AddScoped<IEmployerService, EmployerService>();
    }
}