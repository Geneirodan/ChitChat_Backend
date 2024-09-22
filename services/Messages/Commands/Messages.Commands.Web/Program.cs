using System.Reflection;
using FluentValidation;
using MassTransit;
using Messages.Commands.Application;
using Messages.Commands.Application.Interfaces;
using Messages.Commands.MassTransit;
using Messages.Commands.MassTransit.EventHandlers;
using Messages.Commands.Persistence;
using Messages.Commands.Web.Endpoints;
using Shared.MediatR;
using Shared.OpenTelemetry;
using Shared.Web;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Logging.AddOpenTelemetryLogging();

var connectionString = configuration.GetConnectionString("Database");

services
    .AddHttpUser()
    .AddMassTransit()
    .AddValidators()
    .Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMQ"))
    .AddPersistence(connectionString!)
    .AddProblemDetails()
    .AddSwagger()
    .AddAuthenticationAndAuthorization(configuration.GetSection("Jwt"))
    .AddSharedLocalization()
    .AddSharedOpenTelemetry(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseSwagger().UseSwaggerUI();

app.UseExceptionHandler()
    .UseStatusCodePages()
    .UseAuthentication()
    .UseAuthorization();

app.UseRequestLocalization();

app.MapMessages("api/v1/messages");

app.MapHealthChecks();

await app.RunAsync().ConfigureAwait(false);

public partial class Program;