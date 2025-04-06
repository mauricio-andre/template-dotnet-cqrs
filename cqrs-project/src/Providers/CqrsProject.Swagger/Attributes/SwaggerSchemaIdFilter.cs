namespace CqrsProject.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerSchemaIdFilterAttribute : Attribute
{
    public string Name { get; }

    public SwaggerSchemaIdFilterAttribute(string name)
    {
        Name = name;
    }
}
