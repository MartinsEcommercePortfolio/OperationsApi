using Microsoft.AspNetCore.Mvc;
using OperationsApi.Features._Shared;
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
            static async ( [FromBody] Guid taskId, HttpContext http, ReceivingRepository receiving ) =>
            await StartReceiving( taskId, http, receiving ) );

        app.MapPost( "api/tasks/receiving/unload",
            static async ( [FromBody] Guid taskId, HttpContext http, ReceivingRepository receiving ) =>
            await ReceiveTrailerPallet( taskId, http, receiving ) );

        app.MapPost( "api/tasks/receiving/stage",
            static async ( [FromBody] PalletStagedDto dto, HttpContext http, ReceivingRepository receiving ) =>
            await StageReceivedPallet( dto, http, receiving ) );
    }

    static async Task<IResult> GetNextReceivingTask( ReceivingRepository receiving )
    {
        var result = await receiving.GetNextReceivingTask();
        return result is not null
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> StartReceiving( Guid taskId, HttpContext http, ReceivingRepository receiving )
    {
        var result = await receiving.StartReceiving( http.Employee(), taskId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> ReceiveTrailerPallet( Guid palletId, HttpContext http, ReceivingRepository receiving )
    {
        var result = await receiving.ReceivePallet( http.Employee(), palletId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
    static async Task<IResult> StageReceivedPallet( PalletStagedDto dto, HttpContext http, ReceivingRepository receiving )
    {
        var result = await receiving.StagePallet( http.Employee(), dto.PalletId, dto.AreaId );
        return result
            ? Results.Ok( result )
            : Results.Problem();
    }
}