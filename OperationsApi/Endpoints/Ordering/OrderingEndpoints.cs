using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Ordering.Dtos;
using OperationsDomain.Ordering;
using OperationsDomain.Shipping;

namespace OperationsApi.Endpoints.Ordering;

internal static class OrderingEndpoints
{
    internal static void MapOrderingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/ordering/place",
            static async (
                    [FromBody] WarehouseOrderDto order,
                    IShippingRepository shippingRepository,
                    IOrderingRepository orderingRepository ) =>
                await PlaceOrder(
                    order,
                    shippingRepository,
                    orderingRepository ) );
        
        app.MapPost( "api/ordering/ship",
            static async (
                    [FromBody] Guid orderGroupId,
                    IOrderingRepository orderingRepository,
                    IShippingRepository shippingRepository ) =>
                await ShipOrderGroup(
                    orderGroupId,
                    orderingRepository,
                    shippingRepository ) );
    }

    static async Task<IResult> PlaceOrder( WarehouseOrderDto order, IShippingRepository shippingRepository, IOrderingRepository orderingRepository )
    {
        var shipping = await shippingRepository.GetShippingOperationsWithRoutes();

        var route = shipping?.GetRoute( order.PosX, order.PosY );
        if (route is null)
            return Results.NotFound( "No route found." );
        
        var ordering = await orderingRepository.GetOrderingOperationsForNewOrder();

        var orderPlaced = ordering is not null
            && ordering.AddOrder( order.ToModel( route ) )
            && await orderingRepository.SaveAsync();

        return orderPlaced
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> ShipOrderGroup( Guid orderGroupId, IOrderingRepository orderingRepository, IShippingRepository shippingRepository )
    {
        /*var ordering = await orderingRepository
            .GetOrderingOperationsForNewOrder();

        var orderGroup = null; ordering?.CompleteShippingGroup( orderGroupId );
        if (orderGroup is null)
            return Results.Problem();

        var shipping = await shippingRepository
            .GetShippingOperationsWithRoutes();

        var shipped = shipping is not null
            && shipping.ShipOrders( orderGroup.ShippingTrailer )
            && await orderingRepository.SaveAsync()
            && await shippingRepository.SaveAsync();

        if (shipped)
        {
            // TODO: send emails
        }

        return shipped
            ? Results.Ok()
            : Results.Problem();*/
        return Results.Problem();
    }
}