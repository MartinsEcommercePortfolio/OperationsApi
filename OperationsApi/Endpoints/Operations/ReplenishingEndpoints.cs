using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models.Variants;
using OperationsDomain.Warehouse.Operations.Replenishing;

namespace OperationsApi.Endpoints.Operations;

internal static class ReplenishingEndpoints
{
    internal static void MapReplenishingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/replenishing/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.GetReplenishingEmployee() ) );
        
        app.MapGet( "api/tasks/replenishing/nextTask",
            static async ( IReplenishingRepository repository ) =>
            await GetNextReplenishingTask( repository ) );

        app.MapPost( "api/tasks/replenishing/startTask",
            static async (
                    [FromQuery] Guid taskId,
                    [FromQuery] Guid rackingId,
                    [FromQuery] Guid palletId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await StartReplenishingTask(
                    http.GetReplenishingEmployee(),
                    taskId,
                    rackingId,
                    palletId,
                    repository ) );

        app.MapPost( "api/tasks/replenishing/finishTask",
            static async (
                    [FromQuery] Guid rackingId,
                    [FromQuery] Guid palletId,
                    HttpContext http,
                    IReplenishingRepository repository ) =>
                await FinishReplenishingTask(
                    http.GetReplenishingEmployee(),
                    rackingId,
                    palletId,
                    repository ) );
    }

    static IResult RefreshTask( ReplenishingEmployee employee )
    {
        var refreshed = employee.ReplenishingTask is not null
            && employee.ReplenishingTask.IsStarted
            && !employee.ReplenishingTask.IsFinished;

        return refreshed
            ? Results.Ok( ReplenishingTaskSummary.FromModel( employee.ReplenishingTask! ) )
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
    static async Task<IResult> StartReplenishingTask( ReplenishingEmployee employee, Guid taskId, Guid rackingId, Guid palletId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();
        
        var taskStarted = replenishing is not null
            && employee.StartReplenishing( replenishing, taskId, rackingId, palletId );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReplenishingTaskSummary.FromModel( employee.ReplenishingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> FinishReplenishingTask( ReplenishingEmployee employee, Guid rackingId, Guid palletId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();

        var replenished = replenishing is not null
            && employee.FinishReplenishing( replenishing, rackingId, palletId );

        return replenished && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}