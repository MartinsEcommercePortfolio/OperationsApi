using OperationsDomain.Operations.Ordering.Models;

namespace OperationsApi.Services.Dtos;

internal readonly record struct ShipmentOrderDto(
    Guid CustomerOrderId,
    Guid CustomerOrderGroupId,
    string CustomerId,
    List<Guid> ShipmentPalletIds )
{
    internal static ShipmentOrderDto FromModel( WarehouseOrder model ) =>
        new( 
            model.OrderId, 
            model.OrderGroupId, 
            model.CustomerId ?? "Guest Customer", 
            model.Pallets.Select( static p => p.Id ).ToList() );
}