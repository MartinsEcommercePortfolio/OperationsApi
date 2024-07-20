using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Receiving.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Employees.Models;
using OperationsDomain.Operations.Receiving;

namespace OperationsApi.Endpoints.Receiving;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/receiving/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.GetReceivingEmployee() ) );
        
        app.MapGet( "api/receiving/nextTask",
            static async ( IReceivingRepository repository ) =>
            await GetNextReceivingTask( repository ) );

        app.MapPost( "api/receiving/startTask",
            static async ( 
                    [FromQuery] Guid taskId,
                    [FromQuery] Guid trailerId,
                    [FromQuery] Guid dockId, 
                    [FromQuery] Guid areaId, 
                    HttpContext http, 
                    IReceivingRepository repository ) =>
            await StartReceivingTask( http.GetReceivingEmployee(), taskId, trailerId, dockId, areaId, repository ) );

        app.MapPost( "api/tasks/intake/startReceive",
            static async ( [FromQuery] Guid trailerId, [FromQuery] Guid palletId, HttpContext http, IReceivingRepository repository ) =>
            await StartReceivingPallet( http.GetReceivingEmployee(), trailerId, palletId, repository ) );

        app.MapPost( "api/tasks/intake/finishReceive",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid areaId, HttpContext http, IReceivingRepository repository ) =>
            await FinishReceivingPallet( http.GetReceivingEmployee(), palletId, areaId, repository ) );

        app.MapGet( "api/tasks/intake/finishTask",
            static async ( HttpContext http, IReceivingRepository repository ) =>
            await CompleteReceivingTask( http.GetReceivingEmployee(), repository ) );
    }
    
    static IResult RefreshTask( IntakeEmployee employee )
    {
        var refreshed = employee.ReceivingTask is not null
            && employee.ReceivingTask.IsStarted
            && !employee.ReceivingTask.IsFinished;

        return refreshed
            ? Results.Ok( ReceivingTaskSummary.FromModel( employee.ReceivingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingOperationsWithTasks();
        var nextTask = receiving?.GetNextTask();
        
        return nextTask is not null
            ? Results.Ok( ReceivingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( IntakeEmployee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingOperationsWithTasks();

        var taskStarted = receiving is not null
            && employee.StartIntake( receiving, taskId, trailerId, dockId, areaId );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReceivingTaskSummary.FromModel( employee.ReceivingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingPallet( IntakeEmployee employee, Guid trailerId, Guid palletId, IReceivingRepository repository )
    {
        var receivingStarted = employee
            .UnloadPallet( palletId, trailerId );
        
        return receivingStarted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishReceivingPallet( IntakeEmployee employee, Guid areaId, Guid palletId, IReceivingRepository repository )
    {
        var receivingCompleted = employee
            .StagePallet( areaId, palletId );

        return receivingCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( IntakeEmployee employee, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingOperationsWithTasks();

        var taskCompleted = receiving is not null
            && employee.FinishIntake( receiving );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}