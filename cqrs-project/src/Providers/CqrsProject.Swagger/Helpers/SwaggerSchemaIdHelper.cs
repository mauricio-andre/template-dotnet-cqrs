using System.Reflection;
using CqrsProject.Swagger.Attributes;

namespace CqrsProject.Swagger.Helpers;

public static class SwaggerSchemaIdHelper
{
    public static string GetSwaggerSchemaId(Type type)
    {
        var attribute = type.GetCustomAttribute<SwaggerSchemaIdFilterAttribute>();
        return attribute?.Name ?? type.Name;
    }
}
