using System.Reflection;
using CqrsProject.App.RestServer.Filters;

namespace CqrsProject.App.RestServer.Helpers;

public static class SwaggerSchemaIdHelper
{
    public static string GetSwaggerSchemaId(Type type)
    {
        var attribute = type.GetCustomAttribute<SwaggerSchemaIdFilterAttribute>();
        return attribute?.Name ?? type.Name;
    }
}
