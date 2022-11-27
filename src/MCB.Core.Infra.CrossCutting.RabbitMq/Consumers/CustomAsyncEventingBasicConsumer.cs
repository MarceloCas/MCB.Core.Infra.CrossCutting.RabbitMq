using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Consumers;
public class CustomAsyncEventingBasicConsumer
    : AsyncEventingBasicConsumer
{
    // Constructors
    public CustomAsyncEventingBasicConsumer(
        IModel model
    ) : base(model)
    {

    }
}
