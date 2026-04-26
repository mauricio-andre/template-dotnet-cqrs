using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.UseCases.SearchRoleClaim;

public record SearchRoleClaimQuery(
    Guid RoleId,
    string? ClaimType,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<KeyValuePair<string, string>>>;
