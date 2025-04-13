using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CqrsProject.Swagger.Extensions;

public static class SwaggerWebApplicationExtension
{
    public static WebApplication UseSwaggerProvider(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool?>("Scalar:Enable") ?? true)
            return app;

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

            var clientId = app.Configuration.GetValue<string>("OpenApi:ClientId");
            var clientSecret = app.Configuration.GetValue<string>("OpenApi:ClientSecret");
            var audience = app.Configuration.GetValue<string>("Authentication:Bearer:Audience")!;
            options.OAuthClientId(clientId);
            options.OAuthClientSecret(clientSecret);
            options.OAuthScopes(app.Configuration.GetValue<string>("OpenApi:Scopes")!.Split(" "));
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
