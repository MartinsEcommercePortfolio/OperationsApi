using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PickingTaskResumed(
    PickingTaskSummary TaskSummary,
    PickingResponse PickingResponse )
{
    internal static PickingTaskResumed FromModel( (PickingTask, PickingResponse) model ) =>
        new( PickingTaskSummary.FromModel( model.Item1 ), model.Item2 );
}