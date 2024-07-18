using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Ordering.Dtos;
using OperationsDomain.Ordering;
using OperationsDomain.Shipping;
using OperationsDomain.Warehouse.Operations.Picking;

namespace OperationsApi.Endpoints.Ordering;

internal static class OrderingEndpoints
{
    internal static void MapOrderingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/ordering/place",
            static async (
                    [FromBody] WarehouseOrderDto order,
                    IOrderingRepository orderingRepository,
                    IPickingRepository pickingRepository,
                    IShippingRepository shippingRepository ) =>
                await PlaceOrder(
                    order,
                    orderingRepository,
                    pickingRepository,
                    shippingRepository) );
    }

    static async Task<IResult> PlaceOrder( WarehouseOrderDto order, IOrderingRepository orderingRepo, IPickingRepository pickingRepo, IShippingRepository shippingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsForNewOrder();
        var picking = await pickingRepo.GetPickingOperationsWithTasks();
        var shipping = await shippingRepo.GetShippingOperationsWithRoutes();

        if (ordering is null || shipping is null || picking is null)
            return Results.Problem( "Failed to get repositories." );

       /* var route = shipping.GetRouteByPos( order.PosX, order.PosY );
        if (route is null)
            return Results.NotFound( "No route found." );

        var orderPlaced = ordering is not null
            && ordering.AddOrder( order.ToModel( route ) )
            && await orderingRepository.SaveAsync();

        return orderPlaced
            ? Results.Ok( true )
            : Results.Problem();*/

       return Results.Problem();
    }
}