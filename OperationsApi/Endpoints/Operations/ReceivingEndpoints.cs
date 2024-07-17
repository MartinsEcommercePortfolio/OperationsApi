using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Receiving;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsApi.Endpoints.Operations;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/receiving/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapGet( "api/tasks/receiving/nextTask",
            static async ( IReceivingRepository repository ) =>
            await GetNextReceivingTask( repository ) );

        app.MapPost( "api/tasks/receiving/startTask",
            static async ( 
                    [FromQuery] Guid taskId,
                    [FromQuery] Guid trailerId,
                    [FromQuery] Guid dockId, 
                    [FromQuery] Guid areaId, 
                    HttpContext http, 
                    IReceivingRepository repository ) =>
            await StartReceivingTask( http.Employee(), taskId, trailerId, dockId, areaId, repository ) );

        app.MapPost( "api/tasks/receiving/startReceive",
            static async ( [FromQuery] Guid trailerId, [FromQuery] Guid palletId, HttpContext http, IReceivingRepository repository ) =>
            await StartReceivingPallet( http.Employee(), trailerId, palletId, repository ) );

        app.MapPost( "api/tasks/receiving/finishReceive",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid areaId, HttpContext http, IReceivingRepository repository ) =>
            await FinishReceivingPallet( http.Employee(), palletId, areaId, repository ) );

        app.MapGet( "api/tasks/receiving/finishTask",
            static async ( HttpContext http, IReceivingRepository repository ) =>
            await CompleteReceivingTask( http.Employee(), repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee.TaskAs<ReceivingTask>();

        return task.IsStarted && !task.IsFinished
            ? Results.Ok( ReceivingTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingOperationsWithTasks();
        var nextTask = receiving?.GetNextReceivingTask();
        
        return nextTask is not null
            ? Results.Ok( ReceivingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingOperationsWithTasks();

        var taskStarted = receiving is not null
            && employee.StartReceiving( receiving, taskId, trailerId, dockId, areaId );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReceivingTaskSummary.FromModel( employee.TaskAs<ReceivingTask>() ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingPallet( Employee employee, Guid trailerId, Guid palletId, IReceivingRepository repository )
    {
        var receivingStarted = employee
            .StartReceivingPallet( palletId, trailerId );
        
        return receivingStarted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishReceivingPallet( Employee employee, Guid areaId, Guid palletId, IReceivingRepository repository )
    {
        var receivingCompleted = employee
            .FinishReceivingPallet( areaId, palletId );

        return receivingCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( Employee employee, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingOperationsWithTasks();

        var taskCompleted = receiving is not null
            && employee.FinishReceiving( receiving );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}