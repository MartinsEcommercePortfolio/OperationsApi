using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Intake.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Employees.Models;
using OperationsDomain.Operations.Intake;
using OperationsDomain.Operations.Shipping.Models;

namespace OperationsApi.Endpoints.Intake;

internal static class IntakeEndpoints
{
    // sim tells api delivery is here
    // api sends trailer to lot
    // api sends trailer to dock
    // api receives pallets
    // api creates intake task
    // api unloads
    // sim confirms trailer left
    
    internal static void MapIntakeEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/intake/receiveInventory",
            static async ( [FromQuery] Guid trailerId, [FromQuery] Guid palletId, HttpContext http, IReceivingRepository repository ) =>
            await StartReceivingPallet( http.GetReceivingEmployee(), trailerId, palletId, repository ) );
        
        app.MapGet( "api/tasks/intake/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.GetReceivingEmployee() ) );
        
        app.MapGet( "api/tasks/intake/nextTask",
            static async ( IReceivingRepository repository ) =>
            await GetNextReceivingTask( repository ) );

        app.MapPost( "api/tasks/intake/startTask",
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

    static async Task<IResult> ReceiveInventoryOrder( Trailer trailer )
    {
        return Results.Ok();
    }
    static IResult RefreshTask( ReceivingEmployee employee )
    {
        var refreshed = employee.ReceivingTask is not null
            && employee.ReceivingTask.IsStarted
            && !employee.ReceivingTask.IsFinished;

        return refreshed
            ? Results.Ok( IntakeTaskSummary.FromModel( employee.ReceivingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingOperationsWithTasks();
        var nextTask = receiving?.GetNextReceivingTask();
        
        return nextTask is not null
            ? Results.Ok( IntakeTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( ReceivingEmployee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingOperationsWithTasks();

        var taskStarted = receiving is not null
            && employee.StartReceiving( receiving, taskId, trailerId, dockId, areaId );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( IntakeTaskSummary.FromModel( employee.ReceivingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingPallet( ReceivingEmployee employee, Guid trailerId, Guid palletId, IReceivingRepository repository )
    {
        var receivingStarted = employee
            .StartReceivingPallet( palletId, trailerId );
        
        return receivingStarted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishReceivingPallet( ReceivingEmployee employee, Guid areaId, Guid palletId, IReceivingRepository repository )
    {
        var receivingCompleted = employee
            .FinishReceivingPallet( areaId, palletId );

        return receivingCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( ReceivingEmployee employee, IReceivingRepository repository )
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