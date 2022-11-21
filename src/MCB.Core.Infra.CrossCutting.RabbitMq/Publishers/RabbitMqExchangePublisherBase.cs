using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

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

    // Public Methods
    public override Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken)
    {
        return PublishAsync(subject, typeof(TSubject), cancellationToken);
    }
    public override Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        Connection.PublishExchange(
            ExchangeConfig,
            routingKey: GetRoutingKey(subject, subjectBaseType),
            properties: GetBasicPropertiesInternal(subject, subjectBaseType),
            message: GetMessage(subject, subjectBaseType)
        );
#pragma warning restore CS8604 // Possible null reference argument.

        return Task.CompletedTask;
    }

    // Protected Abstract Methods
    protected abstract string GetRoutingKey(object subject, Type subjectBaseType);
}
