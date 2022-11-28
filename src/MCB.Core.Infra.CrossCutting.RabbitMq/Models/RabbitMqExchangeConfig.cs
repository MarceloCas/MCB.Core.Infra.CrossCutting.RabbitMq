using MCB.Core.Infra.CrossCutting.RabbitMq.Models.Enums;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqExchangeConfig
(
    string ExchangeNameBase,
    RabbitMqExchangeType ExchangeType,
    bool Durable,
    bool AutoDelete,
    IDictionary<string, object> Arguments
);