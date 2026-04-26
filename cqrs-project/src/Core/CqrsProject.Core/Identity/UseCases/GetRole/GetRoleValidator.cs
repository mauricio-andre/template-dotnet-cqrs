using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.GetRole;

public class GetRoleValidator : AbstractValidator<GetRoleQuery>
{
    public GetRoleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
