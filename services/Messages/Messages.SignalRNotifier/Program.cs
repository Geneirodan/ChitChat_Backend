using System.Reflection;
using AspNetCore.SignalR.OpenTelemetry;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using MassTransit.SignalR;
using Messages.SignalRNotifier.Hubs;
using OpenTelemetry.Resources;
using Shared.OpenTelemetry;
using Shared.Web;
using TypedSignalR.Client.DevTools;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Logging.AddOpenTelemetryLogging();

services
    .AddMassTransit(configurator =>
        {
            const string prefix = "MessagesSignalR";
            configurator.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(prefix));
            configurator.AddSignalRHub<ChatHub>();
            configurator.AddConsumers(Assembly.GetExecutingAssembly());
            configurator.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
        }
    )
    .Configure<RabbitMqTransportOptions>(configuration.GetSection("RabbitMQ"))
    .AddAuthenticationAndAuthorization(configuration.GetSection("Jwt"))
    .AddSignalR()
    .AddHubInstrumentation();

services.AddSharedOpenTelemetry(configuration)
    .WithTracing(providerBuilder => providerBuilder
        .AddSignalRInstrumentation()
        .AddSource(DiagnosticHeaders.DefaultListenerName)
    )
    .WithMetrics(b => b.AddMeter(InstrumentationOptions.MeterName));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSignalRHubSpecification();
    app.UseSignalRHubDevelopmentUI();
}

app.UseAuthentication()
    .UseAuthorization();

app.MapHub<ChatHub>("/api/v1/chat");

await app.RunAsync().ConfigureAwait(false);

public class ConsumeObserver(ILogger<ConsumeObserver> logger) : IConsumeObserver
{
    private readonly ILogger<ConsumeObserver> _logger = logger;

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        //use _logger

        return Task.CompletedTask;
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        //use _logger

        return Task.CompletedTask;
    }
}