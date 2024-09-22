using System.Reflection;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shared.MediatR.Behaviors;

namespace Shared.MediatR;

public static class DependencyInjection
{
    public static IServiceCollection
        AddMediatRPipeline(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddMediatRPipeline(new MediatRPipelineOptions(), assemblies);

    public static IServiceCollection AddMediatRPipeline(this IServiceCollection services,
        MediatRPipelineOptions options,
        params Assembly[] assemblies)
    {
        return services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            if (options.UseLogging)
                cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));

            if (options.UseAuthorization)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

            if (options.UseValidation)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            if (options.UseExceptions)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

            if (options.UsePerformance)
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });
    }
}