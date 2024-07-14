using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PickingTaskSummary(
    string DockNumber,
    string AreaNumber,
    List<PickingLineSummary> PickSummaries )
{
    internal static PickingTaskSummary FromModel( PickingTask model ) => new(
        model.StagingDock.Number,
        model.StagingArea.Number,
        GetLineSummaries( model.PickLines ) );

    static List<PickingLineSummary> GetLineSummaries( List<PickingLine> models )
    {
        List<PickingLineSummary> summaries = [];
        summaries.AddRange( 
            from m in models select PickingLineSummary.FromModel( m ) );
        return summaries;
    }
}