using CqrsProject.Core.UserRoles.Queries;
using FluentValidation;

namespace CqrsProject.Core.UserRoles.Validations;

public class SearchUserRoleValidator : AbstractValidator<SearchUserRoleQuery>
{
    public SearchUserRoleValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
