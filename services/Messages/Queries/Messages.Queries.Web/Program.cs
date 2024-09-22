using GraphQL.AspNet.Configuration;
using MassTransit;
using Messages.Queries.Application;
using Messages.Queries.MassTransit;
using Messages.Queries.Persistence;
using Messages.Queries.Web.Endpoints;
using Shared.Web;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddPersistence(configuration.GetConnectionString("Database")!);

builder.Services
    .AddMassTransit()
    .Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMQ"))
    .AddApplicationServices()
    .AddHttpUser()
    .AddProblemDetails()
    .AddSwagger()
    .AddAuthenticationAndAuthorization(configuration.GetSection("Jwt"))
    .AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"))
    .AddGraphQL();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
    app.UseGraphQLGraphiQL();
}

app.UseExceptionHandler()
    .UseStatusCodePages()
    .UseAuthentication()
    .UseAuthorization();

app.MapMessages("api/v1/messages");

app.UseGraphQL();

await app.RunAsync().ConfigureAwait(false);

public partial class Program;