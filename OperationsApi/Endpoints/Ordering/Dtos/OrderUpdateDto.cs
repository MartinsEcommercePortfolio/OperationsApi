using OperationsDomain.Ordering.Models;

namespace OperationsApi.Endpoints.Ordering.Dtos;

internal readonly record struct OrderUpdateDto(
    Guid OrderId,
    Guid OrderGroupId,
    OrderStatus Status )
{
    internal static OrderUpdateDto FromModel( WarehouseOrder model ) =>
        new( model.OrderId, model.OrderGroupId, model.Status );
}