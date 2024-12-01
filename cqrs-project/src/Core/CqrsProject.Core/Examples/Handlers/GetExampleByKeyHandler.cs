using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Examples;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Handlers;

public class GetExampleByKeyHandler : IRequestHandler<GetExampleByKeyQuery, ExampleResponse>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<GetExampleByKeyQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public GetExampleByKeyHandler(
        CoreDbContext coreDbContext,
        IValidator<GetExampleByKeyQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<ExampleResponse> Handle(
        GetExampleByKeyQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _coreDbContext.Examples.FirstOrDefaultAsync(
            example => example.Id == request.Id,
            cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(Example), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static ExampleResponse MapToResponse(Example entity)
        => new ExampleResponse(entity.Id, entity.Name);
}
