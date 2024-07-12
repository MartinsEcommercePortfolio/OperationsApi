using OperationsApi.Domain.Warehouses.Receiving;

namespace OperationsApi.Features.WarehouseTasks.Receiving.Dtos;

internal readonly record struct ReceivingTaskSummary(
    string TrailerNumber,
    string DockNumber,
    string AreaNumber,
    string ShipmentId,
    int PalletCount )
{
    public static ReceivingTaskSummary FromModel( ReceivingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Area.Number,
        model.Shipment.Id.ToString(),
        model.Pallets.Count );
}