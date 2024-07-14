using OperationsDomain.Domain.WarehouseSections.Picking.Types;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct PickingLineSummary(
    Guid ProductId,
    int Quantity, 
    RackingDto Racking )
{
    internal static PickingLineSummary FromModel( PickingLine model ) => new(
        model.Id,
        model.Quantity,
        RackingDto.FromModel( model.Racking ) );
}