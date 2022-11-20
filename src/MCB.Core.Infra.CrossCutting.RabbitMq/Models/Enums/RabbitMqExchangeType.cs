namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models.Enums;

public enum RabbitMqExchangeType
{ 
    Direct = 1,
    Topic = 2,
    Header = 3,
    Fanout = 4
}

