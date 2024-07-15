using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Receiving;
using OperationsDomain.Domain.WarehouseSections.Receiving.Models;

namespace OperationsApi.Endpoints.WarehouseTasks;

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

        app.MapPost( "api/tasks/receiving/unloadPallet",
            static async ( [FromQuery] Guid trailerId, [FromQuery] Guid palletId, HttpContext http, IReceivingRepository repository ) =>
            await ReceiveUnloadedPallet( http.Employee(), trailerId, palletId, repository ) );

        app.MapPost( "api/tasks/receiving/stagePallet",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid areaId, HttpContext http, IReceivingRepository repository ) =>
            await StageReceivedPallet( http.Employee(), palletId, areaId, repository ) );

        app.MapGet( "api/tasks/receiving/completeTask",
            static async ( HttpContext http, IReceivingRepository repository ) =>
            await CompleteReceivingTask( http.Employee(), repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee.GetTask<ReceivingTask>();

        return task.IsStarted && !task.IsCompleted
            ? Results.Ok( ReceivingTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository repository )
    {
        ReceivingSection? receiving = await repository.GetReceivingSectionWithTasks();
        var nextTask = receiving?.GetNextReceivingTask();
        
        return nextTask is not null
            ? Results.Ok( ReceivingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingSectionWithTasks();

        var task = receiving?
            .StartReceivingTask( employee, taskId, trailerId, dockId, areaId );

        var taskStarted = task is not null
            && task.IsStarted;
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( ReceivingTaskSummary.FromModel( task! ) )
            : Results.Problem();
    }
    static async Task<IResult> ReceiveUnloadedPallet( Employee employee, Guid trailerId, Guid palletId, IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingSectionWithPallets();

        var palletReceived = receiving is not null
            && receiving.ReceiveUnloadedPallet( employee, trailerId, palletId )
            && await repository.SaveAsync();
        
        return palletReceived
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StageReceivedPallet( Employee employee, Guid palletId, Guid areaId, IReceivingRepository repository )
    {
        var palletStaged = employee
            .GetTask<ReceivingTask>()
            .StagePallet( palletId, areaId );

        return palletStaged && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( Employee employee, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingSectionWithTasks();

        var taskCompleted = receiving is not null
            && receiving.CompleteReceivingTask( employee );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}