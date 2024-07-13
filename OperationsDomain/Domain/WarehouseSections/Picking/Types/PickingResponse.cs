namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public readonly record struct PickingResponse(
    PickResponseType ResponseType,
    Guid? RackingId,
    Guid? ProductId )
{
    public static PickingResponse FailedToStart() =>
        new( PickResponseType.FailedToStart, null, null );
    public static PickingResponse ItemNotFound() =>
        new( PickResponseType.InvalidPick, null, null );
    public static PickingResponse ItemPicked() =>
        new( PickResponseType.ItemPicked, null, null );
}