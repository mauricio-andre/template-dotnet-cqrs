using CqrsProject.App.GrpcServer.Attributes;
using CqrsProject.App.GrpcServer.Authorization;
using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.GrpcServer.Methods.V1.Examples;

[Authorize(Policy = AuthorizationPolicyNames.CanReadExamples)]
[RequireTenantIdInterceptor]
public class ExamplesGrpcService : ExamplesService.ExamplesServiceBase
{
    private readonly IMediator _mediator;

    public ExamplesGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task ListExamples(
        IAsyncStreamReader<ListExamplesRequest> requestStream,
        IServerStreamWriter<ExampleReply> responseStream,
        ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            int total;
            var skip = 0;
            var take = 1000;
            do
            {
                var result = await _mediator.Send(new SearchExampleQuery(
                    Term: requestStream.Current.HasTerm
                        ? requestStream.Current.Term
                        : null,
                    Take: take,
                    Skip: skip,
                    SortBy: requestStream.Current.HasSortBy
                        ? requestStream.Current.SortBy
                        : null
                ));

                total = result.TotalCount;
                skip += take;

                await foreach (var item in result.Items)
                {
                    await responseStream.WriteAsync(new ExampleReply
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
            } while (skip < total);
        }
    }

    public override async Task<ExampleReply> GetExample(
        GetExampleRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(new GetExampleByKeyQuery(request.Id));

        return new ExampleReply
        {
            Id = result.Id,
            Name = result.Name
        };
    }

    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public override async Task<ExampleReply> CreateExample(
        CreateExampleRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(new CreateExampleCommand(request.Name));

        return new ExampleReply
        {
            Id = result.Id,
            Name = result.Name
        };
    }

    [Authorize(Policy = AuthorizationPolicyNames.CanManageExamples)]
    public override async Task<Empty> RemoveExample(
        RemoveExampleRequest request,
        ServerCallContext context)
    {
        await _mediator.Send(new RemoveExampleCommand(request.Id));
        return new Empty();
    }
}
