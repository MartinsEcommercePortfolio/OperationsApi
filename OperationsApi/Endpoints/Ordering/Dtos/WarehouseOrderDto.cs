using OperationsDomain.Ordering.Models;

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
    internal WarehouseOrder ToModel() =>
        new( OrderId, OrderGroupId, CustomerId, DateCreated, PosX, PosY, CartItemDto.ToModels( Items ) );
}