namespace OperationsApi.Endpoints.Shipping;

internal static class ShippingEndpoints
{
    internal static void MapShippingEndpoints( this IEndpointRouteBuilder app )
    {
        
    }

    static async Task<IResult> ReceiveShipment()
    {
        return Results.Problem();
    }
    static async Task<IResult> DispatchShipment()
    {
        return Results.Problem();
    }
}