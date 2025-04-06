using CqrsProject.Swagger.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.Swagger.Extensions;

public static class SwaggerServiceCollectionExtension
{
    public static IServiceCollection AddSwaggerProvider(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => SwaggerSchemaIdHelper.GetSwaggerSchemaId(type));
        });

        return services;
    }
}
