using OperationsDomain.Ordering.Types;
using OperationsDomain.Shipping.Models;

namespace OperationsApi.Endpoints.Ordering.Dtos;

internal readonly record struct WarehouseOrderDto(
    Guid OrderId,
    Guid OrderGroupId,
    string? CustomerId,
    DateTime DateCreated,
    int PosX,
    int PosY,
    List<CartItemDto> Items )
{
    internal WarehouseOrder ToModel( ShippingRoute shippingRoute ) =>
        new( OrderId, OrderGroupId, shippingRoute, CustomerId, DateCreated, PosX, PosY, CartItemDto.ToModels( Items ) );
}