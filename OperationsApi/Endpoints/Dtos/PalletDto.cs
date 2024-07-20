using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Dtos;

internal readonly record struct PalletDto(
    Guid PalletId,
    Guid ProductId,
    int ItemCount,
    double Length,
    double Width,
    double Height,
    double Weight,
    double ItemLength,
    double ItemWidth,
    double ItemHeight,
    double ItemWeight,
    List<Guid> ItemIds )
{
    internal static PalletDto FromModel( Pallet pallet )
    {
        throw new Exception( "PalletDto.ToModel is not implemented!" );
    }
}