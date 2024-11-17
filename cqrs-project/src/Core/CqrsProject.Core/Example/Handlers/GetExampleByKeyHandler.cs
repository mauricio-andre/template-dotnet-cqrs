using CqrsProject.Core.Data;
using CqrsProject.Core.Examples;
using CqrsProject.Core.Exceptions;
using CqrsProject.Core.Queries;
using CqrsProject.Core.Responses;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Handlers;

public class GetExampleByKeyHandler : IRequestHandler<GetExampleByKeyQuery, ExampleResponse>
{
    private readonly CoreDbContext _coreDbContext;
    private readonly IValidator<GetExampleByKeyQuery> _validator;

    public GetExampleByKeyHandler(
        CoreDbContext coreDbContext,
        IValidator<GetExampleByKeyQuery> validator)
    {
        _coreDbContext = coreDbContext;
        _validator = validator;
    }

    public async Task<ExampleResponse> Handle(
        GetExampleByKeyQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var entity = await _coreDbContext.Examples.FirstOrDefaultAsync(
            example => example.Id == request.Id,
            cancellationToken);

        // TODO: configurar multi language
        if (entity == null)
            throw new ExampleNotFoundException("Definir messagem");

        return MapToResponse(entity);
    }

    private static ExampleResponse MapToResponse(Example entity)
        => new ExampleResponse(entity.Id, entity.Name);
}
