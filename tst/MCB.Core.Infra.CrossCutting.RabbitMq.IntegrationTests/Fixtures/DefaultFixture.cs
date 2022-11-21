using MCB.Core.Infra.CrossCutting.Abstractions.Serialization;
using MCB.Core.Infra.CrossCutting.DependencyInjection;
using MCB.Core.Infra.CrossCutting.DependencyInjection.Abstractions.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models.Enums;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;
using MCB.Core.Infra.CrossCutting.Serialization;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(DefaultFixture))]
public class DefaultFixtureColelction
    : ICollectionFixture<DefaultFixture>
{

}
public class DefaultFixture
{
    // Fields
    private readonly IProtobufSerializer _protobufSerializer;

    // Properties
    public IDependencyInjectionContainer DependencyInjectionContainer { get; }
    public RabbitMqConnectionConfig DefaultConnectionConfig { get; }
    public RabbitMqExchangeConfig DefaultDirectExchangeConfig { get; }
    public RabbitMqExchangeConfig DefaultFanoutExchangeConfig { get; }
    public RabbitMqExchangeConfig DefaultHeadersExchangeConfig { get; }
    public RabbitMqExchangeConfig DefaultTopicExchangeConfig { get; }
    public RabbitMqQueueConfig DefaultQueueConfig { get; }

    // Public Methods
    public DefaultFixture()
    { 
        var dependencyInjectionContainer = new DependencyInjectionContainer();
        Bootstrapper.ConfigureDependencyInjection(dependencyInjectionContainer);
        DependencyInjectionContainer = dependencyInjectionContainer.Build();

        DefaultConnectionConfig = CreateConnectionConfig();
        DefaultDirectExchangeConfig = CreateDirectExchangeConfig();
        DefaultFanoutExchangeConfig = CreateFanoutExchangeConfig();
        DefaultHeadersExchangeConfig = CreateHeadersExchangeConfig();
        DefaultTopicExchangeConfig = CreateTopicExchangeConfig();
        DefaultQueueConfig = CreateQueueConfig();

        ProtobufSerializer.ConfigureTypeCollection(new[]
        {
            typeof(DummyMessage)
        });
        _protobufSerializer = DependencyInjectionContainer.Resolve<IProtobufSerializer>()!;
    }

    public RabbitMqConnectionConfig CreateConnectionConfig()
    {
        return new RabbitMqConnectionConfig(
            ClientProvidedName: $"{nameof(DefaultFixtureColelction)}.{Guid.NewGuid()}",
            HostName: "127.0.0.1",
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
    public RabbitMqExchangeConfig CreateDirectExchangeConfig()
    {
        return new RabbitMqExchangeConfig(
            ExchangeName: $"temp.exchange.{Guid.NewGuid()}.direct",
            ExchangeType: RabbitMqExchangeType.Direct,
            Durable: false,
            AutoDelete: true,
            Arguments: null!
        );
    }
    public RabbitMqExchangeConfig CreateFanoutExchangeConfig()
    {
        return new RabbitMqExchangeConfig(
            ExchangeName: $"temp.exchange.{Guid.NewGuid()}.fanout",
            ExchangeType: RabbitMqExchangeType.Fanout,
            Durable: false,
            AutoDelete: true,
            Arguments: null!
        );
    }
    public RabbitMqExchangeConfig CreateHeadersExchangeConfig()
    {
        return new RabbitMqExchangeConfig(
            ExchangeName: $"temp.exchange.{Guid.NewGuid()}.headers",
            ExchangeType: RabbitMqExchangeType.Header,
            Durable: false,
            AutoDelete: true,
            Arguments: null!
        );
    }
    public RabbitMqExchangeConfig CreateTopicExchangeConfig()
    {
        return new RabbitMqExchangeConfig(
            ExchangeName: $"temp.exchange.{Guid.NewGuid()}.topic",
            ExchangeType: RabbitMqExchangeType.Topic,
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

    public RabbitMqQueuePublisher CreateRabbitMqQueuePublisher(
        IRabbitMqConnection connection,
        RabbitMqQueueConfig? queueConfig = null
    )
    {
        return new RabbitMqQueuePublisher(
            connection,
            queueConfig: queueConfig ?? DefaultQueueConfig,
            _protobufSerializer
        );
    }

    public RabbitMqExchangePublisherBase CreateDirectExchangePublisher(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig? exchangeConfig = null
    )
    {
        return new RabbitMqDirectExchangePublisher(
            connection,
            exchangeConfig: exchangeConfig ?? DefaultDirectExchangeConfig,
            _protobufSerializer
        );
    }
    public RabbitMqExchangePublisherBase CreateFanoutExchangePublisher(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig? exchangeConfig = null
    )
    {
        return new RabbitMqFanoutExchangePublisher(
            connection,
            exchangeConfig: exchangeConfig ?? DefaultFanoutExchangeConfig,
            _protobufSerializer
        );
    }
    public RabbitMqExchangePublisherBase CreateHeadersExchangePublisher(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig? exchangeConfig = null
    )
    {
        return new RabbitMqHeadersExchangePublisher(
            connection,
            exchangeConfig: exchangeConfig ?? DefaultHeadersExchangeConfig,
            _protobufSerializer
        );
    }
    public RabbitMqExchangePublisherBase CreateTopicExchangePublisher(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig? exchangeConfig = null
    )
    {
        return new RabbitMqTopicExchangePublisher(
            connection,
            exchangeConfig: exchangeConfig ?? DefaultTopicExchangeConfig,
            _protobufSerializer
        );
    }
}
