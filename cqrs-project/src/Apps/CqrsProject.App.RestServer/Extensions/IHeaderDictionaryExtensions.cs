using Microsoft.AspNetCore.Http;

namespace CqrsProject.App.RestServer.Extensions;

public static class IHeaderDictionaryExtensions
{
    public static IHeaderDictionary AddCollectionHeaders(
        this IHeaderDictionary headers,
        int totalCount)
    {
        headers.Append("x-total-count", totalCount.ToString());
        return headers;
    }
}
