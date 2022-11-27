using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection;
public abstract class RabbitMqConnectionBase
    : IRabbitMqConnection
{
    // Constants
    public const string INVALID_RABBITMQ_EXCHANGE_TYPE = "INVALID_RABBITMQ_EXCHANGE_TYPE";

    // Fields
    private readonly RabbitMqConnectionConfig _connectionConfig;
    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection = null!;

    // Properties
    public IModel Channel { get; private set; } = null!;
    public bool IsOpen => _connection?.IsOpen == true && Channel?.IsOpen == true;
    public DateTime? LastOpenDate { get; private set; }

    // Constructors
    protected RabbitMqConnectionBase(RabbitMqConnectionConfig connectionConfig)
    {
        _connectionConfig = connectionConfig;
        _connectionFactory = new ConnectionFactory
        {
            ClientProvidedName = _connectionConfig.ClientProvidedName,
            HostName = _connectionConfig.HostName,
            Port = _connectionConfig.Port,
            UserName = _connectionConfig.Username,
            Password = _connectionConfig.Password,
            VirtualHost = _connectionConfig.VirtualHost,
            DispatchConsumersAsync = _connectionConfig.DispatchConsumersAsync,
            AutomaticRecoveryEnabled = _connectionConfig.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = _connectionConfig.NetworkRecoveryInterval,
            TopologyRecoveryEnabled = _connectionConfig.TopologyRecoveryEnabled,
            RequestedHeartbeat = _connectionConfig.RequestedHeartbeat,
        };
    }

    // Public Methods
    public void OpenConnection(bool forceReopen = false)
    {
        if (IsOpen)
        {
            if (forceReopen)
                CloseConnectionInternal();
            else
                return;
        }

        OpenConnectionInternal();
    }

    public QueueDeclareOk? QueueDeclare(RabbitMqQueueConfig queueConfig)
    {
        TryAutoConnect();

        return Channel.QueueDeclare(
            queue: queueConfig.QueueName,
            durable: queueConfig.Durable,
            exclusive: queueConfig.Exclusive,
            autoDelete: queueConfig.AutoDelete,
            arguments: queueConfig.Arguments
        );
    }
    public void ExchangeDeclare(RabbitMqExchangeConfig exchangeConfig)
    {
        TryAutoConnect();

        Channel.ExchangeDeclare(
            exchange: exchangeConfig.ExchangeName,
            type: exchangeConfig.ExchangeType switch
            {
                Models.Enums.RabbitMqExchangeType.Direct => ExchangeType.Direct,
                Models.Enums.RabbitMqExchangeType.Topic => ExchangeType.Topic,
                Models.Enums.RabbitMqExchangeType.Header => ExchangeType.Headers,
                Models.Enums.RabbitMqExchangeType.Fanout => ExchangeType.Fanout,
                _ => throw new InvalidOperationException(INVALID_RABBITMQ_EXCHANGE_TYPE),
            },
            durable: exchangeConfig.Durable,
            autoDelete: exchangeConfig.AutoDelete,
            arguments: exchangeConfig.Arguments
        );
    }

    public bool CheckIfQueueExists(string queueName)
    {
        try
        {
            _connection.CreateModel().QueueDeclarePassive(queueName);
            return true;
        }
        catch (OperationInterruptedException)
        {
            return false;
        }
    }
    public bool CheckIfExchangeExists(string exchangeName)
    {
        try
        {
            _connection.CreateModel().ExchangeDeclarePassive(exchangeName);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void PublishQueue(RabbitMqQueueConfig queueConfig, IBasicProperties properties, ReadOnlyMemory<byte> message) 
    {
        lock (Channel)
        {
            Channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueConfig.QueueName,
                basicProperties: properties,
                body: message
            );
        }
    }
    public void PublishExchange(RabbitMqExchangeConfig exchangeConfig, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> message)
    {
        lock (Channel)
        {
            Channel.BasicPublish(
                exchange: exchangeConfig.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: message
            );
        }
    }

    public (uint messageCount, uint consumerCount)? GetQueueCounters(string queueName)
    {
        try
        {
            var response = _connection.CreateModel().QueueDeclarePassive(queueName);
            return (response.MessageCount, response.ConsumerCount);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool DeleteQueue(string queueName, bool ifUnused = false, bool ifEmpty = false)
    {
        try
        {
            Channel.QueueDelete(queueName, ifUnused, ifEmpty);
            return true;
        }
        catch (Exception)
        {
            OpenConnection();
            return false;
        }
    }
    public void PurgeQueue(string queueName)
    {
        Channel.QueuePurge(queueName);
    }

    public void DisconectConsumer(string consumerTag)
    {
        Channel.BasicCancel(consumerTag);
    }

    public void Ack(ulong deliveryTag, bool multiple)
    {
        Channel.BasicAck(deliveryTag, multiple);
    }
    public void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        Channel.BasicNack(deliveryTag, multiple, requeue);
    }

    public IBasicProperties CreateBasicProperties()
    {
        return Channel.CreateBasicProperties();
    }

    public void CloseConsumer(string consumerTag)
    {
        Channel.BasicCancel(consumerTag);
    }

    public void Dispose()
    {
        CloseConnectionInternal();

        Channel.Dispose();
        _connection.Dispose();

        GC.SuppressFinalize(this);
    }

    // Private Methods
    private void OpenConnectionInternal()
    {
        _connection = _connectionFactory.CreateConnection();
        Channel = _connection.CreateModel();
        LastOpenDate = DateTime.UtcNow;
    }
    private void CloseConnectionInternal()
    {
        if(Channel?.IsOpen == true)
            Channel?.Close();

        if(_connection?.IsOpen == true)
            _connection?.Close();
    }
    private void TryAutoConnect()
    {
        if (_connectionConfig.AutoConnect && IsOpen == false)
            OpenConnection();
    }
}
