using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class SecurityRequirementOperationTransformer(IConfiguration configuration) : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = context
            .Description
            .ActionDescriptor
            .EndpointMetadata
            .OfType<AuthorizeAttribute>();

        if (!authorizeAttributes.Any())
            return Task.CompletedTask;

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Responses ??= new OpenApiResponses();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    SecuritySchemeType.OAuth2.GetDisplayName(),
                    context.Document)
            ] = configuration.GetValue<string>("OpenApi:Scopes")!.Split(" ").ToList()
        });

        if (!operation.Responses.ContainsKey(StatusCodes.Status401Unauthorized.ToString()))
            operation.Responses.Add(
                StatusCodes.Status401Unauthorized.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });

        var hasPermission = authorizeAttributes
            .Any(authorize => authorize.Policy != null
                || authorize.Roles != null
                || authorize.AuthenticationSchemes != null);

        if (hasPermission && !operation.Responses.ContainsKey(StatusCodes.Status403Forbidden.ToString()))
            operation.Responses.Add(
                StatusCodes.Status403Forbidden.ToString(),
                new OpenApiResponse { Description = HttpStatusCode.Forbidden.ToString() });

        return Task.CompletedTask;
    }
}
