using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Putaways.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Employees.Models;
using OperationsDomain.Operations.Putaways;

namespace OperationsApi.Endpoints.Putaways;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/putaways/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.GetPutawayEmployee() ) );
        
        app.MapPost( "api/tasks/putaways/startTask",
            static async ( [FromQuery] Guid palletId, HttpContext http, IPutawayRepository repository ) =>
            await StartPutawayTask( http.GetPutawayEmployee(), palletId, repository ) );

        app.MapPost( "api/tasks/putaways/finishTask",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPutawayRepository repository ) =>
            await FinishPutaway( http.GetPutawayEmployee(), palletId, rackingId, repository ) );
    }

    static IResult RefreshTask( PutawayEmployee employee )
    {
        var refreshed = employee.PutawayTask is not null
            && employee.PutawayTask.IsStarted
            && !employee.PutawayTask.IsFinished;

        return refreshed
            ? Results.Ok( PutawayTaskSummary.FromModel( employee.PutawayTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPutawayTask( PutawayEmployee employee, Guid palletId, IPutawayRepository repository )
    {
        var putaways = await repository
            .GetPutawaysOperationsWithPalletsAndRackings();

        if (putaways is null)
            return Results.NotFound();

        var taskStarted = await employee
            .StartPutaway( putaways, palletId )
            .ConfigureAwait( false );
        
        return taskStarted && await repository.SaveAsync()
            ? Results.Ok( PutawayTaskSummary.FromModel( employee.PutawayTask! ) )
            : Results.Problem();
    }
    static async Task<IResult> FinishPutaway( PutawayEmployee employee, Guid palletId, Guid rackingId, IPutawayRepository repository )
    {
        var putaways = await repository.GetPutawaysOperationsWithTasks();

        var success = putaways is not null
            && employee.FinishPutaway( putaways, palletId, rackingId );
        
        return success && await repository.SaveAsync()
            ? Results.Ok( true )
            : Results.Problem();
    }
}