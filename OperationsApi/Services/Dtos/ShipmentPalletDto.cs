namespace OperationsApi.Services.Dtos;

internal readonly record struct ShipmentPalletDto(
    Guid PalletId,
    List<Guid> ItemIds );