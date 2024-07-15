using System.ComponentModel;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct ShipmentDto(
    Guid TrailerId,
    string TrailerNumber,
    string DockNumber,
    List<PalletDto> Loads )
{
    public Trailer ToModel( Dock? dock )
    {
        throw new Exception( "ShipmentDto.ToModel not implemented!" );
        /*Trailer trailer = new() {
            Id = TrailerId,
            Number = TrailerNumber,
            Dock = dock
        };
        return trailer;*/
    }
}