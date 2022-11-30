using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Consumers.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers.Interfaces;
using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Consumers;

public abstract class RabbitMqConsumerBase
    : IRabbitMqConsumer
{
    // Fields
    private readonly object _startConsumerLock = new();
    private readonly object _stopConsumerLock = new();

    // Constants
    public const string CONSUMER_ALREADY_RUNNING = "CONSUMER_ALREADY_RUNNING";
    public const string CONSUMER_NOT_RUNNING = "CONSUMER_NOT_RUNNING";
    public const string HANDLER_ASYNC_NOT_ALLOWED = "HANDLER_ASYNC_NOT_ALLOWED";

    // Properties
    protected IRabbitMqConnection Connection { get; }
    protected IRabbitMqPublisher? ErrorRabbitMqPublisher { get; }
    protected ConsumerInfo ConsumerInfo { get; private set; }

    public bool IsRunning => ConsumerInfo.Consumer?.IsRunning == true;

    // Contructors
    protected RabbitMqConsumerBase(
        IRabbitMqConnection connection,
        IRabbitMqPublisher? errorRabbitMqPublisher
    )
    {
        Connection = connection;
        ErrorRabbitMqPublisher = errorRabbitMqPublisher;
    }

    // Public Abstract Methods
    public abstract string GetQueueName(string queueBaseName);
    public abstract string GetErrorQueueName(string queueBaseName);
    public abstract Task<(bool success, bool redelivery)> HandlerInternalAsync(RabbitMqMessageEnvelop subject, CancellationToken cancellationToken);

    // Public Methods
    public virtual void StartConsumer(
        RabbitMqConsumerConfig consumerConfig,
        Action<IRabbitMqConnection, RabbitMqConsumerConfig>? additionalConfigHandler,
        bool forceStop = false
    )
    {
        if (IsRunning && forceStop)
            StopConsumer();
        else if (IsRunning)
            throw new InvalidOperationException(CONSUMER_ALREADY_RUNNING);

        StartConsumerInternal(consumerConfig, additionalConfigHandler);
    }
    public virtual void StopConsumer()
    {
        if (!IsRunning)
            throw new InvalidOperationException(CONSUMER_NOT_RUNNING);

        StopConsumerInternal();
    }
    public Task HandlerAsync(RabbitMqMessageEnvelop subject, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException(HANDLER_ASYNC_NOT_ALLOWED);
    }

    // Protetected Abstract Methods
    protected abstract RabbitMqMessageEnvelop DesserializeRabbitMqMessageEnvelop(ReadOnlyMemory<byte> message);

    // Private Methods
    private async Task Received(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs @event)
    {
        var rabbitMqMessageEnvelop = default(RabbitMqMessageEnvelop);

        try
        {
            rabbitMqMessageEnvelop = DesserializeRabbitMqMessageEnvelop(@event.Body);

            var handlerResult = await HandlerInternalAsync(
                subject: DesserializeRabbitMqMessageEnvelop(@event.Body),
                cancellationToken: default
            );

            lock (Connection.Channel)
            {
                if (handlerResult.success)
                    Connection.Channel.BasicAck(
                        deliveryTag: @event.DeliveryTag,
                        multiple: false
                    );
                else
                    Connection.Channel.BasicNack(
                        deliveryTag: @event.DeliveryTag,
                        multiple: false,
                        requeue: handlerResult.redelivery
                    );
            }
        }
        catch (Exception)
        {
            if(rabbitMqMessageEnvelop is not null)
                ErrorRabbitMqPublisher?.PublishAsync(
                    rabbitMqMessageEnvelop.Message,
                    cancellationToken: default
                );

            lock (Connection.Channel)
            {
                Connection.Channel.BasicNack(
                    deliveryTag: @event.DeliveryTag,
                    multiple: false,
                    requeue: false
                );
            }
        }
    }

    private void StartConsumerInternal(
        RabbitMqConsumerConfig consumerConfig, 
        Action<IRabbitMqConnection, RabbitMqConsumerConfig>? additionalConfigHandler
    )
    {
        lock (_startConsumerLock)
        {
            Connection.QueueDeclare(consumerConfig.QueueConfig, consumerConfig.QueueNameFactory);

            var customAsyncEventingBasicConsumer = new CustomAsyncEventingBasicConsumer(Connection.Channel);
            customAsyncEventingBasicConsumer.Received += Received;

            additionalConfigHandler?.Invoke(Connection, consumerConfig);

            var consumerTag = Connection.Channel.BasicConsume(
                queue: GetQueueName(consumerConfig.QueueConfig.QueueNameBase),
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
}