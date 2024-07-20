using OperationsDomain;
using OperationsDomain.Outbound.Picking.Models;
using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Warehouse.Dtos;

internal readonly record struct PickingTaskSummary(
    int DockNumber,
    int AreaNumber,
    List<PickSummary> PickSummaries )
{
    internal static PickingTaskSummary FromModel( PickingTask model ) => new(
        model.StagingDock.Number,
        model.StagingArea.Number,
        GetPickSummaries( model.Pallets ) );

    static List<PickSummary> GetPickSummaries( List<Pallet> pallets )
    {
        List<PickSummary> summaries = [];
        summaries.AddRange( from m in pallets select PickSummary.FromModel( m, m.Racking ) );
        return summaries;
    }
}