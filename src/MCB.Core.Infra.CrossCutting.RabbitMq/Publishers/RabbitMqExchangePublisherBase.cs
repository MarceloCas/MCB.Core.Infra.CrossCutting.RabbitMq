using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;

public abstract class RabbitMqExchangePublisherBase
    : RabbitMqPublisherBase
{
    // Properties
    protected RabbitMqExchangeConfig ExchangeConfig { get; }

    // Constructors
    public RabbitMqExchangePublisherBase(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig exchangeConfig
    ) : base(connection)
    {
        ExchangeConfig = exchangeConfig;
    }

    // Protected Abstract Methods
    protected abstract string GetExchangeName(string exchangeBaseName);

    // Public Methods
    public override void Initialize()
    {
        Connection.ExchangeDeclare(ExchangeConfig, exchangeNameFactory: GetExchangeName);
    }
    public override Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken)
    {
        return PublishAsync(subject, typeof(TSubject), cancellationToken);
    }
    public override Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken)
    {
        return PublishAsync(
            message: CreateRabbitMqMessageEnvelopInfo(subject, subjectBaseType),
            routingKey: GetRoutingKey(subject, subjectBaseType),
            basicProperties: GetBasicPropertiesInternal(subject!, subjectBaseType),
            cancellationToken: cancellationToken
        );
    }
    public override Task PublishAsync(ReadOnlyMemory<byte> message, string? routingKey, IBasicProperties? basicProperties, CancellationToken cancellationToken)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        lock (Connection.Channel)
        {
            Connection.Channel.BasicPublish(
                exchange: GetExchangeName(ExchangeConfig.ExchangeNameBase),
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: message
            );
        }
#pragma warning restore CS8604 // Possible null reference argument.

        return Task.CompletedTask;
    }
    // Protected Abstract Methods
    protected abstract string GetRoutingKey(object subject, Type subjectBaseType);
}
