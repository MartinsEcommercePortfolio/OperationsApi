using OperationsDomain.Operations.Picking.Models;
using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Picking.Dtos;

internal readonly record struct PickingTaskSummary(
    int DockNumber,
    int AreaNumber,
    List<PickingSummary> PickSummaries )
{
    internal static PickingTaskSummary FromModel( PickingTask model ) => new(
        model.StagingDock.Number,
        model.StagingArea.Number,
        GetPickSummaries( model.Pallets ) );

    static List<PickingSummary> GetPickSummaries( List<Pallet> pallets )
    {
        List<PickingSummary> summaries = [];
        summaries.AddRange( from m in pallets select PickingSummary.FromModel( m, m.Racking ) );
        return summaries;
    }
}