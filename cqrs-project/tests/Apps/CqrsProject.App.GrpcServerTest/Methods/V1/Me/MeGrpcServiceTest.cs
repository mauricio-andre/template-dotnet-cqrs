using System.Net;
using System.Net.Http.Headers;
using CqrsProject.App.GrpcServer.Methods.V1.Me;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Responses;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Core.Identity.UseCases.IdentitySync;
using CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using NSubstitute;

namespace CqrsProject.App.GrpcServerTest.Methods.V1.Me;

public class MeGrpcServiceTest
{
    [Fact(DisplayName = "Should reject SyncUser when the token is missing")]
    public async Task GivenSyncUserWithoutToken_WhenCalled_ThenThrowUnauthenticated()
    {
        using GrpcServerWebApplicationFactory factory = new GrpcServerWebApplicationFactory();
        using GrpcChannel channel = factory.CreateChannel();
        MeService.MeServiceClient client = new MeService.MeServiceClient(channel);

        await Assert.ThrowsAsync<RpcException>(
            () => client.SyncUserAsync(new Empty()).ResponseAsync);
    }

    [Fact(DisplayName = "Should sync the user when the token is valid")]
    public async Task GivenSyncUserWithToken_WhenCalled_ThenSendIdentitySyncCommand()
    {
        using GrpcServerWebApplicationFactory factory = new GrpcServerWebApplicationFactory();
        using GrpcChannel channel = factory.CreateChannel();
        MeService.MeServiceClient client = new MeService.MeServiceClient(channel);
        string token = JwtHelper.GenerateJwtToken("grpc-user");
        Metadata headers = CreateAuthorizationHeaders(token);

        factory.Mediator
            .Send(Arg.Any<IdentitySyncCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        Empty response = await client.SyncUserAsync(new Empty(), headers: headers);

        Assert.NotNull(response);
        await factory.Mediator.Received(1).Send(
            Arg.Is<IdentitySyncCommand>(command =>
                command.NameIdentifier == "grpc-user"
                && command.AccessToken == token),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject ListTenants when the token is missing")]
    public async Task GivenListTenantsWithoutToken_WhenCalled_ThenThrowUnauthenticated()
    {
        using GrpcServerWebApplicationFactory factory = new GrpcServerWebApplicationFactory();
        using GrpcChannel channel = factory.CreateChannel();
        MeService.MeServiceClient client = new MeService.MeServiceClient(channel);

        await Assert.ThrowsAsync<RpcException>(async () =>
        {
            using AsyncServerStreamingCall<MeTenantReply> call = client.ListTenants(new Empty());
            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
            }
        });
    }

    [Fact(DisplayName = "Should stream tenants when the token is valid")]
    public async Task GivenListTenantsWithToken_WhenCalled_ThenStreamTenants()
    {
        using GrpcServerWebApplicationFactory factory = new GrpcServerWebApplicationFactory();
        using GrpcChannel channel = factory.CreateChannel();
        MeService.MeServiceClient client = new MeService.MeServiceClient(channel);
        string token = JwtHelper.GenerateJwtToken("grpc-user");
        Metadata headers = CreateAuthorizationHeaders(token);

        factory.Mediator
            .Send(Arg.Any<SearchMeTenantQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new CollectionResponse<SearchMeTenantResponse>(GetTenants(), 2)));

        List<MeTenantReply> tenants = [];
        using AsyncServerStreamingCall<MeTenantReply> call = client.ListTenants(new Empty(), headers: headers);

        while (await call.ResponseStream.MoveNext(CancellationToken.None))
            tenants.Add(call.ResponseStream.Current);

        Assert.Collection(
            tenants,
            item =>
            {
                Assert.Equal("11111111-1111-1111-1111-111111111111", item.Id);
                Assert.Equal("Tenant One", item.TenantName);
            },
            item =>
            {
                Assert.Equal("22222222-2222-2222-2222-222222222222", item.Id);
                Assert.Equal("Tenant Two", item.TenantName);
            });

        await factory.Mediator.Received(1).Send(
            Arg.Is<SearchMeTenantQuery>(query =>
                query.Take == 1000
                && query.Skip == 0
                && query.TenantName == null
                && query.TenantIdList == null),
            Arg.Any<CancellationToken>());
    }

    private static Metadata CreateAuthorizationHeaders(string token)
    {
        return new Metadata
        {
            { "authorization", $"Bearer {token}" }
        };
    }

    private static async IAsyncEnumerable<SearchMeTenantResponse> GetTenants()
    {
        yield return new SearchMeTenantResponse(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Tenant One");

        yield return new SearchMeTenantResponse(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Tenant Two");

        await Task.CompletedTask;
    }
}
