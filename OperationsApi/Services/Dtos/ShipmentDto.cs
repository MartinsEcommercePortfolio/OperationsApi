using OperationsDomain.Ordering.Types;
using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsApi.Services.Dtos;

internal readonly record struct ShipmentDto(
    Guid TrailerId,
    Guid RouteId,
    List<ShipmentOrderDto> Orders,
    List<ShipmentPalletDto> Pallets )
{
    /*internal static ShipmentDto FromModels( Trailer trailer, Shipment shipment, List<WarehouseOrder> orders, List<Pallet> pallets )
    {
        return new ShipmentDto(trailer.Id, shipment.Route.Id)
    }*/
}