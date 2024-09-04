using System.Security.Authentication;
using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Identity.Services;

public class IdentitySyncService : IIdentitySyncService
{
    private readonly UserManager<User> _userManager;

    public IdentitySyncService(UserManager<User> userManager) => _userManager = userManager;

    public async Task SyncIdentityUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        var (localUser, claimsIdentity) = await GetIdentitiesAsync(principal);

        if (localUser == null)
            localUser = await TryBindUserLoginAsync(claimsIdentity);

        if (localUser == null)
        {
            await CreateLocalUserAsync(claimsIdentity, cancellationToken);
            return;
        }

        await UpdateLocalUserAsync(localUser, claimsIdentity, cancellationToken);
    }

    public async Task<(User? localUser, ClaimsIdentity claimsIdentity)> GetIdentitiesAsync(ClaimsPrincipal? principal)
    {
        var identity = principal?.Identities.FirstOrDefault(identity =>
            identity.AuthenticationType == AuthenticationDefaults.IdentityType);

        if (identity == null)
            throw new AuthenticationException("User identity was not identified");

        var nameIdentifier = identity.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (nameIdentifier == null)
            throw new AuthenticationException("No unique identifier provided in access token");

        var localUser = await _userManager.FindByLoginAsync(AuthenticationDefaults.AuthenticationScheme, nameIdentifier);

        return (localUser, identity);
    }

    public async Task<User?> TryBindUserLoginAsync(ClaimsIdentity claimsIdentity)
    {
        var email = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email);
        var nameIdentifier = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (email == null || nameIdentifier == null)
            return null;

        var localUser = await _userManager.FindByEmailAsync(email.Value);

        if (localUser == null)
            return null;

        await _userManager.AddLoginAsync(
            localUser,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                nameIdentifier,
                "CqrsProject login"
            ));

        return localUser;
    }

    private async Task CreateLocalUserAsync(ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        // TODO: Consumir rotas de UserInfo para sincronizar os dados do usuário
        cancellationToken.ThrowIfCancellationRequested();
        var nameIdentifier = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value;
        var localUser = new User()
        {
            EmailConfirmed = false,
            TwoFactorEnabled = false,
            Email = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value,
            UserName = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value,
            CreationTime = DateTimeOffset.Now,
            IsDeleted = false
        };

        await _userManager.CreateAsync(localUser);
        await _userManager.AddLoginAsync(
            localUser,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                nameIdentifier,
                "CqrsProject loggin"
            )
        );
    }

    private async Task UpdateLocalUserAsync(User localUser, ClaimsIdentity claimsIdentity, CancellationToken cancellationToken)
    {
        // TODO: Consumir rotas de UserInfo para sincronizar os dados do usuário
        cancellationToken.ThrowIfCancellationRequested();

        localUser.LastModificationTime = DateTimeOffset.Now;
        localUser.Email = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value;
        localUser.UserName = claimsIdentity.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value;

        await _userManager.UpdateAsync(localUser);
    }
}
