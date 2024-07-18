using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees.Models.Variants;
using OperationsDomain.Warehouse.Operations.Loading;

namespace OperationsApi.Endpoints.Operations;

public static class LoadingEndpoints
{
    internal static void MapLoadingEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/loading/refreshTask",
            static ( HttpContext http ) =>
                RefreshTask( http.GetLoadingEmployee() ) );

        app.MapGet( "api/tasks/loading/nextTask",
            static async ( ILoadingRepository repository ) =>
                await GetNextLoadingTask( repository ) );

        app.MapPost( "api/tasks/loading/startTask", static async (
                [FromQuery] Guid taskId,
                [FromQuery] Guid trailerId,
                [FromQuery] Guid dockId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await StartLoadingTask(
                http.GetLoadingEmployee(), taskId, trailerId, dockId, repository ) );

        app.MapPost( "api/tasks/loading/startLoad", static async (
                [FromQuery] Guid palletId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await StartLoadingPallet( 
                http.GetLoadingEmployee(), palletId, repository ) );

        app.MapPost( "api/tasks/loading/finishLoad", static async (
                [FromQuery] Guid trailerId,
                [FromQuery] Guid palletId,
                HttpContext http,
                ILoadingRepository repository ) =>
            await FinishLoadingPallet( 
                http.GetLoadingEmployee(), trailerId, palletId, repository ) );

        app.MapPost( "api/tasks/loading/finishTask", static async (
                HttpContext http,
                ILoadingRepository repository ) =>
            await FinishLoadingTask(
                http.GetLoadingEmployee(), repository ) );
    }

    static IResult RefreshTask( LoadingEmployee employee )
    {
        var refreshed = employee.LoadingTask is not null
            && employee.LoadingTask.IsStarted
            && !employee.LoadingTask.IsFinished;

        return refreshed
            ? Results.Ok( LoadingTaskSummary.FromModel( employee.LoadingTask! ) )
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
    static async Task<IResult> StartLoadingTask( LoadingEmployee employee, Guid taskId, Guid trailerId, Guid dockId, ILoadingRepository repository )
    {
        var loading = await repository
            .GetLoadingOperationsWithTasks();

        var taskStarted = loading is not null
            && employee.StartLoading( loading, taskId, trailerId, dockId );

        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( LoadingTaskSummary.FromModel( employee.LoadingTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> StartLoadingPallet( LoadingEmployee employee, Guid palletId, ILoadingRepository repository )
    {
        var loadingStarted = employee
            .StartLoadingPallet( palletId );

        return loadingStarted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishLoadingPallet( LoadingEmployee employee, Guid trailerId, Guid palletId, ILoadingRepository repository )
    {
        var receivingCompleted = employee
            .FinishLoadingPallet( trailerId, palletId );

        return receivingCompleted && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
    static async Task<IResult> FinishLoadingTask( LoadingEmployee employee, ILoadingRepository repository )
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