using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Ordering;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Picking;

namespace OperationsApi.Endpoints.Operations;

internal static class PickingEndpoints
{
    internal static void MapPickingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/picking/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.GetPickingEmployee() ) );
        
        app.MapGet( "api/tasks/picking/nextTask",
            static async ( IPickingRepository repository ) =>
            await GetNextTask( repository ) );

        app.MapPost( "api/tasks/picking/startTask",
            static async (
                    [FromQuery] Guid taskId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await StartPickingTask(
                    http.GetPickingEmployee(),
                    taskId,
                    repository ) );

        app.MapPost( "api/tasks/picking/startPick",
            static async (
                    [FromQuery] Guid lineId,
                    [FromQuery] Guid rackingId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await StartPickingLocation(
                    http.GetPickingEmployee(),
                    lineId,
                    rackingId,
                    repository ) );

        app.MapPost( "api/tasks/picking/pickItem",
            static async (
                    [FromQuery] Guid rackingId,
                    [FromQuery] Guid itemId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await PickItem(
                    http.GetPickingEmployee(),
                    rackingId,
                    itemId,
                    repository ) );

        app.MapPost( "api/tasks/picking/finishPick",
            static async (
                    [FromQuery] Guid lineId,
                    HttpContext http,
                    IPickingRepository repository ) =>
                await FinishPickingLocation(
                    http.GetPickingEmployee(),
                    lineId,
                    repository ) );

        app.MapPost( "api/tasks/picking/finishTask",
            static async (
                    HttpContext http,
                    IPickingRepository pickingRepository,
                    IOrderingRepository orderingRepository ) =>
                await StageFinishPickingOrder(
                    http.GetPickingEmployee(),
                    pickingRepository,
                    orderingRepository ) );
    }

    static IResult RefreshTask( PickingEmployee employee )
    {
        bool refreshed = employee.PickingTask is not null
            && employee.PickingTask.IsStarted
            && !employee.PickingTask.IsFinished;

        return refreshed
            ? Results.Ok( PickingTaskSummary.FromModel( employee.PickingTask! ) )
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
    static async Task<IResult> StartPickingTask( PickingEmployee employee, Guid taskId, IPickingRepository repository )
    {
        var picking = await repository
            .GetPickingOperationsWithTasks();
        
        var pickStarted = picking is not null
            && employee.StartPicking( picking, taskId );
        
        return pickStarted && await repository.SaveAsync()
            ? Results.Ok( PickingTaskSummary.FromModel( employee.PickingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingLocation( PickingEmployee employee, Guid lineId, Guid rackingId, IPickingRepository repository )
    {
        var task = employee
            .StartPickingPallet( lineId, rackingId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> PickItem( PickingEmployee employee, Guid rackingId, Guid palletId , IPickingRepository repository )
    {
        var picked = employee
            .StartPickingPallet( rackingId, palletId );

        return picked && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishPickingLocation( PickingEmployee employee, Guid palletId, IPickingRepository repository )
    {
        var task = employee
            .FinishPickingPallet( palletId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StageFinishPickingOrder( PickingEmployee employee, IPickingRepository pickingRepository, IOrderingRepository orderingRepository )
    {
        var picking = await pickingRepository
            .GetPickingOperationsWithTasks();

        var taskCompleted = picking is not null
            && employee.FinishPicking( picking );

        if (!taskCompleted)
            return Results.Problem();

        var ordering = await orderingRepository
            .GetOrderingOperationsForNewOrder();

        var orderUpdated = ordering is not null
            && ordering.CompleteOrder( employee.PickingTask!.WarehouseOrderId );

        return orderUpdated
            && await pickingRepository.SaveAsync()
            && await orderingRepository.SaveAsync()
                ? Results.Ok( true )
                : Results.Problem();
    }
}