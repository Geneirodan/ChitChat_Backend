using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.MediatR;

namespace Messages.Queries.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var options = new MediatRPipelineOptions { UseValidation = false };
        return services.AddMediatRPipeline(options, Assembly.GetExecutingAssembly());
    }
}