using CqrsProject.App.GrpcServer.Extensions;
using CqrsProject.Common.Localization;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Localization;
using GrpcStatus = Google.Rpc.Status;
using BadRequest = Google.Rpc.BadRequest;
using Status = Grpc.Core.Status;
using Google.Rpc;
using CqrsProject.Common.Exceptions;

namespace CqrsProject.App.GrpcServer.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ExceptionInterceptor(IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        return await CurrentTenantHandler(() => continuation(request, context));
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await CurrentTenantHandler<TResponse?>(
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
            async () =>
            {
                await continuation(requestStream, responseStream, context);
                return null;
            });
    }

    private async Task<T> CurrentTenantHandler<T>(Func<Task<T>> continuation)
    {
        try
        {
            return await continuation();
        }
        catch (ValidationException validationException)
        {
            var badRequest = new BadRequest();
            badRequest.FieldViolations.AddRange(
                validationException.Errors
                    .Select(error => new BadRequest.Types.FieldViolation
                    {
                        Field = error.PropertyName,
                        Description = error.ErrorMessage
                    }));

            var status = new GrpcStatus
            {
                Code = (int)Code.InvalidArgument,
                Message = _stringLocalizer["message:validation:fluentValidationExceptionTitle"].Value,
                Details = { Any.Pack(badRequest) }
            };

            throw status.ToRpcException();
        }
        catch (DuplicatedEntityException duplicatedEntityException)
        {
            var badRequest = new BadRequest();
            badRequest.FieldViolations.AddRange(
                duplicatedEntityException.Errors
                    .SelectMany(error => error.Value.Select(value => new BadRequest.Types.FieldViolation
                    {
                        Field = error.Key,
                        Description = value
                    })));

            var status = new GrpcStatus
            {
                Code = (int)Code.AlreadyExists,
                Message = duplicatedEntityException.Message,
                Details = { Any.Pack(badRequest) }
            };

            throw status.ToRpcException();
        }
        catch (BusinessException businessException)
        {
            var badRequest = new BadRequest();
            badRequest.FieldViolations.AddRange(
                businessException.Errors
                    .SelectMany(error => error.Value.Select(value => new BadRequest.Types.FieldViolation
                    {
                        Field = error.Key,
                        Description = value
                    })));

            var status = new GrpcStatus
            {
                Code = (int)Code.InvalidArgument,
                Message = businessException.Message,
                Details = { Any.Pack(badRequest) }
            };

            throw status.ToRpcException();
        }
        catch (UnauthorizedAccessException)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not allowed to complete request"));
        }
    }
}
