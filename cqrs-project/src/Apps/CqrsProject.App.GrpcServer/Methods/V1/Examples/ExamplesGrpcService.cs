using CqrsProject.App.GrpcServer.Authorization;
using CqrsProject.App.GrpcServer.Helpers;
using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Queries;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.GrpcServer.Methods.V1.Examples;

[Authorize(Policy = AuthorizationPolicyNames.CanReadExamples)]
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
        GrpcHelper.CheckRequireHeaderTenantId(context);

        while (await requestStream.MoveNext())
        {
            var result = await _mediator.Send(new SearchExampleQuery(
                Term: requestStream.Current.HasTerm
                    ? requestStream.Current.Term
                    : null,
                Take: requestStream.Current.HasTake
                    ? requestStream.Current.Take
                    : null,
                Skip: requestStream.Current.HasSkip
                    ? requestStream.Current.Skip
                    : null,
                SortBy: requestStream.Current.HasSortBy
                    ? requestStream.Current.SortBy
                    : null
            ));

            await foreach (var item in result.Items)
            {
                await responseStream.WriteAsync(new ExampleReply
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
        }
    }

    public override async Task<ExampleReply> GetExample(
        GetExampleRequest request,
        ServerCallContext context)
    {
        GrpcHelper.CheckRequireHeaderTenantId(context);
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
        GrpcHelper.CheckRequireHeaderTenantId(context);
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
        GrpcHelper.CheckRequireHeaderTenantId(context);
        await _mediator.Send(new RemoveExampleCommand(request.Id));
        return new Empty();
    }
}
