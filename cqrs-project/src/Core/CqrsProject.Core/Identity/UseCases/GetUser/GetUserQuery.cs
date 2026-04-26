using CqrsProject.Core.Identity.Responses;
using MediatR;

namespace CqrsProject.Core.Identity.UseCases.GetUser;

public record GetUserQuery(
    Guid Id
) : IRequest<UserResponse>;
