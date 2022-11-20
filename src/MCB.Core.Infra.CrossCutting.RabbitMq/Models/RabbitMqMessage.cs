namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;

public record RabbitMqMessage(
    Guid TenantId,
    Guid CorrelationId,
    string ExecutionUser,
    string SourcePlatform,
    byte[] Message
);