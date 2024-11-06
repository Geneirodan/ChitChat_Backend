using MassTransit;

namespace Messages.SignalRNotifier;

/// <inheritdoc />
// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ConsumeObserver(ILogger<ConsumeObserver> logger) : IConsumeObserver
{
    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        logger.LogError(exception, "Fault in ConsumeObserver while processing message {Message}", context.Message);
        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Ended consume of message {Message}", context.Message);
        return Task.CompletedTask;
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        logger.LogInformation("Started consume of message {Message}", context.Message);
        return Task.CompletedTask;
    }
}