using OperationsDomain.Inbound.Intake.Models;

namespace OperationsApi.Endpoints.Warehouse.Dtos;

internal readonly record struct ReceivingTaskSummary(
    int TrailerNumber,
    int DockNumber,
    int AreaNumber,
    int PalletCount )
{
    public static ReceivingTaskSummary FromModel( ReceivingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Area.Number,
        model.Trailer.Pallets.Count );
}