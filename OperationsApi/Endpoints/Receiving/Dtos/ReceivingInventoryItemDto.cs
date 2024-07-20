namespace OperationsApi.Endpoints.Receiving.Dtos;

internal readonly record struct ReceivingInventoryItemDto(
    Guid InventoryId,
    Guid ProductId,
    int Quantity,
    double Length,
    double Width,
    double Height,
    double Weight );