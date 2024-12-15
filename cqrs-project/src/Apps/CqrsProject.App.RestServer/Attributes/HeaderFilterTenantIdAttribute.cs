namespace CqrsProject.App.RestServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class HeaderFilterTenantIdAttribute : HeaderFilterAttribute
{
    public HeaderFilterTenantIdAttribute() : base("x-tenant-id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }
}
