using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PickingLineSummary(
    Guid ProductId,
    int Quantity,
    string Aisle,
    string Bay,
    string Level )
{
    internal static PickingLineSummary FromModel( PickingLine model ) => new(
        model.Id,
        model.Quantity,
        model.Racking.Aisle,
        model.Racking.Bay,
        model.Racking.Level );
}