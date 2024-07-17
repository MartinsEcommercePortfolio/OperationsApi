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
        app.MapGet( "api/tasks/replenishing/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
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
                    http.Employee(),
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
                    http.Employee(),
                    rackingId,
                    palletId,
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
    static async Task<IResult> StartReplenishingTask( Employee employee, Guid taskId, Guid rackingId, Guid palletId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingOperationsWithTasks();
        
        var taskStarted = replenishing is not null
            && employee.StartReplenishing( replenishing, taskId, rackingId, palletId );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReplenishingTaskSummary.FromModel( employee.TaskAs<ReplenishingTask>() ) )
            : Results.Problem();
    }
    static async Task<IResult> FinishReplenishingTask( Employee employee, Guid rackingId, Guid palletId, IReplenishingRepository repository )
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