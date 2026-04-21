using CqrsProject.Common.Extensions;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Identity.Handlers;

public class SearchUserRoleHandler : IRequestHandler<SearchUserRoleQuery, CollectionResponse<UserRoleResponse>>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IValidator<SearchUserRoleQuery> _validator;

    public SearchUserRoleHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<SearchUserRoleQuery> validator)
    {
        _dbContextFactory = dbContextFactory;
        _validator = validator;
    }

    public async Task<CollectionResponse<UserRoleResponse>> Handle(
        SearchUserRoleQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = CreateSearchQuery(administrationDbContext, request).AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        query = query
            .ApplySorting(request)
            .ApplyPagination(request);

        var items = query.AsAsyncEnumerable();
        return new CollectionResponse<UserRoleResponse>(items, totalCount);
    }

    private static IQueryable<UserRoleResponse> CreateSearchQuery(
        AdministrationDbContext administrationDbContext,
        SearchUserRoleQuery request)
    {
        return
            from user in administrationDbContext.Users
                .WhereIf(
                    !string.IsNullOrEmpty(request.UserName),
                    user => user.UserName!.ToLower().Contains(request.UserName!.ToLower()))
                .WhereIf(
                    request.UserId.HasValue,
                    user => user.Id == request.UserId)
            join userRole in administrationDbContext.UserRoles
                on user.Id equals userRole.UserId
            join role in administrationDbContext.Roles
                .WhereIf(
                    !string.IsNullOrEmpty(request.RoleName),
                    role => role.Name!.ToLower().Contains(request.RoleName!.ToLower()))
                .WhereIf(
                    request.RoleId.HasValue,
                    role => role.Id == request.RoleId)
                on userRole.RoleId equals role.Id
            where !user.IsDeleted
            select new UserRoleResponse(
                user.Id,
                role.Id,
                user.UserName!,
                role.Name!);
    }
}
