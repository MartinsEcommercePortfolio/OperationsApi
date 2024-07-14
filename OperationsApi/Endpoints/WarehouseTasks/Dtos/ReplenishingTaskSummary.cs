using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct ReplenishingTaskSummary(
    Guid PalletId,
    bool HasBeenPicked, 
    RackingDto FromRacking,
    RackingDto ToRacking )
{
    internal static ReplenishingTaskSummary FromModel( ReplenishingTask model ) => new(
        model.Pallet.Id,
        model.PalletHasBeenPicked,
        RackingDto.FromModel( model.FromRacking ),
        RackingDto.FromModel( model.ToRacking ) );
}