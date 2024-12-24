namespace CqrsProject.App.RestServer.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class HeaderFilterTenantIdAttribute : HeaderFilterAttribute
{
    public HeaderFilterTenantIdAttribute() : base("Tenant-Id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }
}
