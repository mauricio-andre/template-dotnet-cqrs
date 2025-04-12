namespace CqrsProject.Swagger.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class HeaderFilterSwaggerTenantIdAttribute : HeaderFilterSwaggerAttribute
{
    public HeaderFilterSwaggerTenantIdAttribute() : base("Tenant-Id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }
}
