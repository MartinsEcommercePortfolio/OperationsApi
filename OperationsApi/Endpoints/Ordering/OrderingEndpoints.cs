using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Ordering.Dtos;
using OperationsDomain.Operations.Ordering;

namespace OperationsApi.Endpoints.Ordering;

internal static class OrderingEndpoints
{
    internal static void MapOrderingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/ordering/place",
            static async (
                    [FromBody] WarehouseOrderDto order,
                    IOrderingRepository orderingRepository) =>
                await PlaceOrder(
                    order,
                    orderingRepository ) );
    }

    static async Task<IResult> PlaceOrder( WarehouseOrderDto order, IOrderingRepository orderingRepo )
    {
        var ordering = await orderingRepo.GetOrderingOperationsAll();

        var orderPlaced = ordering is not null
            && ordering.AddNewOrder( order.ToModel() );

        return orderPlaced && await orderingRepo.SaveAsync()
            ? Results.Ok()
            : Results.Problem();
    }
}