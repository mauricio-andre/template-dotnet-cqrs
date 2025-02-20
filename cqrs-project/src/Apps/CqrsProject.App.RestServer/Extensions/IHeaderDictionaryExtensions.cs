using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestServer.Extensions;

public static class IHeaderDictionaryExtensions
{
    public static IHeaderDictionary AddContentRangeHeaders(
        this IHeaderDictionary headers,
        int? rangeStart,
        int? rangeEnd,
        int? size,
        string unit = "items")
    {
        headers.Append("Content-Range", string.Concat(
            unit,
            " ",
            rangeStart ?? 0,
            "-",
            rangeEnd ?? size ?? 0,
            "/",
            size?.ToString() ?? "*"
        ));

        return headers;
    }

    public static IHeaderDictionary AddContentLengthHeaders(
        this IHeaderDictionary headers,
        int length)
    {
        headers.Append("Content-Length", length.ToString());

        return headers;
    }
}
