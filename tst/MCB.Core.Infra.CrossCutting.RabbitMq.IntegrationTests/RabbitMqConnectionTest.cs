using FluentAssertions;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection;
using MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Fixtures;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests;

[Collection(nameof(DefaultFixture))]
public class RabbitMqConnectionTest
{
    // Fields
    private readonly DefaultFixture _defaultFixture;

    // Constructors
    public RabbitMqConnectionTest(DefaultFixture defaultFixture)
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
    public void RabbitMqConnection_Should_OpenConnection_Should_Declare_Exchange()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.DefaultExchangeConfig;

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        connection.ExchangeDeclare(exchangeConfig); // Run for second time to bypass
    }
    [Fact]
    public void RabbitMqConnection_Should_CheckIfExchangeExists_Success()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.CreateExchangeConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        connection.ExchangeDeclare(exchangeConfig);
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeName);

        // Assert
        exchangeExists.Should().BeTrue();
    }
    [Fact]
    public void RabbitMqConnection_Should_CheckIfExchangeExists_Fail()
    {
        // Arrange
        var connectionConfig = _defaultFixture.DefaultConnectionConfig;
        var exchangeConfig = _defaultFixture.CreateExchangeConfig();

        var connection = new RabbitMqConnection(connectionConfig);
        connection.OpenConnection();

        // Act
        var exchangeExists = connection.CheckIfExchangeExists(exchangeConfig.ExchangeName);

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

        result1!.QueueName.Should().Be(queueConfig.QueueName);
        result2!.QueueName.Should().Be(queueConfig.QueueName);
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
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueName);

        // Assert
        result1.Should().NotBeNull();
        result1!.QueueName.Should().Be(queueConfig.QueueName);
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
        var queueExists = connection.CheckIfQueueExists(queueConfig.QueueName);

        // Assert
        queueExists.Should().BeFalse();
    }
}