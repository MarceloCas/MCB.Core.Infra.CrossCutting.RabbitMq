using FluentAssertions;
using MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Fixtures;
using MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests;

[Collection(nameof(DefaultFixture))]
public class RabbitMqTest
{
    // Fields
    private readonly DefaultFixture _defaultFixture;

    // Constructors
    public RabbitMqTest(DefaultFixture defaultFixture)
    {
        _defaultFixture = defaultFixture;
    }

    [Fact]
    public void RabbitMqConnection_Should_OpenConnection()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var connection = new RabbitMqConnection(connectionConfig);

        // Act
        connection.OpenConnection();
        var firstIsOpen = connection.IsOpen;
        var firstLastOpenDate = connection.LastOpenDate;
        
        connection.OpenConnection(); // Call OpenConnection again should by pass
        var secondIsOpen = connection.IsOpen;
        var secondLastOpenDate = connection.LastOpenDate;

        // Assert
        firstIsOpen.Should().BeTrue();
        secondIsOpen.Should().BeTrue();
        firstLastOpenDate.Should().Be(secondLastOpenDate);
    }
    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_With_ForceReopen()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var connection = new RabbitMqConnection(connectionConfig);

        // Act
        connection.OpenConnection();
        var firstIsOpen = connection.IsOpen;
        var firstLastOpenDate = connection.LastOpenDate!.Value;

        Thread.Sleep(500);

        connection.OpenConnection(forceReopen: true);
        var secondIsOpen = connection.IsOpen;
        var secondLastOpenDate = connection.LastOpenDate!.Value;

        // Assert
        firstIsOpen.Should().BeTrue();
        secondIsOpen.Should().BeTrue();
        Assert.True(firstLastOpenDate < secondLastOpenDate);
    }

    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_DirectExchange()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.DefaultDirectExchangeConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        connection.ExchangeDeclare(exchangeConfig); // Run for second time to bypass

        // Assert
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);
        exchangeExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_FanoutExchange()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.DefaultFanoutExchangeConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        connection.ExchangeDeclare(exchangeConfig); // Run for second time to bypass

        // Assert
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);
        exchangeExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_HeadersExchange()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.DefaultHeadersExchangeConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        connection.ExchangeDeclare(exchangeConfig); // Run for second time to bypass

        // Assert
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);
        exchangeExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_TopicExchange()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.DefaultTopicExchangeConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        connection.ExchangeDeclare(exchangeConfig); // Run for second time to bypass

        // Assert
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);
        exchangeExists.Should().BeTrue();
    }

    [Fact]
    public void RabbitMqConnection_Should_CheckIfExchangeExists_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.CreateDirectExchangeConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);

        // Assert
        exchangeExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_CheckIfExchangeExists_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.CreateDirectExchangeConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeNameBase);

        // Assert
        exchangeExists.Should().BeFalse();
    }

    [Fact]
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_Queue()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.DefaultQueueConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var result1 = connection.QueueDeclare(queueConfig);
        var result2 = connection.QueueDeclare(queueConfig); // Run for second time to bypass

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();

        result1!.QueueName.Should().Be(queueConfig.QueueNameBase);
        result2!.QueueName.Should().Be(queueConfig.QueueNameBase);
    }
    [Fact]
    public void RabbitMqConnection_Should_CheckIfQueueExists_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var result1 = connection.QueueDeclare(queueConfig);
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);

        // Assert
        result1.Should().NotBeNull();
        result1!.QueueName.Should().Be(queueConfig.QueueNameBase);
        queueExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_CheckIfQueueExists_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeFalse();
    }

    [Fact]
    public async Task RabbitMqQueuePublisher_Should_PublishMessage()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();
        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );

        // Act
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);
    }
    [Fact]
    public async Task RabbitMqQueuePublisher_Should_PublishMessage_With_Inexistent_Queue()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );

        // Act
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);
    }
    
    [Fact]
    public async Task RabbitMqConnection_Should_GetQueueCounters_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();
        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );

        var messageCount = 10;

        for (int i = 0; i < messageCount; i++)
            await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        var queueCounters = connection.GetQueueCounters(queueConfig.QueueNameBase);

        // Assert
        queueCounters?.messageCount.Should().Be((uint)messageCount);
        queueCounters?.consumerCount.Should().Be(0);
    }
    [Fact]
    public void RabbitMqConnection_Should_GetQueueCounters_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var queueCounters = connection.GetQueueCounters(queueConfig.QueueNameBase);

        // Assert
        queueCounters.Should().BeNull();
    }

    [Fact]
    public async Task RabbitMqConnection_Should_DeleteQueue_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase);
        var queueExists = connection.CheckIfExchangeExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeFalse();
    }

    [Fact]
    public void RabbitMqConnection_Should_DeleteQueue_IfEmpty_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifEmpty: true);
        var queueExists = connection.CheckIfExchangeExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeFalse();
    }
    [Fact]
    public async Task RabbitMqConnection_Should_DeleteQueue_IfEmpty_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifEmpty: true);
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeTrue();
    }

    [Fact]
    public async Task RabbitMqConnection_Should_DeleteQueue_IfUnused_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifUnused: true);
        var queueExists = connection.CheckIfExchangeExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeFalse();
    }
    [Fact]
    public async Task RabbitMqConnection_Should_DeleteQueue_IfUnused_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifEmpty: true);
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);

        // Assert
#warning change this test when consumer was implemented
        //queueExists.Should().BeFalse();
        queueExists.Should().BeTrue();
    }

    [Fact]
    public void RabbitMqConnection_Should_DeleteQueue_IfEmpty_And_IfUnused_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifUnused: true, ifEmpty: true);
        var queueExists = connection.CheckIfExchangeExists(queueConfig.QueueNameBase);

        // Assert
        queueExists.Should().BeFalse();
    }
    [Fact]
    public async Task RabbitMqConnection_Should_DeleteQueue_And_IfUnused_IfEmpty_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);

        // Act
        connection.DeleteQueue(queueConfig.QueueNameBase, ifUnused: true, ifEmpty: true);
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);

        // Assert
#warning change this test when consumer was implemented
        //queueExists.Should().BeFalse();
        queueExists.Should().BeTrue();
    }

    [Fact]
    public async Task RabbitMqConnection_Should_PurgeQueue()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var queueConfig = _defaultFixture.CreateQueueConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        connection.QueueDeclare(queueConfig);

        var queuePublisher = _defaultFixture.CreateRabbitMqQueuePublisher(
            connection,
            queueConfig
        );
        await queuePublisher.PublishAsync(new DummyMessage(), cancellationToken: default);
        var beforeQueueCounters = connection.GetQueueCounters(queueConfig.QueueNameBase)!;

        // Act
        connection.PurgeQueue(queueConfig.QueueNameBase);
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueNameBase);
        var afterQueueCounters = connection.GetQueueCounters(queueConfig.QueueNameBase)!;

        // Assert
        queueExists.Should().BeTrue();
        beforeQueueCounters.Value.messageCount.Should().Be(1);
        afterQueueCounters.Value.messageCount.Should().Be(0);
    }
}