using MCB.Core.Infra.CrossCutting.Abstractions.Serialization;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;

public class RabbitMqQueuePublisher
    : RabbitMqQueuePublisherBase
{
    // Fields
    private IProtobufSerializer _protobufSerializer;

    // Constructors
    public RabbitMqQueuePublisher(
        IRabbitMqConnection connection,
        RabbitMqQueueConfig queueConfig,
        IProtobufSerializer protobufSerializer
    ) : base(connection, queueConfig)
    {
        _protobufSerializer = protobufSerializer;
    }

    // Protected Methods
    protected override IDictionary<string, object>? GetBasicProperties(object subject, Type subjectBaseType)
    {
        return null;
    }
    protected override ReadOnlyMemory<byte>? SerializeMessage(object subject, Type subjectBaseType)
    {
        return _protobufSerializer.SerializeToProtobuf(subject);
    }
}
