using MCB.Core.Infra.CrossCutting.RabbitMq.Connection;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;
public class RabbitMqConnection
    : RabbitMqConnectionBase
{
    public RabbitMqConnection(RabbitMqConnectionConfig connectionConfig) : base(connectionConfig)
    {
    }
}
