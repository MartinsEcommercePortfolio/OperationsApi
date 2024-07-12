namespace OperationsApi.Domain.Tasks.Receiving;

internal readonly record struct StartReceivingCommand(
    Guid TrailerId,
    Guid LoadId );