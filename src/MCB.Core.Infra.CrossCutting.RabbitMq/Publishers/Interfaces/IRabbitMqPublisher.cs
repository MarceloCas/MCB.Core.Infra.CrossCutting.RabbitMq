using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Publishers.Interfaces;

public interface IRabbitMqPublisher
    : IPublisher
{
    void Initialize();
    Task PublishAsync(ReadOnlyMemory<byte> message, string? routingKey, IBasicProperties? basicProperties, CancellationToken cancellationToken);
}
