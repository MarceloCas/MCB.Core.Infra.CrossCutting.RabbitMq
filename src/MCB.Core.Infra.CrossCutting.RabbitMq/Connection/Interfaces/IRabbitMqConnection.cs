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
    void ExchangeDeclare(RabbitMqExchangeConfig exchangeConfig, Func<string, string>? exchangeNameFactory = null);
    QueueDeclareOk? QueueDeclare(RabbitMqQueueConfig queueConfig, Func<string, string>? queueNameFactory = null);
}
