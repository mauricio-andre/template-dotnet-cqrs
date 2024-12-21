using CqrsProject.Core.Tenants.Commands;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class RemoveTenantValidator : AbstractValidator<RemoveTenantCommand>
{
    public RemoveTenantValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
