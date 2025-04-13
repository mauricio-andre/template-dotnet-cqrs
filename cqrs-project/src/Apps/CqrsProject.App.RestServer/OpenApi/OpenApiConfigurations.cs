using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;

namespace CqrsProject.App.RestServer.OpenApi;

public static class OpenApiConfigurations
{
    public static Action<OpenApiOptions> AddOptions()
        => (options) =>
        {
            options.AddOperationTransformer<HeaderParametersOperationTransformer>();
            options.AddOperationTransformer<DefaultResponseOperationTransformer>();
            options.AddOperationTransformer<MarkObsoleteOperationTransformer>();
            options.AddOperationTransformer<SecurityRequirementOperationTransformer>();
            options.AddDocumentTransformer<SecuritySchemeDocumentTransformer>();
            options.AddDocumentTransformer<InfoDocumentTransformer>();
        };
}
