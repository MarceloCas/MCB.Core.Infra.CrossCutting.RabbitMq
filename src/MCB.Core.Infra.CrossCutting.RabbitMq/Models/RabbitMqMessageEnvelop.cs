namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;

public record RabbitMqMessageEnvelop(
    Guid TenantId,
    Guid CorrelationId,
    string ExecutionUser,
    string SourcePlatform,
    DateTime TimeStamp,
    Type MessageType,
    byte[] Message
);