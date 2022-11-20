using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers.Interfaces;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;
public class RabbitMqQueuePublisherBase
    : IRabbitMqPublisher
{

    // Constructors
    public RabbitMqQueuePublisherBase(

    )
    {

    }

    // Public Methods
    public Task PublishAsync<TSubject>(TSubject subject, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    public Task PublishAsync<TSubject>(TSubject subject, Type subjectBaseType, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Type subscriberType, Type subjectType)
    {
        throw new NotImplementedException();
    }
    public void Subscribe<TSubscriber>(Type subjectType)
    {
        throw new NotImplementedException();
    }
    public void Subscribe<TSubscriber, TSubject>() where TSubscriber : ISubscriber<TSubject>
    {
        throw new NotImplementedException();
    }
}
