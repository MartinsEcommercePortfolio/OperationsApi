using Microsoft.AspNetCore.Mvc;
using OperationsApi.Features._Shared;
using OperationsApi.Utilities;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Putaways;

namespace OperationsApi.Features.WarehouseTasks;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/putaways/start",
            static async ( [FromBody] Guid palletId, HttpContext http, IPutawayRepository putaways ) =>
            await StartPutaway( http.Employee(), palletId, putaways ) );
    }

    static async Task<IResult> StartPutaway( Employee employee, Guid palletId, IPutawayRepository putaways )
    {
        Racking? putawayRacking = await putaways.BeginPutaway( employee, palletId );
        return putawayRacking is not null
            ? Results.Ok( RackingDto.FromModel( putawayRacking ) )
            : Results.Problem();
    }
}