using Auth0.ManagementApi.Models;

namespace Soda.Domain.DomainServices.Authentication.Interfaces.Authentication;

public interface IAuthenticationService
{
    Task<string> SingUpAsync(SignUpModel signUpModel, CancellationToken cancellationToken);
    Task<User> GetUserInfoAsync(string userAuthId, CancellationToken cancellationToken);
    Task UpdateUserPasswordAsync(string userAuthId, string password,CancellationToken cancellationToken);
    Task DeleteUserAsync(string userAuthId);
}

public class SignUpModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}