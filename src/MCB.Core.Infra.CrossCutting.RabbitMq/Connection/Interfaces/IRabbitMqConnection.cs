using RabbitMQ.Client;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
public interface IRabbitMqConnection
    : IDisposable
{
    // Properties
    IModel Channel { get; }
    bool IsOpen { get; }
    DateTime? LastOpenDate { get; }

    // Methods
    IBasicProperties CreateBasicProperties();
}
