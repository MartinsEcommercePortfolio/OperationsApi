using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations.Picking.Models;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct PickingTaskSummary(
    string DockNumber,
    string AreaNumber,
    List<PickSummary> PickSummaries )
{
    internal static PickingTaskSummary FromModel( PickingTask model ) => new(
        model.StagingDock.Number,
        model.StagingArea.Number,
        GetPickSummaries( model.Pallets ) );

    static List<PickSummary> GetPickSummaries( List<Pallet> pallets )
    {
        List<PickSummary> summaries = [];
        summaries.AddRange( from m in pallets select PickSummary.FromModel( m ) );
        return summaries;
    }
}