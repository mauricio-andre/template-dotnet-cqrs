using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateRoleClaim;

public record CreateRoleClaimForYourselfEvent(Guid UserId) : INotification;
