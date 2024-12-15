using CqrsProject.Core.Commands;
using FluentValidation;

namespace CqrsProject.Core.Validators;

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
