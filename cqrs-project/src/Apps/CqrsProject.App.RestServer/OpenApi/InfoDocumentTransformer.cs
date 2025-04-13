using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace CqrsProject.App.RestServer.OpenApi;

internal sealed class InfoDocumentTransformer(
    IConfiguration configuration,
    IApiVersionDescriptionProvider versionProvider) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var description = versionProvider.ApiVersionDescriptions
            .FirstOrDefault(x => x.GroupName == context.DocumentName);

        if (description == null)
            return Task.CompletedTask;

        var text = new StringBuilder();
        text.Append(configuration.GetValue<string>("OpenApi:Description"));
        AddInfoDeprecated(text, description);
        AddInfoSunsetPolicy(text, description);

        document.Info.Description = text.ToString();
        document.Info.Title = configuration.GetValue<string>("OpenApi:Title");
        document.Info.Version = description.ApiVersion.ToString();

        return Task.CompletedTask;
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
