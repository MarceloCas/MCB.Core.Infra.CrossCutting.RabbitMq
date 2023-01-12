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
    private IModel _channel = null!;

    // Properties
    public bool IsOpen => _connection?.IsOpen == true && _channel?.IsOpen == true;
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
    public virtual void OpenConnection(bool forceReopen = false)
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

    public virtual QueueDeclareOk? QueueDeclare(RabbitMqQueueConfig queueConfig)
    {
        TryAutoConnect();

        return _channel.QueueDeclare(
            queue: queueConfig.QueueName,
            durable: queueConfig.Durable,
            exclusive: queueConfig.Exclusive,
            autoDelete: queueConfig.AutoDelete,
            arguments: queueConfig.Arguments
        );
    }
    public virtual void ExchangeDeclare(RabbitMqExchangeConfig exchangeConfig)
    {
        TryAutoConnect();

        _channel.ExchangeDeclare(
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

    public virtual bool CheckIfQueueExists(string queueName)
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
    public virtual bool CheckIfExchangeExists(string exchangeName)
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

    public virtual void PublishQueue(RabbitMqQueueConfig queueConfig, IBasicProperties properties, ReadOnlyMemory<byte> message) 
    {
        lock (_channel)
        {
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueConfig.QueueName,
                basicProperties: properties,
                body: message
            );
        }
    }
    public virtual void PublishExchange(RabbitMqExchangeConfig exchangeConfig, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> message)
    {
        lock (_channel)
        {
            _channel.BasicPublish(
                exchange: exchangeConfig.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: message
            );
        }
    }

    public virtual (uint messageCount, uint consumerCount)? GetQueueCounters(string queueName)
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

    public virtual bool DeleteQueue(string queueName, bool ifUnused = false, bool ifEmpty = false)
    {
        try
        {
            _channel.QueueDelete(queueName, ifUnused, ifEmpty);
            return true;
        }
        catch (Exception)
        {
            OpenConnection();
            return false;
        }
    }
    public virtual void PurgeQueue(string queueName)
    {
        _channel.QueuePurge(queueName);
    }

    public virtual void DisconectConsumer(string consumerTag)
    {
        _channel.BasicCancel(consumerTag);
    }

    public virtual void Ack(ulong deliveryTag, bool multiple)
    {
        _channel.BasicAck(deliveryTag, multiple);
    }
    public virtual void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        _channel.BasicNack(deliveryTag, multiple, requeue);
    }

    public virtual IBasicProperties CreateBasicProperties()
    {
        return _channel.CreateBasicProperties();
    }

    public virtual void Dispose()
    {
        CloseConnectionInternal();

        _channel.Dispose();
        _connection.Dispose();

        GC.SuppressFinalize(this);
    }

    // Private Methods
    private void OpenConnectionInternal()
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        LastOpenDate = DateTime.UtcNow;
    }
    private void CloseConnectionInternal()
    {
        if(_channel?.IsOpen == true)
            _channel?.Close();

        if(_connection?.IsOpen == true)
            _connection?.Close();
    }
    private void TryAutoConnect()
    {
        if (_connectionConfig.AutoConnect && IsOpen == false)
            OpenConnection();
    }
}
