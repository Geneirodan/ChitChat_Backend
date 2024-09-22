using Messages.Queries.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Messages.Queries.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        return services
            .AddDbContext<ApplicationContext>(x => x.UseSqlServer(connectionString))
            .AddScoped<IMessageRepository, MessageRepository>()
            .AddScoped<IMessageReadRepository, MessageReadRepository>();
    }
}