namespace CqrsProject.App.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class FromHeaderFilterTenantIdAttribute : FromHeaderFilterAttribute
{
    public FromHeaderFilterTenantIdAttribute() : base("Tenant-Id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }
}
