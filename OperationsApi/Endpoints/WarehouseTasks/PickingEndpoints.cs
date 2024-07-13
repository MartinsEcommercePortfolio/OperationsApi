using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseSections.Picking;

namespace OperationsApi.Endpoints.WarehouseTasks;

internal static class PickingEndpoints
{
    internal static void MapPickingEndpoints( this IEndpointRouteBuilder app )
    {
        
    }

    static async Task<IResult> StartPickingTask( Employee employee, Guid palletId, IPickingRepository putaways )
    {
        var putawayRacking = await putaways.BeginPickingTask( employee, palletId );
        return putawayRacking is not null
            ? Results.Ok( true )
            : Results.Problem();
    }
}