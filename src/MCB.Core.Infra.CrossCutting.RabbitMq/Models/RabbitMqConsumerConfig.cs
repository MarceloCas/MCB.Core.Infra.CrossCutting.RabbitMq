namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqConsumerConfig
(
    RabbitMqQueueConfig QueueConfig,
    RabbitMqQueueConfig ErrorQueueConfig
);
