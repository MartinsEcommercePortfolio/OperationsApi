namespace OperationsApi.Services.Dtos;

internal readonly record struct OrderUpdateDto(
    Guid OrderId,
    Guid OrderGroupId,
    int Status );