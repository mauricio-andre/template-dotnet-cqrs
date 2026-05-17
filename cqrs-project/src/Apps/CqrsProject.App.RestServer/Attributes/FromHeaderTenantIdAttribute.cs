using Microsoft.OpenApi;

namespace CqrsProject.App.RestServer.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class FromHeaderTenantIdAttribute : FromHeaderCustomAttribute
{
    public FromHeaderTenantIdAttribute() : base("Tenant-Id", "Tenant Id in standard uuid format", JsonSchemaType.String, "uuid", true)
    {
    }
}
