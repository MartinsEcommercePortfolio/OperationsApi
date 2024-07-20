namespace OperationsApi.Endpoints.Inbound.Dtos;

internal readonly record struct InboundInventoryDto(
    Guid OrderId,
    List<InboundInventoryItemDto> Items );