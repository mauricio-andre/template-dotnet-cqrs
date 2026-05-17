using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestServer.Extensions;

public static class IHeaderDictionaryExtensions
{
    public static IHeaderDictionary AddContentRangeHeaders(
        this IHeaderDictionary headers,
        int? rangeStart,
        int? takeRequired,
        int? size,
        string unit = "items")
    {
        int? endRange = rangeStart ?? 0 + takeRequired;
        headers.Append("Content-Range", string.Concat(
            unit,
            " ",
            rangeStart ?? 0,
            "-",
            (endRange < size ? endRange : size) ?? 0,
            "/",
            size?.ToString() ?? "*"
        ));

        return headers;
    }
}
