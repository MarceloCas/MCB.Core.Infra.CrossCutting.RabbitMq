using MCB.Core.Infra.CrossCutting.RabbitMq.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqExchangeConfig
(
    string ExchangeName,
    RabbitMqExchangeType ExchangeType,
    bool Durable,
    bool AutoDelete,
    IDictionary<string, object> Arguments
);