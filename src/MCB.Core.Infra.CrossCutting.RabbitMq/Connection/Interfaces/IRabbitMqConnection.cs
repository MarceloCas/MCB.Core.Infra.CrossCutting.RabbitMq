using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
public interface IRabbitMqConnection
    : IDisposable
{
    // Properties
    IModel Channel { get; }
    bool IsOpen { get; }
    DateTime? LastOpenDate { get; }

    // Methods
    IBasicProperties CreateBasicProperties();
    void PublishExchange(RabbitMqExchangeConfig exchangeConfig, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> message);
    void PublishQueue(RabbitMqQueueConfig queueConfig, IBasicProperties properties, ReadOnlyMemory<byte> message);
}
