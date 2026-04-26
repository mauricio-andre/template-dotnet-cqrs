using FluentValidation;

namespace CqrsProject.Core.Tenants.UseCases.RemoveTenantConnectionString;

public class RemoveTenantConnectionStringValidator : AbstractValidator<RemoveTenantConnectionStringCommand>
{
    public RemoveTenantConnectionStringValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
