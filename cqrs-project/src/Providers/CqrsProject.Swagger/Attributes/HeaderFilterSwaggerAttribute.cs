namespace CqrsProject.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HeaderFilterSwaggerAttribute : Attribute
{
    public string HeaderName { get; }
    public string? Description { get; }
    public string? SchemaType { get; }
    public string? SchemaFormat { get; }
    public bool IsRequired { get; }
    public bool AllowEmptyValue { get; }

    public HeaderFilterSwaggerAttribute(
        string headerName,
        string? description = null,
        string? schemaType = null,
        string? schemaFormat = null,
        bool isRequired = false,
        bool allowEmptyValue = false)
    {
        HeaderName = headerName;
        Description = description;
        SchemaType = schemaType;
        SchemaFormat = schemaFormat;
        IsRequired = isRequired;
        AllowEmptyValue = allowEmptyValue;
    }

}
