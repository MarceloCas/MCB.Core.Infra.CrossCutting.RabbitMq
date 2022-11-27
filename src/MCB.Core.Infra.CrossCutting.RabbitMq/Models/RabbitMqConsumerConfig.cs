namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqConsumerConfig
(
    string QueueBaseName,
    bool UseQOS
);
