namespace OperationsApi.Services.Dtos;

internal readonly record struct OrderDelayedDto(
    Guid OrderId,
    Guid OrderGroupId );