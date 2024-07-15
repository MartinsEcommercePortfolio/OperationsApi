using OperationsDomain.Domain.Catalog;
using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Picking.Models;

public readonly record struct PickingResponse(
    int? PicksRemaining,
    Product? Product,
    Racking? Racking,
    Area? Area )
{
    public static PickingResponse ReadyToStage( Area area ) =>
        new( null, null, null, area );
    public static PickingResponse Started( Racking racking ) =>
        new( null, null, racking, null );
}