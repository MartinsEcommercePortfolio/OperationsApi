using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Picking;
using OperationsDomain.Warehouse.Operations.Picking.Models;
using OperationsDomain.Warehouse.Operations.Replenishing;

namespace OperationsApi.Endpoints.Operations;

internal static class PickingEndpoints
{
    internal static void MapPickingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/picking/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapGet( "api/tasks/picking/nextTask",
            static async ( IPickingRepository repository ) =>
            await GetNextTask( repository ) );

        app.MapPost( "api/tasks/picking/startTask",
            static async (
                    [FromQuery] Guid taskId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await StartPickingTask(
                    http.Employee(),
                    taskId,
                    repository ) );

        app.MapPost( "api/tasks/picking/startPick",
            static async (
                    [FromQuery] Guid lineId,
                    [FromQuery] Guid rackingId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await StartPickingLocation(
                    http.Employee(),
                    lineId,
                    rackingId,
                    repository ) );

        app.MapPost( "api/tasks/picking/pickItem",
            static async (
                    [FromQuery] Guid itemId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await PickItem(
                    http.Employee(),
                    itemId,
                    repository ) );

        app.MapPost( "api/tasks/picking/finishPick",
            static async (
                    [FromQuery] Guid lineId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await FinishPickingLocation(
                    http.Employee(),
                    lineId,
                    repository ) );

        app.MapPost( "api/tasks/picking/finishTask",
            static async (
                    [FromQuery] Guid areaId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await StageAndFinishPickingOrder(
                    http.Employee(),
                    areaId,
                    repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee
            .TaskAs<PickingTask>();
        
        return task.IsStarted && !task.IsFinished
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
        
        var pickStarted = picking is not null
            && employee.StartPicking( picking, taskId );
        
        return pickStarted && await repository.SaveAsync()
            ? Results.Ok( PickingTaskSummary.FromModel( employee.TaskAs<PickingTask>() ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingLocation( Employee employee, Guid lineId, Guid rackingId, IPickingRepository repository )
    {
        var task = employee
            .StartPickingLine( lineId, rackingId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> PickItem( Employee employee, Guid itemId, IReplenishingRepository repository )
    {
        var replenishing = await repository.GetReplenishingOperationsWithEventsAndTasks();
        
        var picked = replenishing is not null
            && employee.PickItem( replenishing, itemId );

        return picked && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishPickingLocation( Employee employee, Guid lineId, IPickingRepository repository )
    {
        var task = employee
            .FinishPickingLine( lineId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StageAndFinishPickingOrder( Employee employee, Guid areaId, IPickingRepository repository )
    {
        var picking = await repository
            .GetPickingOperationsWithTasks();

        var taskCompleted = picking is not null
            && employee.FinishPicking( picking, areaId );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}