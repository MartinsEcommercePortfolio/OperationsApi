namespace OperationsApi.Endpoints.Receiving;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        
    }

    static IResult ReceiveTrailer()
    {
        // on sim trailer request come in
        // return dock or parking waiting space
        return Results.Problem();
    }
    static IResult ReceiveTrailerPallets()
    {
        // on trailer docked
        return Results.Problem();
    }
}