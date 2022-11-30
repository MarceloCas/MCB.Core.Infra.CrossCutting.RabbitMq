namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqConsumerConfig
(
    RabbitMqQueueConfig QueueConfig,
    Func<string, string>? QueueNameFactory,
    RabbitMqQueueConfig ErrorQueueConfig,
    Func<string, string>? ErrorQueueNameFactory
);
