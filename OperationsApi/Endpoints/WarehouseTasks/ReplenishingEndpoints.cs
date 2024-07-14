using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Replenishing;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class ReplenishingEndpoints
{
    internal static void MapReplenishingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/replenishing/nextTask",
            static async ( IReplenishingRepository repository ) =>
            await GetNextReplenishingTask( repository ) );

        app.MapPost( "api/tasks/replenishing/startTask",
            static async ( [FromQuery] Guid taskId, HttpContext http, IReplenishingRepository repository ) =>
            await StartReplenishingTask( http.Employee(), taskId, repository ) );

        app.MapPost( "api/tasks/replenishing/pickup",
            static async ( [FromQuery] Guid palletId, HttpContext http, IReplenishingRepository repository ) =>
            await PickupReplenishingPallet( http.Employee(), palletId, repository ) );

        app.MapPost( "api/tasks/replenishing/replenish",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IReplenishingRepository repository ) =>
            await ReplenishLocation( http.Employee(), palletId, rackingId, repository ) );
    }

    static async Task<IResult> GetNextReplenishingTask( IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingSectionWithTasks();
        var nextTask = replenishing?
            .GetNextReplenishingTask();
        
        return nextTask is not null
            ? Results.Ok( ReplenishingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReplenishingTask( Employee employee, Guid taskId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingSectionWithTasks();
        
        var replenishingTask = replenishing?
            .StartReplenishingTask( employee, taskId );
        
        var taskStarted = replenishingTask is not null
            && replenishingTask.IsStarted
            && await repository.SaveAsync();
        
        return taskStarted
            ? Results.Ok( ReplenishingTaskSummary.FromModel( replenishingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> PickupReplenishingPallet( Employee employee, Guid palletId, IReplenishingRepository repository )
    {
        var dropOffLocation = employee
            .GetTask<ReplenishingTask>()
            .PickupReplenishingPallet( palletId );

        return dropOffLocation is not null && await repository.SaveAsync()
            ? Results.Ok( dropOffLocation )
            : Results.Problem();
    }
    static async Task<IResult> ReplenishLocation( Employee employee, Guid palletId, Guid rackingId, IReplenishingRepository repository )
    {
        var replenishing = await repository
            .GetReplenishingSectionWithTasks();

        var replenished = replenishing is not null
            && replenishing.ReplenishLocation( employee, palletId, rackingId )
            && employee.GetTask<ReplenishingTask>().IsCompleted
            && await repository.SaveAsync();

        return replenished
            ? Results.Ok( true )
            : Results.Problem();
    }
}