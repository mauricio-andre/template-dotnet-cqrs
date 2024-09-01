using System.Security.Claims;

namespace CqrsProject.Core.Identity.Interfaces;

public interface IIdentitySyncService
{
    Task SyncIdentityUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
    Task<(User? localUser, ClaimsIdentity claimsIdentity)> GetIdentitiesAsync(ClaimsPrincipal? principal);
    Task<User?> TryBindUserLoginAsync(ClaimsIdentity claimsIdentity);
}
