using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Interfaces;

public interface IRabbitMqSubscriber
    : ISubscriber<RabbitMqMessage>
{
}
