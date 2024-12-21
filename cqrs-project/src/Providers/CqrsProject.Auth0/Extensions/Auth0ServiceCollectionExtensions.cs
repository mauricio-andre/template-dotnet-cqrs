using CqrsProject.Auth0.Options;
using CqrsProject.Auth0.Services;
using CqrsProject.Common.Providers.OAuth.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Auth0.Extensions;

public static class Auth0ServiceCollectionExtensions
{
    public static IServiceCollection AddAuth0Provider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IOAuthService, Auth0Service>();
        services.Configure<Auth0Options>(configuration.GetSection(Auth0Options.Position));
        return services;
    }
}
