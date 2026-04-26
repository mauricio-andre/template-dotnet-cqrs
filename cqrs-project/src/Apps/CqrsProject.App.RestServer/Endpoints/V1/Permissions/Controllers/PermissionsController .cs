using System.Reflection;
using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CqrsProject.Core.Identity.Consts;

namespace CqrsProject.App.RestServer.Endpoints.V1.Permissions.Controllers;

[ApiController]
[ApiVersion(1)]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize(Policy = AuthorizationPolicyNames.CanManageAdministration)]
public class PermissionsController : ControllerBase
{

    public PermissionsController()
    {
    }

    [HttpGet]
    [ProducesResponseType<IList<string>>(StatusCodes.Status200OK)]
    public IActionResult Search()
    {
        var list = typeof(IdentityPermissionClaimDefaults)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f =>
                f.IsLiteral
                && !f.IsInitOnly
                && f.FieldType == typeof(string)
                && f.GetRawConstantValue()?.ToString() != IdentityPermissionClaimDefaults.ClaimType)
            .Select(f => f.GetRawConstantValue() as string)
            .Order()
            .ToList();

        return Ok(list);
    }
}
