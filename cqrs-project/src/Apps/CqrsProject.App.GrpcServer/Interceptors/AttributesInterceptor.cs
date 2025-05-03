using CqrsProject.App.GrpcServer.GrpcMetadata;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace CqrsProject.App.GrpcServer.Interceptors;

public class AttributesInterceptor : Interceptor
{
    private readonly IGrpcInterceptorAttributeMap _grpcInterceptorAttributeMap;

    public AttributesInterceptor(IGrpcInterceptorAttributeMap grpcInterceptorAttributeMap)
    {
        _grpcInterceptorAttributeMap = grpcInterceptorAttributeMap;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (!_grpcInterceptorAttributeMap.TryGetInterceptorsByMethod(context.Method, out var actions))
            return continuation(request, context);

        var pipeline = actions!.Aggregate(
            continuation,
            (next, interceptor) => async (requestPipe, contextPipe) =>
                await interceptor.OnActionExecutingUnary(requestPipe, contextPipe, next));

        return pipeline(request, context);
    }

    public override Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        if (!_grpcInterceptorAttributeMap.TryGetInterceptorsByMethod(context.Method, out var actions))
            return continuation(request, responseStream, context);

        var pipeline = actions!.Aggregate(
            continuation,
            (next, interceptor) => async (requestPipe, responsePipe, contextPipe) =>
                await interceptor.OnActionExecutingServerStreaming(requestPipe, responsePipe, contextPipe, next));

        return pipeline(request, responseStream, context);
    }

    public override Task<TResponse> ClientStreamingServerHandler <TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        if (!_grpcInterceptorAttributeMap.TryGetInterceptorsByMethod(context.Method, out var actions))
            return continuation(requestStream, context);

        var pipeline = actions!.Aggregate(
            continuation,
            (next, interceptor) => async (requestPipe, contextPipe) =>
                await interceptor.OnActionExecutingClientStreaming(requestPipe, contextPipe, next));

        return pipeline(requestStream, context);
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        if (!_grpcInterceptorAttributeMap.TryGetInterceptorsByMethod(context.Method, out var actions))
            return continuation(requestStream, responseStream, context);

        var pipeline = actions!.Aggregate(
            continuation,
            (next, interceptor) => async (requestPipe, responsePipe, contextPipe) =>
                await interceptor.OnActionExecutingDuplexStreaming(requestPipe, responsePipe, contextPipe, next));

        return pipeline(requestStream, responseStream, context);
    }
}
