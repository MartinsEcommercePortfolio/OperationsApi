using OperationsDomain.Domain.WarehouseSections.Putaways.Models;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PutawayTaskSummary(
    Guid PalletId,
    RackingDto Racking )
{
    internal static PutawayTaskSummary FromModel( PutawayTask model ) =>
        new( model.PalletId, RackingDto.FromModel( model.PutawayRacking ) );
}