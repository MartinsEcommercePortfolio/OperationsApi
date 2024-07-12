using Microsoft.AspNetCore.Mvc;
using OperationsApi.Utilities;

namespace OperationsApi.Features.WarehouseTasks.Putaways;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/putaways/start",
            static async ( [FromBody] Guid palletId, HttpContext http, PutawayRepository putaways ) =>
            await GetNextReceivingTask( palletId, http, putaways ) );
    }

    static async Task<IResult> GetNextReceivingTask( Guid palletId, HttpContext http, PutawayRepository putaways )
    {
        var result = await putaways.StartPutaway( http.Employee(), palletId );
        return result is not null
            ? Results.Ok( result )
            : Results.Problem();
    }
}