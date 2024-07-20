using OperationsDomain.Inbound.Intake.Models;

namespace OperationsApi.Endpoints.Intake.Dtos;

internal readonly record struct IntakeTaskSummary(
    int TrailerNumber,
    int DockNumber,
    int AreaNumber,
    int PalletCount )
{
    public static IntakeTaskSummary FromModel( ReceivingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Area.Number,
        model.Trailer.Pallets.Count );
}