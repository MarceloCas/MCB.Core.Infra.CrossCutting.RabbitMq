using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection;
public class RabbitMqConnection
    : IRabbitMqConnection
{
    // Fields
    private readonly RabbitMqConnectionConfig _rabbitMqConnectionConfig;
    private readonly ConnectionFactory _connectionFactory;

    private IConnection _connection = null!;
    private IModel _channel = null!;

    // Properties
    public bool IsOpen => _connection?.IsOpen == true && _channel?.IsOpen == true;

    // Constructors
    public RabbitMqConnection(RabbitMqConnectionConfig rabbitMqConnectionConfig)
    {
        _rabbitMqConnectionConfig = rabbitMqConnectionConfig;
        _connectionFactory = new ConnectionFactory
        {
            ClientProvidedName = _rabbitMqConnectionConfig.ClientProvidedName,
            HostName = _rabbitMqConnectionConfig.HostName,
            Port = _rabbitMqConnectionConfig.Port,
            UserName = _rabbitMqConnectionConfig.Username,
            Password = _rabbitMqConnectionConfig.Password,
            VirtualHost = _rabbitMqConnectionConfig.VirtualHost,
            DispatchConsumersAsync = _rabbitMqConnectionConfig.DispatchConsumersAsync,
            AutomaticRecoveryEnabled = _rabbitMqConnectionConfig.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = _rabbitMqConnectionConfig.NetworkRecoveryInterval,
            TopologyRecoveryEnabled = _rabbitMqConnectionConfig.TopologyRecoveryEnabled,
            RequestedHeartbeat = _rabbitMqConnectionConfig.RequestedHeartbeat,
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

    public bool CheckIfQueueExists(string queueName)
    {
        try
        {
            _connection.CreateModel().QueueDeclarePassive(queueName);
            return true;
        }
        catch (Exception)
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

    public void DeleteQueue(string queueName, bool ifUnused, bool ifEmpty)
    {
        _channel.QueueDelete(queueName, ifUnused, ifEmpty);
    }
    public void PurgeQueue(string queueName)
    {
        _channel.QueuePurge(queueName);
    }

    public void DisconectConsumer(string consumerTag)
    {
        _channel.BasicCancel(consumerTag);
    }

    public void Ack(ulong deliveryTag, bool multiple)
    {
        _channel.BasicAck(deliveryTag, multiple);
    }
    public void Nack(ulong deliveryTag, bool multiple, bool requeue)
    {
        _channel.BasicNack(deliveryTag, multiple, requeue);
    }

    // Private Methods
    private void OpenConnectionInternal()
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
    }
    private void CloseConnectionInternal()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
