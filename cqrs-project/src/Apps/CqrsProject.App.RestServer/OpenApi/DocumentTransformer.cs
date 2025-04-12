using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

public static class DocumentTransformer
{
    public static Func<OpenApiDocument, OpenApiDocumentTransformerContext, CancellationToken, Task> AddInfos(
        ApiVersionDescription description,
        IConfiguration configuration)
        => (document, context, cancellationToken) =>
        {
            var text = new StringBuilder();
            text.Append(configuration.GetValue<string>("OpenApi:Description"));

            if (description.IsDeprecated)
                text.Append("This API version has been deprecated");

            if (description.SunsetPolicy is SunsetPolicy policy)
            {
                if (policy.Date is DateTimeOffset when)
                    text.Append("The API, will be sunset on")
                        .Append(when.Date.ToShortDateString())
                        .Append(".");

                if (policy.HasLinks)
                {
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

            document.Info.Description = text.ToString();
            document.Info.Title = configuration.GetValue<string>("OpenApi:Title");
            document.Info.Version = description.ApiVersion.ToString();

            return Task.CompletedTask;
        };

    public static Func<OpenApiDocument, OpenApiDocumentTransformerContext, CancellationToken, Task> AddSecuritySchemes(
        IConfiguration configuration)
        => (document, context, cancellationToken) =>
        {
            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes.Add(
                JwtBearerDefaults.AuthenticationScheme,
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
                            Scopes = configuration.GetValue<string>("OpenApi:Scopes")!.Split(" ").ToDictionary(x => x)
                        }
                    }
                });

            return Task.CompletedTask;
        };
}
