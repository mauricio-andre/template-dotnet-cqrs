using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Scalar.AspNetCore;

namespace CqrsProject.Scalar.Extensions;

public static class ScalarServiceCollectionExtension
{
    public static WebApplication UseScalarProvider(this WebApplication app, Action<ScalarOptions>? options = default)
    {
        // Issue https://github.com/scalar/scalar/discussions/4668
        // Force manual audience include in urlToken ?audience=my-audience
        // Overview Issues Authentication https://github.com/scalar/scalar/issues/3696
        if (app.Configuration.GetValue<bool?>("Scalar:Enable") ?? false)
            app.MapScalarApiReference(options ?? (_ => { }));

        return app;
    }
}
