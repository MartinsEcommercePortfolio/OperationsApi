using Microsoft.AspNetCore.Mvc;
using OperationsApi.Endpoints.WarehouseTasks.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Putaways;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/putaways/start",
            static async ( [FromQuery] Guid palletId, HttpContext http, IPutawayRepository repository ) =>
            await StartPutawayTask( http.Employee(), palletId, repository ) );

        app.MapPost( "api/tasks/putaways/complete",
            static async ( [FromQuery] Guid palletId, [FromQuery] Guid rackingId, HttpContext http, IPutawayRepository repository ) =>
            await CompletePutawayTask( http.Employee(), palletId, rackingId, repository ) );
    }

    static async Task<IResult> StartPutawayTask( Employee employee, Guid palletId, IPutawayRepository repository )
    {
        var putaways = await repository.GetPutawaysSectionWithPalletsAndRackings();

        if (putaways is null)
            return Results.NotFound();

        var putawayRacking = await putaways
            .BeginPutaway( employee, palletId )
            .ConfigureAwait( false );
        
        return putawayRacking is not null && await repository.SaveAsync()
            ? Results.Ok( RackingDto.FromModel( putawayRacking ) )
            : Results.Problem();
    }
    static async Task<IResult> CompletePutawayTask( Employee employee, Guid palletId, Guid rackingId, IPutawayRepository repository )
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