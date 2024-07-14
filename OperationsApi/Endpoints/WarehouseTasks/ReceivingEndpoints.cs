using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Features._Shared;
using OperationsApi.Utilities;
using OperationsDomain.Domain.WarehouseSections.Receiving;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/receiving/nextTask",
            static async ( IReceivingRepository repo ) =>
            await GetNextReceivingTask( repo ) );

        app.MapPost( "api/tasks/receiving/startTask",
            static async ( [FromBody] Guid taskId, HttpContext http, IReceivingRepository receiving ) =>
            await StartReceivingTask( taskId, http, receiving ) );

        app.MapPost( "api/tasks/receiving/unloadPallet",
            static async ( [FromBody] Guid taskId, HttpContext http, IReceivingRepository receiving ) =>
            await UnloadPallet( taskId, http, receiving ) );

        app.MapPost( "api/tasks/receiving/stagePallet",
            static async ( [FromBody] PalletStagedDto dto, HttpContext http, IReceivingRepository receiving ) =>
            await StagePallet( dto, http, receiving ) );

        app.MapGet( "api/tasks/receiving/completeTask",
            static async ( HttpContext http, IReceivingRepository receiving ) =>
            await CompleteReceivingTask( http, receiving ) );
    }
    
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository receiving )
    {
        var receivingTask = await receiving.GetNextReceivingTask();
        return receivingTask is not null
            ? Results.Ok( ReceivingTaskSummary.FromModel( receivingTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( Guid taskId, HttpContext http, IReceivingRepository receiving )
    {
        bool taskStarted = await receiving.StartReceivingTask( http.Employee(), taskId );
        return taskStarted
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> UnloadPallet( Guid palletId, HttpContext http, IReceivingRepository receiving )
    {
        bool palletReceived = await receiving.ReceivePallet( http.Employee(), palletId );
        return palletReceived
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StagePallet( PalletStagedDto dto, HttpContext http, IReceivingRepository receiving )
    {
        bool palletStaged = await receiving.StagePallet( http.Employee(), dto.PalletId, dto.AreaId );
        return palletStaged
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( HttpContext http, IReceivingRepository receiving )
    {
        bool completed = await receiving.CompleteReceivingTask( http.Employee() );
        return completed
            ? Results.Ok( true )
            : Results.Problem();
    }
}