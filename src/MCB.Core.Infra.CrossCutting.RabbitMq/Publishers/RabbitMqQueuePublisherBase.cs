using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

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

    // Public Methods
    public override Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken)
    {
        return PublishAsync(subject, typeof(TSubject), cancellationToken);
    }
    public override Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        Connection.PublishQueue(
            QueueConfig,
            properties: GetBasicPropertiesInternal(subject, subjectBaseType),
            message: GetMessage(subject, subjectBaseType)
        );
#pragma warning restore CS8604 // Possible null reference argument.

        return Task.CompletedTask;
    }
}
