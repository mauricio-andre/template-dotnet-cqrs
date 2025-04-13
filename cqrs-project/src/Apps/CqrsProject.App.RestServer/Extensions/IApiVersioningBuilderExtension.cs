using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CqrsProject.App.RestServer.OpenApi;

namespace CqrsProject.App.RestServer.Extensions;

public static class IApiVersioningBuilderExtension
{
    public static IApiVersioningBuilder AddOpenApiVersions(
        this IApiVersioningBuilder apiVersion,
        IServiceCollection services)
    {
        using (var tempProvider = services.BuildServiceProvider())
        {
            var versionProvider = tempProvider.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in versionProvider.ApiVersionDescriptions)
            {
                services.AddOpenApi(
                    description.GroupName,
                    OpenApiConfigurations.AddOptions());
            }
        }

        return apiVersion;
    }
}
