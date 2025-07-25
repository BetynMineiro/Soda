using Microsoft.Extensions.DependencyInjection;
using Soda.Domain.DomainServices.Authentication.Interfaces.Authentication;
using Soda.Domain.DomainServices.Authentication.Services;
using Soda.Domain.DomainServices.Users;
using Soda.Domain.Specification;
using Soda.Domain.Specification.Authentication;
using Soda.Domain.Validators;
using Soda.Domain.Validators.Authentication;

namespace Soda.Domain;

public static class DomainModule
{
    public static void ConfigureDomainLayer(this IServiceCollection services)
    {
        // Specifications
        services.AddScoped<ICreateEmployerSpecification, CreateEmployerSpecification>();
        services.AddScoped<ISignUpSpecification, SignUpSpecification>();
        services.AddScoped<IUpdatePasswordSpecification, UpdatePasswordSpecification>();
        //Validators
        services.AddScoped<IIsValidEmployerValidator, IsValidEmployerValidator>();

        //Services
        services.AddScoped<IIsValidSignUpValidator, IsValidSignUpValidator>();
        services.AddScoped<IIsValidUpdatePasswordValidator, IsValidUpdatePasswordValidator>();
        services.AddScoped<UserServiceContext>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}