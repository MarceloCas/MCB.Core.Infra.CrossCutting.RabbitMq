using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers.Interfaces;
using RabbitMQ.Client;
using System.Net;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;

public abstract class RabbitMqPublisherBase
    : IRabbitMqPublisher
{
    // Constants
    public const string MESSAGE_SERIALIZATION_CANNOT_RETURN_NULL = "MESSAGE_SERIALIZATION_CANNOT_RETURN_NULL";
    public const string MESSAGE_ENVELOP_SERIALIZATION_CANNOT_RETURN_NULL = "MESSAGE_ENVELOP_SERIALIZATION_CANNOT_RETURN_NULL";

    // Fields
    protected IRabbitMqConnection Connection { get; }

    // Constructors
    public RabbitMqPublisherBase(
        IRabbitMqConnection connection
    )
    {
        Connection = connection;
    }

    // Public Methods
    public abstract Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken);
    public abstract Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken);

    public void Subscribe(Type subscriberType, Type subjectType)
    {
        throw new NotImplementedException();
    }
    public void Subscribe<TSubscriber>(Type subjectType)
    {
        throw new NotImplementedException();
    }
    public void Subscribe<TSubscriber, TSubject>() where TSubscriber : ISubscriber<TSubject>
    {
        throw new NotImplementedException();
    }

    // Protected Methods
    protected ReadOnlyMemory<byte> GetMessage<TSubject>(TSubject subject, Type subjectBaseType)
    {
        if (subject is null)
            throw new ArgumentNullException(nameof(subject));

        var subjectSerialized = SerializeMessage(subject, subjectBaseType);

        if (subjectSerialized is null)
            throw new InvalidOperationException(MESSAGE_SERIALIZATION_CANNOT_RETURN_NULL);

        var rabbitMqMessageEnvelopInfo = GetRabbitMqMessageEnvelopInfo(subject, subjectBaseType);
        var rabbitMqMessageEnvelop = new RabbitMqMessageEnvelop(
            TenantId: rabbitMqMessageEnvelopInfo.TenantId,
            CorrelationId: rabbitMqMessageEnvelopInfo.CorrelationId,
            ExecutionUser: rabbitMqMessageEnvelopInfo.ExecutionUser,
            SourcePlatform: rabbitMqMessageEnvelopInfo.SourcePlatform,
            TimeStamp: DateTime.UtcNow,
            MessageType: typeof(TSubject),
            Message: subjectSerialized.Value.ToArray()
        );

        var serializedRabbitMqMessageEnvelop = SerializeRabbitMqEnvelopMessage(rabbitMqMessageEnvelop);

        return serializedRabbitMqMessageEnvelop is null
            ? throw new InvalidOperationException(MESSAGE_ENVELOP_SERIALIZATION_CANNOT_RETURN_NULL)
            : serializedRabbitMqMessageEnvelop.Value;
    }
    protected IBasicProperties? GetBasicPropertiesInternal(object subject, Type subjectBaseType)
    {
        var propertyDictionary = GetBasicProperties(subject, subjectBaseType);

        if (propertyDictionary is null
            || propertyDictionary.Count == 0)
            return null;

        var basicProperties = Connection.CreateBasicProperties();
        basicProperties.Headers = propertyDictionary;

        return basicProperties;
    }

    // Protected Abstract Methods
    protected abstract IDictionary<string, object>? GetBasicProperties(object subject, Type subjectBaseType);
    protected abstract (Guid TenantId, Guid CorrelationId, string ExecutionUser, string SourcePlatform) GetRabbitMqMessageEnvelopInfo(object subject, Type subjectBaseType);
    protected abstract ReadOnlyMemory<byte>? SerializeMessage(object subject, Type subjectBaseType);
    protected abstract ReadOnlyMemory<byte>? SerializeRabbitMqEnvelopMessage(RabbitMqMessageEnvelop rabbitMqMessageEnvelop);
}
