using OperationsDomain.Ordering.Models;

namespace OperationsApi.Endpoints.Ordering.Dtos;

internal readonly record struct CartItemDto(
    Guid ItemId,
    int Quantity )
{
    internal WarehouseOrderItem ToModel() =>
        new( ItemId, Quantity );
    internal static List<WarehouseOrderItem> ToModels( List<CartItemDto> dtos )
    {
        List<WarehouseOrderItem> items = [];
        items.AddRange( from d in dtos select d.ToModel() );
        return items;
    }
}