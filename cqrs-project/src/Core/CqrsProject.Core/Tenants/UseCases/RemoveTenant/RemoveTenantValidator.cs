using FluentValidation;
using CqrsProject.Core.UserTenants.UseCases.RemoveUserTenant;

namespace CqrsProject.Core.Tenants.UseCases.RemoveTenant;

public class RemoveUserTenantValidator : AbstractValidator<RemoveTenantCommand>
{
    public RemoveUserTenantValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
