using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct ReceivingTaskSummary(
    string TrailerNumber,
    string DockNumber,
    string AreaNumber,
    int PalletCount )
{
    public static ReceivingTaskSummary FromModel( ReceivingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Area.Number,
        model.Trailer.Pallets.Count );
}