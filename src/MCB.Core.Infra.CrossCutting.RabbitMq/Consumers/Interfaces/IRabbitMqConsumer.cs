using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Consumers.Interfaces;
public interface IRabbitMqConsumer
    : ISubscriber<RabbitMqMessageEnvelop>
{
    bool IsRunning { get; }
}
