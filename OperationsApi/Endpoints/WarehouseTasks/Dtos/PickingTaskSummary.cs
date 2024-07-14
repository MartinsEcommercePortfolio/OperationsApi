using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PickingTaskSummary(
)
{
    internal static PickingTaskSummary FromModel( PickingTask model ) =>
        new();
}