namespace CqrsProject.App.RestServer.Endpoints.V1.Roles.Dtos;

public record CreateRoleClaimRequestDto(
    string ClaimType,
    string ClaimValue
);
