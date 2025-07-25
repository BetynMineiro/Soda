using System.IO.Compression;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Soda.Api;
using Soda.Api.Filters;
using Soda.Api.Middleware;
using Soda.CrossCutting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddControllers(p => p.ConfigureFilters())
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
    });

builder.Services.ConfigureApiServicesLayer(builder.Configuration, builder.Environment);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://127.0.0.1")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var appSettings = builder.Configuration.GetAppSettingsApiConfig();

builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<SwaggerSchemaFilter>();
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"https://{appSettings.Auth0Config.Domain}/oauth/token"),
                AuthorizationUrl = new Uri($"https://{appSettings.Auth0Config.Domain}/authorize?audience={appSettings.Auth0Config.Audience}"),
                Scopes = new Dictionary<string, string> { { "openid", "OpenId" } }
            }
        }
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" } }, new[] { "openid" }
        }
    });
});

var domain = $"https://{appSettings.Auth0Config.Domain}/";
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // If the access token does not have a `sub` claim, `UserAuth.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
        options.Authority = domain;
        options.Audience = appSettings.Auth0Config.Audience;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier,
            ValidateIssuer = true,
            ValidIssuer = domain,
            ValidateAudience = true,
            ValidAudience = appSettings.Auth0Config.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "roles",
        };
    });


builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(settings =>
    {
        settings.SwaggerEndpoint("/swagger/v1/swagger.json", "Soda Web API v1.0");
        settings.OAuthClientId(appSettings.Auth0Config.ClientId);
        settings.OAuthClientSecret(appSettings.Auth0Config.ClientSecret);
        settings.OAuthUsePkce();
    });
    app.UseCors("AllowLocalhost");
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<IdentityMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();