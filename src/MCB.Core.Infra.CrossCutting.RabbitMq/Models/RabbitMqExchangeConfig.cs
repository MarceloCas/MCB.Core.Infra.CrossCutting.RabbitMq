using MCB.Core.Infra.CrossCutting.RabbitMq.Models.Enums;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqExchangeConfig
(
    string ExchangeName,
    RabbitMqExchangeType ExchangeType,
    bool Durable,
    bool AutoDelete,
    IDictionary<string, object>? Arguments
);