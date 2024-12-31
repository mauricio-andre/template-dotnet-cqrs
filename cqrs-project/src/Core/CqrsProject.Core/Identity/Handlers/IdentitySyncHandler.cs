using CqrsProject.Common.Consts;
using CqrsProject.Common.Providers.OAuth.Dtos;
using CqrsProject.Common.Providers.OAuth.Interfaces;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Identity.Handlers;

public class IdentitySyncHandler : IRequestHandler<IdentitySyncCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IOAuthService _oAuthService;

    public IdentitySyncHandler(UserManager<User> userManager, IOAuthService oAuthService)
    {
        _userManager = userManager;
        _oAuthService = oAuthService;
    }

    public async Task Handle(IdentitySyncCommand request, CancellationToken cancellationToken)
    {
        var localUser = await _userManager.FindByLoginAsync(
            AuthenticationDefaults.AuthenticationScheme,
            request.NameIdentifier);

        var userInfo = await _oAuthService.GetUserInfoAsync(request.AccessToken, cancellationToken);

        if (localUser == null)
            localUser = await TryBindUserLoginAsync(userInfo, request.NameIdentifier, cancellationToken);

        if (localUser == null)
        {
            await CreateLocalUserAsync(userInfo, request.NameIdentifier, cancellationToken);
            return;
        }

        await UpdateLocalUserAsync(localUser, userInfo, cancellationToken);
    }

    public async Task<User?> TryBindUserLoginAsync(
        OAuthUserInfoDto userInfo,
        string nameIdentifier,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var localUser = await _userManager.FindByEmailAsync(userInfo.Email);

        if (localUser == null)
            return null;

        await _userManager.AddLoginAsync(
            localUser,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                nameIdentifier,
                AuthenticationDefaults.DisplayName
            ));

        return localUser;
    }

    private async Task CreateLocalUserAsync(
        OAuthUserInfoDto userInfo,
        string nameIdentifier,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userWithSameUserName = await _userManager.FindByNameAsync(userInfo.PreferredUserName);
        var userName = userWithSameUserName == null ? userInfo.PreferredUserName : userInfo.Email;

        var localUser = new User()
        {
            EmailConfirmed = false,
            TwoFactorEnabled = false,
            Email = userInfo.Email,
            UserName = userName,
            CreationTime = DateTimeOffset.Now.UtcDateTime,
            IsDeleted = false
        };

        await _userManager.CreateAsync(localUser);
        await _userManager.AddLoginAsync(
            localUser,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                nameIdentifier,
                AuthenticationDefaults.DisplayName
            )
        );
    }

    private async Task UpdateLocalUserAsync(
        User localUser,
        OAuthUserInfoDto userInfo,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        localUser.LastModificationTime = DateTimeOffset.Now.UtcDateTime;
        localUser.Email = userInfo.Email;
        localUser.EmailConfirmed = userInfo.EmailVerified;
        localUser.IsDeleted = false;

        await _userManager.UpdateAsync(localUser);
    }
}
