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
    }

    static async Task<IResult> PlaceOrder( WarehouseOrderDto order, IShippingRepository shippingRepository, IOrderingRepository orderingRepository )
    {
        var shipping = await shippingRepository.GetShippingOperationsWithRoutes();

        var route = shipping?.GetRouteByPos( order.PosX, order.PosY );
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
}