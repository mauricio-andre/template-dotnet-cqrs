using CqrsProject.Core.Tenants.Commands;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

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
