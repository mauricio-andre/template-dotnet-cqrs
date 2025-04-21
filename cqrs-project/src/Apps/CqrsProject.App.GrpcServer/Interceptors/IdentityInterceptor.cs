using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CqrsProject.Core.Identity.Interfaces;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CqrsProject.App.GrpcServer.Interceptors;

public class IdentityInterceptor : Interceptor
{
    private readonly ICurrentIdentity _currentIdentity;

    public IdentityInterceptor(ICurrentIdentity currentIdentity)
    {
        _currentIdentity = currentIdentity;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        CurrentIdentityHandler(context);
        return await continuation(request, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        CurrentIdentityHandler(context);
        await continuation(request, responseStream, context);
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        CurrentIdentityHandler(context);
        return await continuation(requestStream, context);
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        CurrentIdentityHandler(context);
        await continuation(requestStream, responseStream, context);
    }

    private void CurrentIdentityHandler(ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        _currentIdentity.SetCurrentIdentity(httpContext?.User);

        if (_currentIdentity.HasLocalIdentity())
            Activity.Current?.AddTag("appUser", _currentIdentity.GetLocalIdentityId());
    }
}
