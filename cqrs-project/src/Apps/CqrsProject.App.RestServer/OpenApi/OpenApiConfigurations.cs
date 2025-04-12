using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;

namespace CqrsProject.App.RestServer.OpenApi;

public static class OpenApiConfigurations
{
    public static Action<OpenApiOptions> AddOptions(
        ApiVersionDescription description,
        IConfiguration configuration)
        => (options) =>
        {
            options.AddOperationTransformer(OperationTransformer.AddHeaderParameters());
            options.AddOperationTransformer(OperationTransformer.AddDefaultResponse());
            options.AddOperationTransformer(OperationTransformer.AddMarkObsolete());
            options.AddOperationTransformer(OperationTransformer.AddSecurityRequirement(configuration));
            options.AddDocumentTransformer(DocumentTransformer.AddSecuritySchemes(configuration));
            options.AddDocumentTransformer(DocumentTransformer.AddInfos(description, configuration));
        };
}
