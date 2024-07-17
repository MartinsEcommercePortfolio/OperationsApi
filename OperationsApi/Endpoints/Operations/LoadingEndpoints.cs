using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Loading;
using OperationsDomain.Warehouse.Operations.Loading.Models;

namespace OperationsApi.Endpoints.Operations;

public static class LoadingEndpoints
{
    internal static void MapReceivingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/loading/refreshTask",
            static ( HttpContext http ) =>
                RefreshTask( http.Employee() ) );

        app.MapGet( "api/tasks/loading/nextTask",
            static async ( ILoadingRepository repository ) =>
                await GetNextLoadingTask( repository ) );

        app.MapPost( "api/tasks/loading/startTask", static async (
                [FromQuery] Guid taskId,
                [FromQuery] Guid trailerId,
                [FromQuery] Guid dockId,
                [FromQuery] Guid areaId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await StartLoadingTask(
                http.Employee(), taskId, trailerId, dockId, areaId, repository ) );

        app.MapPost( "api/tasks/loading/startLoad", static async (
                [FromQuery] Guid areaId,
                [FromQuery] Guid palletId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await StartLoadingPallet( 
                http.Employee(), areaId, palletId, repository ) );

        app.MapPost( "api/tasks/loading/finishLoad", static async (
                [FromQuery] Guid trailerId,
                [FromQuery] Guid palletId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await FinishLoadingPallet( 
                http.Employee(), trailerId, palletId, repository ) );

        app.MapPost( "api/tasks/loading/finishTask", static async (
                HttpContext http,
                ILoadingRepository repository ) =>
            await FinishLoadingTask(
                http.Employee(), repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee.TaskAs<LoadingTask>();

        return task.IsStarted && !task.IsFinished
            ? Results.Ok( LoadingTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> GetNextLoadingTask( ILoadingRepository repository )
    {
        var loading = await repository.GetLoadingOperationsWithTasks();
        var nextTask = loading?.GetNextTask();

        return nextTask is not null
            ? Results.Ok( LoadingTaskSummary.FromModel( nextTask ) )
            : Results.Problem();
    }
    static async Task<IResult> StartLoadingTask( Employee employee, Guid taskId, Guid trailerId, Guid dockId, Guid areaId, ILoadingRepository repository )
    {
        var loading = await repository
            .GetLoadingOperationsWithTasks();

        var taskStarted = loading is not null
            && employee.StartLoading( loading, taskId, trailerId, dockId, areaId );

        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( LoadingTaskSummary.FromModel( employee.TaskAs<LoadingTask>() ) )
            : Results.Problem();
    }
    static async Task<IResult> StartLoadingPallet( Employee employee, Guid areaId, Guid palletId, ILoadingRepository repository )
    {
        var loadingStarted = employee
            .StartLoadingPallet( palletId, areaId );

        return loadingStarted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishLoadingPallet( Employee employee, Guid trailerId, Guid palletId, ILoadingRepository repository )
    {
        var receivingCompleted = employee
            .FinishLoadingPallet( trailerId, palletId );

        return receivingCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishLoadingTask( Employee employee, ILoadingRepository repository )
    {
        var loading = await repository
            .GetLoadingOperationsWithTasks();

        var taskCompleted = loading is not null
            && employee.FinishLoading( loading );

        return taskCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}