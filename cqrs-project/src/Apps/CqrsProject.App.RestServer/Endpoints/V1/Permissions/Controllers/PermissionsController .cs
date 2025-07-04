using System.Reflection;
using Asp.Versioning;
using CqrsProject.App.RestServer.Authorization;
using CqrsProject.Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        var list = typeof(AuthorizationPermissionClaims)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f =>
                f.IsLiteral
                && !f.IsInitOnly
                && f.FieldType == typeof(string)
                && f.GetRawConstantValue()?.ToString() != AuthorizationPermissionClaims.ClaimType)
            .Select(f => f.GetRawConstantValue() as string)
            .Order()
            .ToList();

        return Ok(list);
    }
}
