using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.SearchRole;

public class SearchRoleValidator : AbstractValidator<SearchRoleQuery>
{
    public SearchRoleValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
