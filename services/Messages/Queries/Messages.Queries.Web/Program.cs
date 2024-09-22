using GraphQL.AspNet.Configuration;
using MassTransit;
using Messages.Queries.Application;
using Messages.Queries.MassTransit;
using Messages.Queries.Persistence;
using Messages.Queries.Web.Endpoints;
using Shared.OpenTelemetry;
using Shared.Web;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Logging.AddOpenTelemetryLogging();

services.AddPersistence(configuration.GetConnectionString("Database")!);

services
    .AddMassTransit()
    .Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMQ"))
    .AddApplicationServices()
    .AddHttpUser()
    .AddProblemDetails()
    .AddSwagger()
    .AddAuthenticationAndAuthorization(configuration.GetSection("Jwt"))
    .AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"))
    .AddGraphQL();

services.AddSharedOpenTelemetry(configuration);

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

app.MapHealthChecks();

app.UseGraphQL();

await app.RunAsync().ConfigureAwait(false);

public partial class Program;