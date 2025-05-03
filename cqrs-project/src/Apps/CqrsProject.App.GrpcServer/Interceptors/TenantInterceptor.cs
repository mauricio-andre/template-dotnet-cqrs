using CqrsProject.App.GrpcServer.Extensions;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Exceptions;
using CqrsProject.Core.Tenants.Interfaces;
using Grpc.Core;
using Grpc.Core.Interceptors;
using MediatR;

namespace CqrsProject.App.GrpcServer.Interceptors;

public class TenantInterceptor : Interceptor
{
    private readonly ICurrentTenant _currentTenant;
    private readonly ICurrentIdentity _currentIdentity;
    private readonly IMediator _mediator;

    public TenantInterceptor(
        ICurrentTenant currentTenant,
        ICurrentIdentity currentIdentity,
        IMediator mediator)
    {
        _currentTenant = currentTenant;
        _currentIdentity = currentIdentity;
        _mediator = mediator;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        return await CurrentTenantHandler(context, () => continuation(request, context));
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await CurrentTenantHandler<TResponse?>(
            context,
            async () =>
            {
                await continuation(request, responseStream, context);
                return null;
            });
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        return await CurrentTenantHandler(
            context,
            () => continuation(requestStream, context));
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        if (context.IsServerReflectionMethod())
        {
            await continuation(requestStream, responseStream, context);
            return;
        }

        await CurrentTenantHandler<TResponse?>(
            context,
            async () =>
            {
                await continuation(requestStream, responseStream, context);
                return null;
            });
    }

    private async Task<T> CurrentTenantHandler<T>(
        ServerCallContext context,
        Func<Task<T>> continuation)
    {
        var tenantIdHeader = context.RequestHeaders.GetValue("tenant-id");
        if (string.IsNullOrEmpty(tenantIdHeader))
            return await continuation();

        if (!Guid.TryParse(tenantIdHeader, out Guid tenantId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Tenant-Id not in expected format"));

        try
        {
            await _mediator.Publish(new TenantAccessedByUserEvent(
                UserId: _currentIdentity.GetLocalIdentityId(),
                TenantId: tenantId
            ));
        }
        catch (TenantUnreleasedException)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "tenant not released to user"));
        }
        catch (UnauthorizedAccessException)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "unrecognized internal user"));
        }

        using (_currentTenant.BeginTenantScope(tenantId))
            return await continuation();
    }
}
