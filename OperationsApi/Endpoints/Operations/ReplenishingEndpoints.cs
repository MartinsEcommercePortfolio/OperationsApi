using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Replenishing;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsApi.Endpoints.Operations;

internal static class ReplenishingEndpoints
{
    internal static void MapReplenishingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/replenishing/refreshReplenishingTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapGet( "api/tasks/replenishing/getNextReplenishingTask",
            static async ( IReplenishingRepository repository ) =>
            await GetNextReplenishingTask( repository ) );

        app.MapPost( "api/tasks/replenishing/startReplenishingTask",
            static async (
                    [FromQuery] Guid taskId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await StartReplenishingTask(
                    http.Employee(),
                    taskId,
                    repository ) );

        app.MapPost( "api/tasks/replenishing/pickupReplen",
            static async (
                    [FromQuery] Guid rackingId,
                    [FromQuery] Guid palletId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await PickupReplenishingPallet(
                    http.Employee(),
                    rackingId,
                    palletId,
                    repository ) );

        app.MapPost( "api/tasks/replenishing/replenLocation",
            static async (
                    [FromQuery] Guid rackingId,
                    [FromQuery] Guid palletId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await ReplenishLocation(
                    http.Employee(),
                    rackingId,
                    palletId,
                    repository ) );

        app.MapPost( "api/tasks/replenishing/finishReplenishingTask",
            static async (
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await FinishReplenishingTask(
                    http.Employee(),
                    repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee
            .TaskAs<ReplenishingTask>();

        return task.IsStarted && !task.IsFinished
            ? Results.Ok( ReplenishingTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextReplenishingTask( IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();
        var nextTask = replenishing?
            .GetNextReplenishingTask();
        
        return nextTask is not null
            ? Results.Ok( ReplenishingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReplenishingTask( Employee employee, Guid taskId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();
        
        var replenishingTask = replenishing?
            .StartReplenishingTask( employee, taskId );

        var taskStarted = replenishingTask is not null
            && replenishingTask.IsStarted;
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReplenishingTaskSummary.FromModel( replenishingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> PickupReplenishingPallet( Employee employee, Guid rackingId, Guid palletId, IReplenishingRepository repository )
    {
        var replenPicked = employee
            .PickReplenishingPallet( rackingId, palletId );

        return replenPicked && await repository.SaveAsync()
            ? Results.Ok( replenPicked )
            : Results.Problem();
    }
    static async Task<IResult> ReplenishLocation( Employee employee, Guid rackingId, Guid palletId, IReplenishingRepository repository )
    {
        var replenished = employee
            .ReplenishLocation( rackingId, palletId );

        return replenished && await repository.SaveAsync()
            ? Results.Ok( replenished )
            : Results.Problem();
    }
    static async Task<IResult> FinishReplenishingTask( Employee employee, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();

        var replenished = replenishing is not null
            && replenishing.FinishReplenishingTask( employee );

        return replenished && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}