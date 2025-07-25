using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using Soda.CrossCutting.Configurations;
using Soda.Domain.DomainServices.Authentication.Interfaces.Authentication;
using Soda.Domain.DomainServices.Authentication.Model;

namespace Soda.Domain.DomainServices.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger _logger;
    private readonly AppSettingsConfig _appSettingsConfig;
    private readonly AuthenticationApiClient _client;

    public AuthenticationService(ILogger logger, AppSettingsConfig appSettingsConfig)
    {
        _logger = logger;
        _appSettingsConfig = appSettingsConfig;
        _client = new AuthenticationApiClient(new Uri($"https://{appSettingsConfig.Auth0Config.Domain}/"));
    }

    private async Task<string> GetAccessToken()
    {
        var config = _appSettingsConfig.Auth0Config;
        var client = new RestClient($"https://{config.Domain}/oauth/token");

        var request = new RestRequest();
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_id", config.ClientId);
        request.AddParameter("client_secret", config.ClientSecret);
        request.AddParameter("audience", $"https://{config.Domain}/api/v2/");

        _logger.LogInformation("Requesting access token from Auth0");

        var response = await client.ExecutePostAsync(request);
        var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Auth0AccessToken>(response.Content);

        _logger.LogInformation("Access token retrieved from Auth0");

        return jsonResponse.access_token;
    }

    public async Task<string> SingUpAsync(SignUpModel signUpModel, CancellationToken cancellationToken)
    {
        try
        {
            var user = new SignupUserRequest
            {
                Email = signUpModel.Email,
                Name = signUpModel.Name,
                Password = signUpModel.Password,
                ClientId = _appSettingsConfig.Auth0Config.ClientId,
                Connection = _appSettingsConfig.Auth0Config.Connection,
            };

            _logger.LogInformation("Starting to create user on Auth0 | Method: SingUpAsync | Class: AuthenticationService | Email: {email}", signUpModel.Email);

            var created = await _client.SignupUserAsync(user, cancellationToken);

            _logger.LogInformation("UserAuth created on Auth0 | UserAuthId: {userAuthId}", created?.Id);

            return created is null ? string.Empty : $"auth0|{created.Id}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to create user on Auth0 | Method: SingUpAsync | Class: AuthenticationService");
            return string.Empty;
        }
    }

    public async Task<User> GetUserInfoAsync(string userAuthId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving user info from Auth0 | UserAuthId: {userAuthId}", userAuthId);

        var accessToken = await GetAccessToken();
        var mgmtClient = new ManagementApiClient(accessToken, _appSettingsConfig.Auth0Config.Domain);
        var auth0user = await mgmtClient.Users.GetAsync(userAuthId, cancellationToken: cancellationToken);

        _logger.LogInformation("User info retrieved from Auth0 | UserAuthId: {userAuthId}", userAuthId);

        return auth0user;
    }

    public async Task UpdateUserPasswordAsync(string userAuthId, string password, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user password on Auth0 | UserAuthId: {userAuthId}", userAuthId);

        var accessToken = await GetAccessToken();
        var client = new ManagementApiClient(accessToken, _appSettingsConfig.Auth0Config.Domain);
        var updateTo = new UserUpdateRequest { Password = password };
        await client.Users.UpdateAsync(userAuthId, updateTo, cancellationToken);

        _logger.LogInformation("User password updated on Auth0 | UserAuthId: {userAuthId}", userAuthId);
    }

    public async Task DeleteUserAsync(string userAuthId)
    {
        _logger.LogInformation("Deleting user from Auth0 | UserAuthId: {userAuthId}", userAuthId);

        var accessToken = await GetAccessToken();
        var client = new ManagementApiClient(accessToken, _appSettingsConfig.Auth0Config.Domain);
        await client.Users.DeleteAsync(userAuthId);

        _logger.LogInformation("User deleted from Auth0 | UserAuthId: {userAuthId}", userAuthId);
    }
}