using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateRoleClaimForYourselfEvent(Guid UserId) : INotification;
