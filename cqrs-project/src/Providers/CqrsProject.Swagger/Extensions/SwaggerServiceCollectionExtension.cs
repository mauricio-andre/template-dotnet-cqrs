using CqrsProject.Swagger.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.Swagger.Extensions;

public static class SwaggerServiceCollectionExtension
{
    public static IServiceCollection AddSwaggerProvider(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool?>("Scalar:Enable") ?? true)
            return services;

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(type => SwaggerSchemaIdHelper.GetSwaggerSchemaId(type));
        });

        return services;
    }
}
