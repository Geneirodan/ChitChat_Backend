using System.Reflection;
using MassTransit;
using Messages.Commands.MassTransit.EventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.Commands.MassTransit;

public static class DependencyInjection
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services) =>
        services.AddMediatR(cfg=>cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddMassTransit(configurator =>
            {
                const string prefix = "MessagesCommands";
                configurator.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(prefix));
                configurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseInMemoryOutbox(context);
                    cfg.ConfigureEndpoints(context);
                });
            });
}