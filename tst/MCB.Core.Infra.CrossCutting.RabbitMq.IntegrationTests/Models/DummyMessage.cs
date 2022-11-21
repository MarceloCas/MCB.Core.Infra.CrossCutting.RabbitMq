namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;

public class DummyMessage
{
    public System.DateTime CreationDate { get; }

	public DummyMessage()
	{
		CreationDate = System.DateTime.UtcNow;
	}
}
