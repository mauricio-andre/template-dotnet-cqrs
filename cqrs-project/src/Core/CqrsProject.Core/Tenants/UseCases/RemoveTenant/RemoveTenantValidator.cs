using FluentValidation;

namespace CqrsProject.Core.Tenants.UseCases.RemoveTenant;

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
