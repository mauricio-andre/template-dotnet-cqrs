namespace CqrsProject.App.RestServer.Filters;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerSchemaIdFilterAttribute : Attribute
{
    public string Name { get; }

    public SwaggerSchemaIdFilterAttribute(string name)
    {
        Name = name;
    }
}
