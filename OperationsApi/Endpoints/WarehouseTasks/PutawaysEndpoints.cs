using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Putaways;
using OperationsDomain.Domain.WarehouseSections.Putaways.Types;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/tasks/putaways/refreshTask",
            static ( HttpContext http ) =>
            RefreshTask( http.Employee() ) );
        
        app.MapPost( "api/tasks/putaways/startTask",
            static async ( [FromQuery] Guid palletId, HttpContext http, IPutawayRepository repository ) =>
            await StartPutawayTask( http.Employee(), palletId, repository ) );

        app.MapPost( "api/tasks/putaways/completeTask",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPutawayRepository repository ) =>
            await CompletePutaway( http.Employee(), palletId, rackingId, repository ) );
    }

    static IResult RefreshTask( Employee employee )
    {
        var task = employee.GetTask<PutawayTask>();

        return task.IsStarted && !task.IsCompleted
            ? Results.Ok( PutawayTaskSummary.FromModel( task ) )
            : Results.Problem();
    }
    static async Task<IResult> StartPutawayTask( Employee employee, Guid palletId, IPutawayRepository repository )
    {
        var putaways = await repository
            .GetPutawaysSectionWithPalletsAndRackings();

        if (putaways is null)
            return Results.NotFound();

        var putawayTask = await putaways
            .BeginPutaway( employee, palletId )
            .ConfigureAwait( false );
        
        return putawayTask is not null && putawayTask.IsStarted && await repository.SaveAsync()
            ? Results.Ok( PutawayTaskSummary.FromModel( putawayTask ) )
            : Results.Problem();
    }
    static async Task<IResult> CompletePutaway( Employee employee, Guid palletId, Guid rackingId, IPutawayRepository repository )
    {
        var putaways = await repository.GetPutawaysSectionWithTasks();

        var success = putaways is not null
            && putaways.CompletePutaway( employee, palletId, rackingId )
            && await repository.SaveAsync();
        
        return success
            ? Results.Ok( true )
            : Results.Problem();
    }
}