namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyNames
{
    public const string CanReadExamples = "CanReadExamples";
    public const string CanManageExamples = "CanManageExamples";
    public const string CanManageSelf = "CanManageSelf";
}
