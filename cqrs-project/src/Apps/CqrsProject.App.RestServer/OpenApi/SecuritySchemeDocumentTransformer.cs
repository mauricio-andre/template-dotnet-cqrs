using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class SecuritySchemeDocumentTransformer(IConfiguration configuration) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes.Add(
            SecuritySchemeType.OAuth2.GetDisplayName(),
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration.GetValue<string>("OpenApi:AuthorizationUrl")!),
                        TokenUrl = new Uri(configuration.GetValue<string>("OpenApi:TokenUrl")!),
                        RefreshUrl = new Uri(configuration.GetValue<string>("OpenApi:RefreshTokenUrl")!),
                        Scopes = configuration.GetValue<string>("OpenApi:Scopes")!.Split(" ").ToDictionary(x => x),
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            { "x-usePkce", new OpenApiString("SHA-256") }
                        }
                    }
                }
            });

        document.Components.SecuritySchemes.Add(
            SecuritySchemeType.OpenIdConnect.GetDisplayName(),
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri(configuration.GetValue<string>("OpenApi:OpenIdConnectUrl")!)
            });

        return Task.CompletedTask;
    }
}
