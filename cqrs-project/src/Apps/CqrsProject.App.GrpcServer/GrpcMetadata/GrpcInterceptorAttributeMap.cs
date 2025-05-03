using System.Reflection;
using CqrsProject.App.GrpcServer.Attributes;
using Grpc.Core;

namespace CqrsProject.App.GrpcServer.GrpcMetadata;

public class GrpcInterceptorAttributeMap : IGrpcInterceptorAttributeMap
{
    private readonly Dictionary<string, IEnumerable<InterceptorAttribute>> _methodAttributeMap = new();

    public GrpcInterceptorAttributeMap()
    {
        var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass
                && t.BaseType?.GetCustomAttribute<BindServiceMethodAttribute>() != null)
            .ToList();

        foreach (var type in types)
        {
            var classInterceptorAttribute = type.GetCustomAttributes<InterceptorAttribute>()
                ?? Enumerable.Empty<InterceptorAttribute>();

            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                var grpcServiceName = type.BaseType!.Name.Replace("Base", string.Empty);
                var fullMethodName = $"/{type.BaseType.Namespace}.{grpcServiceName}/{method.Name}".ToUpper();
                var methodInterceptorAttribute = method.GetCustomAttributes<InterceptorAttribute>()
                    ?? Enumerable.Empty<InterceptorAttribute>();

                if (classInterceptorAttribute.Any() || methodInterceptorAttribute.Any())
                    _methodAttributeMap[fullMethodName] = methodInterceptorAttribute.Concat(classInterceptorAttribute);
            }
        }
    }

    public bool TryGetInterceptorsByMethod(string method, out IEnumerable<InterceptorAttribute>? interceptorAttributes)
        => _methodAttributeMap.TryGetValue(method.ToUpper(), out interceptorAttributes);
}
