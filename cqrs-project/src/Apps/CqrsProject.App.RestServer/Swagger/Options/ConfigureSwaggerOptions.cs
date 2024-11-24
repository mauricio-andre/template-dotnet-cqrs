using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CqrsProject.App.RestService.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IConfiguration _configuration;


    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(_configuration.GetValue<string>("PlatformSwagger:AuthorizationUrl")!),
                    TokenUrl = new Uri(_configuration.GetValue<string>("PlatformSwagger:TokenUrl")!),
                    RefreshUrl = new Uri(_configuration.GetValue<string>("PlatformSwagger:RefreshTokenUrl")!),
                    Scopes = _configuration.GetValue<string>("PlatformSwagger:Scopes")!.Split(" ").ToDictionary(x => x)
                }
            }
        });

        options.OperationFilter<AuthorizeOperationFilter>();
        options.OperationFilter<AttributeHeaderOperationFilter>();
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder();
        var info = new OpenApiInfo()
        {
            Title = _configuration.GetValue<string>("PlatformSwagger:ApiName"),
            Version = description.ApiVersion.ToString()
        };

        text.Append(_configuration.GetValue<string>("PlatformSwagger:ApiDescription"));
        AddInfoDeprecated(text, description);
        AddInfoSunsetPolicy(text, description);

        info.Description = text.ToString();

        return info;
    }

    private static void AddInfoDeprecated(StringBuilder text, ApiVersionDescription description)
    {
        if (description.IsDeprecated)
            text.Append("This API version has been deprecated");
    }

    private static void AddInfoSunsetPolicy(StringBuilder text, ApiVersionDescription description)
    {
        if (description.SunsetPolicy is SunsetPolicy policy)
        {
            AddInfoSunsetDate(text, policy);
            AddInfoSunsetLinks(text, policy);
        }
    }

    private static void AddInfoSunsetDate(StringBuilder text, SunsetPolicy policy)
    {
        if (policy.Date is DateTimeOffset when)
            text.Append("The API, will be sunset on")
                .Append(when.Date.ToShortDateString())
                .Append(".");
    }
    private static void AddInfoSunsetLinks(StringBuilder text, SunsetPolicy policy)
    {
        if (!policy.HasLinks)
            return;

        text.AppendLine();
        for (int i = 0; i < policy.Links.Count; i++)
        {
            var link = policy.Links[i];

            if (link.Type != "text/html")
                continue;

            if (link.Title.HasValue)
                text.Append(link.Title.Value).Append(": ");

            text.Append(link.LinkTarget.OriginalString);
        }
    }
}
