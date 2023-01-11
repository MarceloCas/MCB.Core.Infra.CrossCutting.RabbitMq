using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
public interface IRabbitMqConnection
    : IDisposable
{
    bool IsOpen { get; }

    void OpenConnection(bool forceReopen = false);

    bool CheckIfExchangeExists(string exchangeName);
    void ExchangeDeclare(RabbitMqExchangeConfig exchangeConfig);
    
    bool CheckIfQueueExists(string queueName);
    QueueDeclareOk? QueueDeclare(RabbitMqQueueConfig queueConfig);
    void PurgeQueue(string queueName);
    bool DeleteQueue(string queueName, bool ifUnused = false, bool ifEmpty = false);
    (uint messageCount, uint consumerCount)? GetQueueCounters(string queueName);

    void DisconectConsumer(string consumerTag);

    void PublishExchange(RabbitMqExchangeConfig exchangeConfig, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> message);
    void PublishQueue(RabbitMqQueueConfig queueConfig, IBasicProperties properties, ReadOnlyMemory<byte> message);

    IBasicProperties CreateBasicProperties();
}
