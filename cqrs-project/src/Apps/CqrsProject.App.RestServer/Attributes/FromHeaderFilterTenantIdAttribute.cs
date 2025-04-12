namespace CqrsProject.App.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class FromHeaderTenantIdAttribute : FromHeaderCustomAttribute
{
    public FromHeaderTenantIdAttribute() : base("Tenant-Id", "Tenant Id in standard uuid format", "string", "uuid", true)
    {
    }
}
