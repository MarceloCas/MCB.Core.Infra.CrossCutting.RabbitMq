namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;
public record RabbitMqMessageErrorEnvelop(
    Guid TenantId,
    Guid CorrelationId,
    string ExecutionUser,
    string SourcePlatform,
    DateTime TimeStamp,
    Type MessageType,
    Exception Exception,
    byte[] Message
);
