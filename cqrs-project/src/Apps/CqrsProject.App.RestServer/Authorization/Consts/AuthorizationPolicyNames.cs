namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyNames
{
    public const string CanManageSelf = "CanManageSelf";
    public const string CanManageAdministration = "CanManageAdministration";
    public const string CanReadExamples = "CanReadExamples";
    public const string CanManageExamples = "CanManageExamples";
}
