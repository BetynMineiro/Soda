
using Soda.Domain.DomainServices.Users;

namespace Soda.Api.Middleware;

public class IdentityMiddleware
{
    private readonly RequestDelegate _next;

    public IdentityMiddleware(RequestDelegate next)
    {
        _next = next;

    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            var userServiceContext = context.RequestServices.GetRequiredService<UserServiceContext>();
            
            var id = context.User.Identity.Name;
            userServiceContext.AddUser(id);
        }

        await _next(context);
    }
}