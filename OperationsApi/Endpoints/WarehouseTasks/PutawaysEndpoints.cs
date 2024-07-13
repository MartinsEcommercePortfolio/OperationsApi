using Microsoft.AspNetCore.Mvc;
using OperationsApi.Features._Shared;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Putaways;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/putaways/start",
            static async ( [FromBody] Guid palletId, HttpContext http, IPutawayRepository putaways ) =>
            await StartPutawayTask( http.Employee(), palletId, putaways ) );
    }

    static async Task<IResult> StartPutawayTask( Employee employee, Guid palletId, IPutawayRepository putaways )
    {
        Racking? putawayRacking = await putaways.StartPutawayTask( employee, palletId );
        return putawayRacking is not null
            ? Results.Ok( RackingDto.FromModel( putawayRacking ) )
            : Results.Problem();
    }
}