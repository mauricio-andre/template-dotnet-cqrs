using System.Reflection;
using CqrsProject.Swagger.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.Swagger.Extensions;

public static class SwaggerWebApplicationExtension
{
    public static WebApplication UseSwaggerProvider(this WebApplication app, ConfigurationManager configuration)
    {
        var assembly = typeof(SwaggerWebApplicationExtension).GetTypeInfo().Assembly;

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new ManifestEmbeddedFileProvider(assembly, "wwwroot"),
        });

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptionList = app.DescribeApiVersions();
            foreach (var groupName in descriptionList.Select(desc => desc.GroupName))
            {
                var url = $"/swagger/{groupName}/swagger.json";
                var name = groupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }

            var clientId = configuration.GetValue<string>("PlatformSwagger:ClientId");
            var clientSecret = configuration.GetValue<string>("PlatformSwagger:ClientSecret");
            var audience = configuration.GetValue<string>("Authentication:Bearer:Audience")!;
            options.OAuthClientId(clientId);
            options.OAuthClientSecret(clientSecret);
            options.OAuthScopes(configuration.GetValue<string>("PlatformSwagger:Scopes")!.Split(" "));
            options.OAuthUsePkce();
            options.OAuthAdditionalQueryStringParams(new Dictionary<string, string>()
            {
                {"audience", audience}
            });

            options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            options.InjectJavascript("/swagger-ui/SwaggerRefreshToken.js");
        });

        return app;
    }
}
