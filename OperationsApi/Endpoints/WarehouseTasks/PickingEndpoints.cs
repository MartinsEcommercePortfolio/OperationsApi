using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.WarehouseSections.Picking;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class PickingEndpoints
{
    internal static void MapPickingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/picking/nextTask",
            static async ( IPickingRepository repo ) =>
            await GetNextTask( repo ) );

        app.MapPost( "api/tasks/picking/resumeTask",
            static async ( HttpContext http, IPickingRepository repo ) =>
            await ResumePickingTask( http, repo ) );

        app.MapPost( "api/tasks/picking/startTask",
            static async ( [FromBody] Guid taskId, HttpContext http, IPickingRepository repo ) =>
            await StartPickingTask( taskId, http, repo ) );

        app.MapPost( "api/tasks/picking/getNextLocation",
            static async ( HttpContext http, IPickingRepository repo ) =>
            await GetNextPickLocation( http, repo ) );

        app.MapPost( "api/tasks/picking/startPickingLocation",
            static async ( [FromBody] Guid rackingId, HttpContext http, IPickingRepository repo ) =>
            await StartPickingLocation( rackingId, http, repo ) );

        app.MapPost( "api/tasks/picking/pickNextItem",
            static async ( [FromBody] Guid itemId, HttpContext http, IPickingRepository repo ) =>
            await PickNextItem( itemId, http, repo ) );

        app.MapPost( "api/tasks/picking/stagePickingOrder",
            static async ( [FromBody] Guid areaId, HttpContext http, IPickingRepository repo ) =>
            await StagePickingOrder( areaId, http, repo ) );
    }
    
    static async Task<IResult> GetNextTask( IPickingRepository picking )
    {
        var nextTask = await picking.GetNextPickingTask();
        return nextTask is not null
            ? Results.Ok( PickingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> ResumePickingTask( HttpContext http, IPickingRepository picking )
    {
        var taskModel = await picking.ResumePickingTask( http.Employee() );
        return taskModel is not null
            ? Results.Ok( PickingTaskSummary.FromModel( taskModel ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingTask( Guid taskId, HttpContext http, IPickingRepository picking )
    {
        var pickingLine = await picking.StartPickingTask( http.Employee(), taskId );
        return pickingLine is not null
            ? Results.Ok( PickingLineSummary.FromModel( pickingLine ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextPickLocation( HttpContext http, IPickingRepository picking )
    {
        var pickingLine = await picking.GetNextPick( http.Employee() );
        return pickingLine is not null
            ? Results.Ok( PickingLineSummary.FromModel( pickingLine ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingLocation( Guid rackingId, HttpContext http, IPickingRepository picking )
    {
        var itemsLeftToPick = await picking.ConfirmPickingLocation( http.Employee(), rackingId );
        return itemsLeftToPick is not null
            ? Results.Ok( itemsLeftToPick )
            : Results.Problem();
    }
    static async Task<IResult> PickNextItem( Guid itemId, HttpContext http, IPickingRepository picking )
    {
        var itemsLeftToPick = await picking.PickItem( http.Employee(), itemId );
        return itemsLeftToPick is not null
            ? Results.Ok( itemsLeftToPick )
            : Results.Problem();
    }
    static async Task<IResult> StagePickingOrder( Guid areaId, HttpContext http, IPickingRepository picking )
    {
        var orderStaged = await picking.StagePickingOrder( http.Employee(), areaId );
        return orderStaged
            ? Results.Ok( true )
            : Results.Problem();
    }
}