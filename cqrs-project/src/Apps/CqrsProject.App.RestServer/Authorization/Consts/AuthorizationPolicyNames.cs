namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyNames
{
    public const string HasRoleHostAdmin = "HasRoleHostAdmin";
    public const string HasRoleTenantAdmin = "HasRoleTenantAdmin";
    public const string HasRoleClient = "HasRoleClient";
}
