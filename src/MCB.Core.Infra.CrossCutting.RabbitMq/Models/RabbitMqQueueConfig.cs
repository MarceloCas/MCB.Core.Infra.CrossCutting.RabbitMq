namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;

public record class RabbitMqQueueConfig(
    string QueueName,
    bool Durable,
    bool Exclusive,
    bool AutoDelete,
    IDictionary<string, object> Arguments
);
