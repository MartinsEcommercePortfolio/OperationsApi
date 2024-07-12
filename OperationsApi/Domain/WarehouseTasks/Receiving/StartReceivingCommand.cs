namespace OperationsApi.Domain.WarehouseTasks.Receiving;

internal readonly record struct StartReceivingCommand(
    Guid TrailerId,
    Guid LoadId );