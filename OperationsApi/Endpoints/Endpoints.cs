namespace OperationsApi.Endpoints;

internal static class Endpoints
{
    internal static void MapEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/inventory", static async () => { } );
    }
}