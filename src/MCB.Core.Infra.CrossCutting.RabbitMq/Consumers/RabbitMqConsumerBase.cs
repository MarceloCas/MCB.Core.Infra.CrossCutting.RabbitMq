using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Consumers.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Consumers;

public abstract class RabbitMqConsumerBase
    : IRabbitMqConsumer
{
    // Fields
    private object _startConsumerLock = new();
    private object _stopConsumerLock = new();

    // Constants
    public const string CONSUMER_ALREADY_RUNNING = "CONSUMER_ALREADY_RUNNING";
    public const string CONSUMER_NOT_RUNNING = "CONSUMER_NOT_RUNNING";

    // Properties
    protected IRabbitMqConnection Connection { get; }
    protected ConsumerInfo ConsumerInfo { get; private set; }

    public bool IsRunning => ConsumerInfo.Consumer?.IsRunning == true;

    // Contructors
    protected RabbitMqConsumerBase(
        IRabbitMqConnection connection
    )
    {
        Connection = connection;
    }

    // Public Methods
    public virtual void StartConsumer(
        RabbitMqConsumerConfig consumerConfig,
        bool forceStop = false
    )
    {
        if (IsRunning && forceStop)
            StopConsumer();
        else if (IsRunning)
            throw new InvalidOperationException(CONSUMER_ALREADY_RUNNING);

        StartConsumerInternal(consumerConfig);
    }
    public virtual void StopConsumer()
    {
        if(!IsRunning)
            throw new InvalidOperationException(CONSUMER_NOT_RUNNING);

        StopConsumerInternal();
    }

    // Public Abstract Methods
    public abstract Task HandlerAsync(RabbitMqMessageEnvelop subject, CancellationToken cancellationToken);
    public abstract string GetQueueName(string queueBaseName);

    // Private Methods
    private void StartConsumerInternal(RabbitMqConsumerConfig consumerConfig)
    {
        lock (_startConsumerLock)
        {
            var customAsyncEventingBasicConsumer = new CustomAsyncEventingBasicConsumer(Connection.Channel);
            customAsyncEventingBasicConsumer.Received += Received;

            var consumerTag = Connection.Channel.BasicConsume(
                queue: GetQueueName(consumerConfig.QueueBaseName),
                autoAck: false,
                consumer: customAsyncEventingBasicConsumer
            );

            ConsumerInfo = new ConsumerInfo(consumerTag, consumerConfig, customAsyncEventingBasicConsumer); 
        }
    }
    private void StopConsumerInternal()
    {
        lock (_stopConsumerLock)
        {
            Connection.Channel.BasicCancel(ConsumerInfo.ConsumerTag);
            ConsumerInfo = new ConsumerInfo(
                ConsumerTag: null,
                Config: null,
                Consumer: null
            );
        }
    }
    private Task Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs @event)
    {
        throw new NotImplementedException();
    }
}

