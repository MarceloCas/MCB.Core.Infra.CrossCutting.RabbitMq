using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
public interface IRabbitMqConnection
    : IDisposable
{
    void OpenConnection(bool forceReopen = false);

    bool CheckIfExchangeExists(string exchangeName);
    void PublishExchange(RabbitMqExchangeConfig exchangeConfig, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> message);

    bool CheckIfQueueExists(string queueName);
    QueueDeclareOk? QueueDeclare(RabbitMqQueueConfig queueConfig);
    void PublishQueue(RabbitMqQueueConfig queueConfig, IBasicProperties properties, ReadOnlyMemory<byte> message);
    void PurgeQueue(string queueName);
    bool DeleteQueue(string queueName, bool ifUnused = false, bool ifEmpty = false);
    (uint messageCount, uint consumerCount)? GetQueueCounters(string queueName);

    void DisconectConsumer(string consumerTag);

    IBasicProperties CreateBasicProperties();
}
