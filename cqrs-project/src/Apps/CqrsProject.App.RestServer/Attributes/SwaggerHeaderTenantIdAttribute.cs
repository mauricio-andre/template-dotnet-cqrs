namespace CqrsProject.App.RestServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SwaggerHeaderTenantIdAttribute : SwaggerHeaderAttribute
{
    public SwaggerHeaderTenantIdAttribute() : base("x-tenant-id", "Tenant Id in standard uuid format", "Guid")
    {
    }
}
