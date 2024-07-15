using OperationsDomain.Warehouses.Operations.Receiving.Models;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

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