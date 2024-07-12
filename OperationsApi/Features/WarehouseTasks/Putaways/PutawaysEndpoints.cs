using Microsoft.AspNetCore.Mvc;
using OperationsApi.Domain.Employees;
using OperationsApi.Utilities;

namespace OperationsApi.Features.WarehouseTasks.Putaways;

internal static class PutawaysEndpoints
{
    internal static void MapPutawaysEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/tasks/putaways/start",
            static async ( [FromBody] Guid palletId, HttpContext http, PutawayRepository putaways ) =>
            await StartPutaway( palletId, http.Employee(), putaways ) );
    }

    static async Task<IResult> StartPutaway( Guid palletId, Employee employee, PutawayRepository putaways )
    {
        var result = await putaways.AssignPutaway( palletId, employee );
        return result is not null
            ? Results.Ok( result )
            : Results.Problem();
    }
}