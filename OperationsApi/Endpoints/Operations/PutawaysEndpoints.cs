using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.Operations.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Putaways;
using OperationsDomain.Warehouse.Operations.Putaways.Models;

namespace OperationsApi.Endpoints.Operations;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/putaways/refreshPutawayTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapPost( "api/tasks/putaways/startPutawayTask",
            static async ( [FromQuery] Guid palletId, HttpContext http, IPutawayRepository repository ) =>
            await StartPutawayTask( http.Employee(), palletId, repository ) );

        app.MapPost( "api/tasks/putaways/finishPutawayTask",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPutawayRepository repository ) =>
            await FinishPutaway( http.Employee(), palletId, rackingId, repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee
            .TaskAs<PutawayTask>();

        return task.IsStarted && !task.IsFinished
            ? Results.Ok( PutawayTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPutawayTask( Employee employee, Guid palletId, IPutawayRepository repository )
    {
        var putaways = await repository
            .GetPutawaysOperationsWithPalletsAndRackings();

        if (putaways is null)
            return Results.NotFound();

        var putawayTask = await putaways
            .StartPutaway( employee, palletId )
            .ConfigureAwait( false );
        
        return putawayTask is not null && putawayTask.IsStarted && await repository.SaveAsync()
            ? Results.Ok( PutawayTaskSummary.FromModel( putawayTask ) )
            : Results.Problem();
    }
    static async Task<IResult> FinishPutaway( Employee employee, Guid palletId, Guid rackingId, IPutawayRepository repository )
    {
        var putaways = await repository.GetPutawaysOperationsWithTasks();

        var success = putaways is not null
            && putaways.FinishPutaway( employee, palletId, rackingId )
            && await repository.SaveAsync();
        
        return success
            ? Results.Ok( true )
            : Results.Problem();
    }
}