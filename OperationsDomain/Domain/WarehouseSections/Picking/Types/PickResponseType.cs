namespace OperationsDomain.Domain.WarehouseSections.Picking.Types;

public enum PickResponseType
{
    FailedToStart,
    GoToNextLocation,
    InvalidLocation,
    PickItem,
    ItemPicked,
    InvalidPick,
    Complete
}