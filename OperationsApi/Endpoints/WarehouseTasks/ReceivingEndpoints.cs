using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Receiving;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class ReceivingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/receiving/nextTask",
            static async ( IReceivingRepository repository ) =>
            await GetNextReceivingTask( repository ) );

        app.MapPost( "api/tasks/receiving/startReceiving",
            static async ( [FromQuery] Guid taskId, HttpContext http, IReceivingRepository repository ) =>
            await StartReceivingTask( http.Employee(), taskId, repository ) );

        app.MapPost( "api/tasks/receiving/unloadPallet",
            static async ( [FromQuery] Guid taskId, HttpContext http, IReceivingRepository repository ) =>
            await ReceiveUnloadedPallet( http.Employee(), taskId, repository ) );

        app.MapPost( "api/tasks/receiving/stagePallet",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid areaId, HttpContext http, IReceivingRepository repository ) =>
            await StageReceivedPallet( http.Employee(), palletId, areaId, repository ) );

        app.MapGet( "api/tasks/receiving/completeTask",
            static async ( HttpContext http, IReceivingRepository repository ) =>
            await CompleteReceivingTask( http.Employee(), repository ) );
    }
    
    static async Task<IResult> GetNextReceivingTask( IReceivingRepository repository )
    {
        ReceivingSection? receiving = await repository.GetReceivingSectionWithTasks();
        var nextTask = receiving?.GetNextReceivingTask();
        
        return nextTask is not null
            ? Results.Ok( ReceivingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartReceivingTask( Employee employee, Guid taskId, IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingSectionWithTasks();
        
        var taskStarted = receiving is not null
            && receiving.StartReceivingTask( employee, taskId )
            && await repository.SaveAsync();
        
        return taskStarted
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> ReceiveUnloadedPallet( Employee employee, Guid palletId, IReceivingRepository repository )
    {
        var receiving = await repository.GetReceivingSectionWithPallets();

        var palletReceived = receiving is not null
            && receiving.ReceiveUnloadedPallet( employee, palletId )
            && await repository.SaveAsync();
        
        return palletReceived
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> StageReceivedPallet( Employee employee, Guid palletId, Guid areaId, IReceivingRepository repository )
    {
        var staged = employee
            .GetTask<ReceivingTask>()
            .StagePallet( palletId, areaId );

        return staged && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> CompleteReceivingTask( Employee employee, IReceivingRepository repository )
    {
        var receiving = await repository
            .GetReceivingSectionWithTasks();

        var taskStarted = receiving is not null
            && receiving.CompleteReceivingTask( employee )
            && await repository.SaveAsync();

        return taskStarted
            ? Results.Ok( true )
            : Results.Problem();
    }
}