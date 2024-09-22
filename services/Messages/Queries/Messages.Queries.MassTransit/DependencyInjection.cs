using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.Queries.MassTransit;

public static class DependencyInjection
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services) =>
        services.AddMassTransit(configurator =>
        {
            const string prefix = "MessagesQueries";
            configurator.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(prefix));
            configurator.AddConsumers(Assembly.GetExecutingAssembly());
            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseInMemoryOutbox(context);
                cfg.ConfigureEndpoints(context);
            });
        });
}