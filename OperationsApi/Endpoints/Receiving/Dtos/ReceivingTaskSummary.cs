using OperationsDomain.Operations.Receiving.Models;

namespace OperationsApi.Endpoints.Receiving.Dtos;

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