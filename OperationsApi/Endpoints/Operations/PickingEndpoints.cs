using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Operations.Picking;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsApi.Endpoints.Operations;

internal static class PickingEndpoints
{
    internal static void MapPickingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/picking/refreshPickingTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapGet( "api/tasks/picking/getNextPickingTask",
            static async ( IPickingRepository repository ) =>
            await GetNextTask( repository ) );

        app.MapPost( "api/tasks/picking/startPickingTask",
            static async ( [FromQuery] Guid taskId, HttpContext http, IPickingRepository repository ) =>
            await StartPickingTask( http.Employee(), taskId, repository ) );

        app.MapPost( "api/tasks/picking/startPick",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPickingRepository repository ) =>
            await StartPickingLocation( http.Employee(), palletId, rackingId, repository ) );

        app.MapPost( "api/tasks/picking/pickItem",
            static async ( [FromQuery] Guid itemId, HttpContext http, IPickingRepository repository ) =>
            await PickItem( http.Employee(), itemId, repository ) );

        app.MapPost( "api/tasks/picking/finishPick",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPickingRepository repository ) =>
            await FinishPickingLocation( http.Employee(), palletId, rackingId, repository ) );

        app.MapPost( "api/tasks/picking/stagePickingTask",
            static async ( [FromQuery] Guid areaId, HttpContext http, IPickingRepository repository ) =>
            await StageAndFinishPickingOrder( http.Employee(), areaId, repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee.GetTask<PickingTask>();
        
        return task.IsStarted && !task.IsCompleted
            ? Results.Ok( PickingTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextTask( IPickingRepository repository )
    {
        var picking = await repository
            .GetPickingOperationsWithTasks();
        
        var nextTask = picking?
            .GetNextPickingTask();
        
        return nextTask is not null
            ? Results.Ok( PickingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingTask( Employee employee, Guid taskId, IPickingRepository repository )
    {
        var picking = await repository
            .GetPickingOperationsWithTasks();
        
        var task = picking?
            .StartPickingTask( employee, taskId );

        var pickStarted = task is not null
            && task.IsStarted;
        
        return pickStarted && await repository.SaveAsync()
            ? Results.Ok( task )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingLocation( Employee employee, Guid palletId, Guid rackingId, IPickingRepository repository )
    {
        var task = employee
            .GetTask<PickingTask>()
            .StartPickingLocation( palletId, rackingId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> PickItem( Employee employee, Guid itemId, IPickingRepository repository )
    {
        var picked = employee
            .GetTask<PickingTask>()
            .PickItem( itemId );

        return picked && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();

    }
    static async Task<IResult> FinishPickingLocation( Employee employee, Guid palletId, Guid rackingId, IPickingRepository repository )
    {
        var task = employee
            .GetTask<PickingTask>()
            .FinishPickingLocation( palletId, rackingId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StageAndFinishPickingOrder( Employee employee, Guid areaId, IPickingRepository repository )
    {
        var picking = await repository
            .GetPickingOperationsWithTasks();

        var taskCompleted = picking is not null
            && picking.StageAndFinishPickingOrder( employee, areaId );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}