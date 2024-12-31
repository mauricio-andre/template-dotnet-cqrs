using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.Queries;

public record GetUserQuery(
    Guid Id
) : IRequest<UserResponse>;
