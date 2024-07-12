using Microsoft.AspNetCore.Mvc;
using OperationsApi.Utilities;

namespace OperationsApi.Features.WarehouseTasks.Receiving;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/receiving/next",
            static async ( ReceivingRepository repo ) =>
            await GetNextReceivingTask( repo ) );

        app.MapPost( "api/tasks/receiving/start",
            static async ( [FromBody] Guid taskId, HttpContext http, ReceivingRepository repo ) =>
            await StartReceiving( taskId, http, repo ) );

        app.MapPost( "api/tasks/receiving/unload",
            static async ( [FromBody] Guid taskId, HttpContext http, ReceivingRepository repo ) =>
            await UnloadTrailerPallet( taskId, http, repo ) );

        app.MapPost( "api/tasks/receiving/stage",
            static async ( [FromQuery] Guid taskId, [FromQuery] Guid areaId, HttpContext http, ReceivingRepository repo ) =>
            await StageReceivedPallet( taskId, areaId, http, repo ) );
    }

    static async Task<IResult> GetNextReceivingTask( ReceivingRepository repo )
    {
        var result = await repo.GetNextReceivingTask();
        return result is not null
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> StartReceiving( Guid taskId, HttpContext http, ReceivingRepository repo )
    {
        var result = await repo.StartReceiving( http.GetEmployee(), taskId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> UnloadTrailerPallet( Guid palletId, HttpContext http, ReceivingRepository repo )
    {
        var result = await repo.UnloadTrailerPallet( http.GetEmployee(), palletId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> StageReceivedPallet( Guid palletId, Guid areaId, HttpContext http, ReceivingRepository repo )
    {
        var result = await repo.StageReceivedPallet( http.GetEmployee(), palletId, areaId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
}