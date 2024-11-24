namespace CqrsProject.App.RestServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SwaggerHeaderAttribute : Attribute
{
    public string HeaderName { get; }
    public string? Description { get; }
    public string? SchemaType { get; }
    public bool IsRequired { get; }
    public bool AllowEmptyValue { get; }

    public SwaggerHeaderAttribute(
        string headerName,
        string? description = null,
        string? schemaType = null,
        bool isRequired = false,
        bool allowEmptyValue = false)
    {
        HeaderName = headerName;
        Description = description;
        SchemaType = schemaType;
        IsRequired = isRequired;
        AllowEmptyValue = allowEmptyValue;
    }
}
