namespace OperationsApi.Domain.Tasks.Dtos;

internal readonly record struct PlacePalletRequest(
    Guid UserId,
    Guid TaskId,
    Guid PalletId );