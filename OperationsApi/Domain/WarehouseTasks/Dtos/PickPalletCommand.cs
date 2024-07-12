namespace OperationsApi.Domain.WarehouseTasks.Dtos;

internal readonly record struct PickPalletCommand(
    Guid ItemId );