namespace CqrsProject.App.RestServer.Endpoints.V1.Roles.Dtos;

public record SearchRoleClaimRequestDto(
    string? ClaimType,
    int? Take,
    int? Skip,
    string? SortBy
);
