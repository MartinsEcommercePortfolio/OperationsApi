using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Warehouse.Dtos;
using OperationsApi.Services;
using OperationsApi.Utilities;
using OperationsDomain.Employees.Models;
using OperationsDomain.Ordering;
using OperationsDomain.Outbound.Picking;

namespace OperationsApi.Endpoints.Warehouse;

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
                    HttpHandler httpHandler,
                    IOrderingRepository orderingRepository,
                    IPickingRepository repository ) =>
                await StartPickingTask(
                    http.GetPickingEmployee(),
                    taskId,
                    httpHandler,
                    orderingRepository,
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
                    IPickingRepository pickingRepository ) =>
                await FinishPickingTask(
                    http.GetPickingEmployee(),
                    pickingRepository ) );
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
    static async Task<IResult> StartPickingTask( PickingEmployee employee, Guid taskId, HttpHandler httpHandler, IOrderingRepository orderingRepository, IPickingRepository pickingRepository )
    {
        var ordering = await orderingRepository
            .GetOrderingOperationsAll();
        
        var picking = await pickingRepository
            .GetPickingOperationsWithTasks();

        var pickingStarted = ordering is not null
            && picking is not null
            && employee.StartPicking( picking, taskId );
        var pickingTask = employee.PickingTask;

        if (!pickingStarted || pickingTask is null)
            return Results.Problem();

        var order = ordering?.PendingOrders
            .FirstOrDefault( o => o.Id == pickingTask.WarehouseOrderId );
        
        if (order is null || !ordering!.StartPickingOrder( order ))
            return Results.Problem();

        // NOTIFY ORDERING API
        if (!await httpHandler.TryPut<bool>( Consts.OrderingUpdate, OrderUpdateDto.FromModel( order ) ))
            EndpointLogger.LogError( "OrderingDispatcher HandlePendingOrders() order update http call failed during execution." );
        
        return await orderingRepository.SaveAsync()
            ? Results.Ok( PickingTaskSummary.FromModel( pickingTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPickingLocation( PickingEmployee employee, Guid lineId, Guid rackingId, IPickingRepository repository )
    {
        var task = employee
            .PickPallet( lineId, rackingId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> PickItem( PickingEmployee employee, Guid rackingId, Guid palletId , IPickingRepository repository )
    {
        var picked = employee
            .PickPallet( rackingId, palletId );

        return picked && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishPickingLocation( PickingEmployee employee, Guid palletId, IPickingRepository repository )
    {
        var task = employee
            .StagePallet( palletId );

        return task && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishPickingTask( PickingEmployee employee, IPickingRepository pickingRepository )
    {
        var picking = await pickingRepository
            .GetPickingOperationsWithTasks();

        var taskCompleted = picking is not null
            && employee.FinishPicking( picking );
        
        return taskCompleted && await pickingRepository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}