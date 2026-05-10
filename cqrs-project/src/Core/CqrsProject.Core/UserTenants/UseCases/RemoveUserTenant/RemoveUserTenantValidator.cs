using CqrsProject.Core.Tenants.UseCases.RemoveTenant;
using FluentValidation;

namespace CqrsProject.Core.UserTenants.UseCases.RemoveUserTenant;

public class RemoveUserTenantValidator : AbstractValidator<RemoveUserTenantCommand>
{
    public RemoveUserTenantValidator()
    {
        RuleFor(prop => prop.UserId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
