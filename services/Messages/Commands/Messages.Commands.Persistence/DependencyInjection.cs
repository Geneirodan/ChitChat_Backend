using System.Reflection;
using Messages.Commands.Application.Interfaces;
using Messages.Commands.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Messages.Commands.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        services
            .AddMarten(options =>
            {
                options.Connection(connectionString);
                options.UseSystemTextJsonForSerialization(EnumStorage.AsString, Casing.CamelCase);
            })
            .UseLightweightSessions()
            .OptimizeArtifactWorkflow();

        return services.AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IMessageRepository, MessageRepository>();
}