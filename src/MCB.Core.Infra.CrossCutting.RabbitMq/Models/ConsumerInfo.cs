using MCB.Core.Infra.CrossCutting.RabbitMq.Consumers;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;

public record struct ConsumerInfo(
    string? ConsumerTag, 
    RabbitMqConsumerConfig? Config, 
    CustomAsyncEventingBasicConsumer? Consumer
);