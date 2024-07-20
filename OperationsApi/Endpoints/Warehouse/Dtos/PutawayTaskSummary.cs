using OperationsDomain.Inbound.Putaways.Models;

namespace OperationsApi.Endpoints.Warehouse.Dtos;

internal readonly record struct PutawayTaskSummary(
    Guid PalletId,
    RackingDto Racking )
{
    internal static PutawayTaskSummary FromModel( PutawayTask model ) =>
        new( model.PalletId, RackingDto.FromModel( model.PutawayRacking ) );
}