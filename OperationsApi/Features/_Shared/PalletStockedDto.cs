namespace OperationsApi.Features._Shared;

internal readonly record struct PalletStockedDto(
    Guid PalletId,
    Guid RackingId );