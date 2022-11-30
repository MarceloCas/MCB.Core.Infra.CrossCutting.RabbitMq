using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;

public abstract class RabbitMqQueuePublisherBase
    : RabbitMqPublisherBase
{
    // Properties
    protected RabbitMqQueueConfig QueueConfig { get; }

    // Constructors
    protected RabbitMqQueuePublisherBase(
        IRabbitMqConnection connection,
        RabbitMqQueueConfig queueConfig
    ) : base(connection)
    {
        QueueConfig = queueConfig;
    }

    // Protected Abstract Methods
    protected abstract string GetQueueName(string queueBaseName);

    // Public Methods
    public override Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken)
    {
        return PublishAsync(subject, typeof(TSubject), cancellationToken);
    }
    public override Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken)
    {
        return PublishAsync(
            message: CreateRabbitMqMessageEnvelopInfo(subject, subjectBaseType),
            routingKey: GetQueueName(QueueConfig.QueueNameBase),
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
                exchange: string.Empty,
                routingKey: routingKey,
                basicProperties: basicProperties,
                body: message
            );
        }
#pragma warning restore CS8604 // Possible null reference argument.

        return Task.CompletedTask;
    }
}
