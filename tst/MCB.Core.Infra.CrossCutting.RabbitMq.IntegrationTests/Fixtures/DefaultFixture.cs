using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(DefaultFixture))]
public class DefaultFixtureColelction
    : ICollectionFixture<DefaultFixture>
{

}
public class DefaultFixture
{
    public RabbitMqConnectionConfig DefaultConnectionConfig { get; }
    public RabbitMqExchangeConfig DefaultExchangeConfig { get; }
    public RabbitMqQueueConfig DefaultQueueConfig { get; }

    public DefaultFixture()
    {
        DefaultConnectionConfig = CreateConnectionConfig();
        DefaultExchangeConfig = CreateExchangeConfig();
        DefaultQueueConfig = CreateQueueConfig();
    }

    public RabbitMqConnectionConfig CreateConnectionConfig()
    {
        return new RabbitMqConnectionConfig(
            ClientProvidedName: $"{nameof(DefaultFixtureColelction)}.{Guid.NewGuid()}",
            HostName: "localhost",
            Port: 5672,
            Username: "guest",
            Password: "guest",
            VirtualHost: "/",
            DispatchConsumersAsync: true,
            AutoConnect: true,
            AutomaticRecoveryEnabled: true,
            NetworkRecoveryInterval: TimeSpan.FromSeconds(5),
            TopologyRecoveryEnabled: true,
            RequestedHeartbeat: TimeSpan.FromSeconds(60)
        );
    }
    public RabbitMqExchangeConfig CreateExchangeConfig()
    {
        return new RabbitMqExchangeConfig(
            ExchangeName: $"temp.exchange.{Guid.NewGuid()}",
            ExchangeType: Models.Enums.RabbitMqExchangeType.Direct,
            Durable: false,
            AutoDelete: true,
            Arguments: null!
        );
    }
    public RabbitMqQueueConfig CreateQueueConfig()
    {
        return new RabbitMqQueueConfig(
            QueueName: $"temp.queue.{Guid.NewGuid()}",
            Durable: false,
            Exclusive: false,
            AutoDelete: true,
            Arguments: null!
        );
    }
}
