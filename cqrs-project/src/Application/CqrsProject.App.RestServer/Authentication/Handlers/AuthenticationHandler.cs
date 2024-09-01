using System.Security.Claims;
using System.Text.Encodings.Web;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity;
using CqrsProject.Core.Identity.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CqrsProject.App.RestService.Authentication;

public class AuthenticationHandler : AuthenticationHandler<AuthenticationOptions>
{
    private readonly IIdentitySyncService _identitySyncService;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

    public AuthenticationHandler(
        IOptionsMonitor<AuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IIdentitySyncService identitySyncService,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory) : base(options, logger, encoder)
    {
        _identitySyncService = identitySyncService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
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
        var (localUser, claimsIdentity) = await _identitySyncService.GetIdentitiesAsync(principal);

        if (localUser == null)
            localUser = await _identitySyncService.TryBindUserLoginAsync(claimsIdentity);

        if (localUser == null)
        {
            Logger.LogWarning("user not registered in the local database: {0}", claimsIdentity.Name);
            return;
        }

        var localIdentity = await _userClaimsPrincipalFactory.CreateAsync(localUser);

        localIdentity.Identities.First().AddClaims(new List<Claim>()
        {
            new Claim("creation_time", localUser.CreationTime.ToUnixTimeSeconds().ToString()),
            new Claim("last_modification_time", localUser.LastModificationTime?.ToUnixTimeSeconds().ToString() ?? string.Empty)
        });

        principal.AddIdentity(
            new ClaimsIdentity(
                localIdentity.Claims,
                AuthenticationDefaults.IdentityType,
                ClaimTypes.NameIdentifier,
                ClaimTypes.Role
            ));
    }
}
