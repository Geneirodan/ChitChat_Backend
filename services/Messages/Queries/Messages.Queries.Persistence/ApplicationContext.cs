using System.Reflection;
using Messages.Queries.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messages.Queries.Persistence;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Message> Messages { get; set; }
}