using System.Security.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CqrsProject.App.RestServer.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
        UserManager<User> userManager) : base(options, logger, encoder)
    {
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _userManager = userManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var result = await Context.AuthenticateAsync(Options.JwtTokenScheme);
            if (result.None) return result;
            if (!result.Succeeded) return result;

            if (result.Principal == null)
                return AuthenticateResult.NoResult();

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(
                result.Principal.Claims,
                AuthenticationDefaults.IdentityType,
                ClaimTypes.NameIdentifier,
                ClaimTypes.Role));

            await SetLocalUserClaimAsync(principal);

            return AuthenticateResult.Success(new AuthenticationTicket(
                principal,
                null,
                AuthenticationDefaults.AuthenticationScheme
            ));
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }

    private async Task SetLocalUserClaimAsync(ClaimsPrincipal principal)
    {
        var nameIdentifier = principal.Identities
            .First()
            .FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var localUser = await GetLocalUser(nameIdentifier);
        if (localUser == null)
        {
            Logger.LogWarning("User not registered in the local database: {0}", nameIdentifier);
            return;
        }

        if (localUser.IsDeleted)
        {
            Logger.LogWarning("User was deleted in the local database: {0}", nameIdentifier);
            return;
        }

        var identity = await CreateLocalIdentity(localUser);
        principal.AddIdentity(identity);
    }

    private async Task<User?> GetLocalUser(string? nameIdentifier)
    {
        if (nameIdentifier == null)
            throw new AuthenticationException("No unique identifier provided in access token");

        var localUser = await _userManager.FindByLoginAsync(
            AuthenticationDefaults.AuthenticationScheme,
            nameIdentifier);

        return localUser;
    }

    private async Task<ClaimsIdentity> CreateLocalIdentity(User localUser)
    {
        var localIdentity = await _userClaimsPrincipalFactory.CreateAsync(localUser);

        localIdentity.Identities.First().AddClaims(new List<Claim>()
        {
            new Claim("creation_time", localUser.CreationTime.ToUnixTimeSeconds().ToString()),
            new Claim("last_modification_time", localUser.LastModificationTime?.ToUnixTimeSeconds().ToString() ?? string.Empty)
        });

        return new ClaimsIdentity(
            localIdentity.Claims,
            AuthenticationDefaults.LocalIdentityType,
            ClaimTypes.NameIdentifier,
            ClaimTypes.Role
        );
    }
}
