using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Ordering.Dtos;
using OperationsDomain.Ordering;

namespace OperationsApi.Endpoints.Ordering;

internal static class OrderingEndpoints
{
    internal static void MapOrderingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/ordering/place",
            static async ( [FromBody] WarehouseOrderDto order, IOrderingRepository repository ) =>
                await PlaceOrder( order, repository ) );
    }

    static async Task<IResult> PlaceOrder( WarehouseOrderDto order, IOrderingRepository repository )
    {
        var ordering = await repository.GetOrderingOperationsWithOrderGroups();

        var orderPlaced = ordering is not null
            && ordering.AddOrder( order.ToModel() );

        return orderPlaced && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}