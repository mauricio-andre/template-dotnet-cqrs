using CqrsProject.Core.Commands;
using CqrsProject.Core.Tenants;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class CreateTenantConnectionStringValidator : AbstractValidator<CreateTenantConnectionStringCommand>
{
    public CreateTenantConnectionStringValidator()
    {
        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.ConnectionName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(TenantConnectionStringConstrains.ConnectionNameMaxLength);

        RuleFor(prop => prop.KeyName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(TenantConnectionStringConstrains.KeyNameMaxLength);
    }
}
