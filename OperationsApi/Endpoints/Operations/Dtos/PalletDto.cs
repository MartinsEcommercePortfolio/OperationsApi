using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsApi.Endpoints.Operations.Dtos;

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
    internal Pallet ToModel( Trailer trailer )
    {
        throw new Exception( "PalletDto.ToModel is not implemented!" );
    }
}