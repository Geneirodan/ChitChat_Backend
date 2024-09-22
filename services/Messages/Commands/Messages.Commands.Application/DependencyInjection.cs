using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.MediatR;

namespace Messages.Commands.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return services
            .AddMediatRPipeline(assembly)
            .AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
    }
}