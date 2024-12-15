using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace CqrsProject.App.RestServer.Transformers;

public class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null)
            return null;

        return Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
