using OperationsDomain.Employees.Models;
using OperationsDomain.Infrastructure.Units;

namespace OperationsDomain.Operations.Putaways.Models;

public sealed class PutawayTask : WarehouseTask
{
    PutawayTask(
        Guid id, Employee? employee, bool isStarted, bool isFinished, Pallet pallet, Area area, Racking racking )
        : base( id, employee, isStarted, isFinished )
    {
        Pallet = pallet;
        PickupArea = area;
        PutawayRacking = racking;
        PalletId = pallet.Id;
        PickupAreaId = area.Id;
        PutawayRackingId = racking.Id;
    }

    public static PutawayTask New( Pallet pallet, Area area, Racking racking ) =>
        new( Guid.NewGuid(), null, false, false, pallet, area, racking );

    public Pallet Pallet { get; private set; }
    public Area PickupArea { get; private set; }
    public Racking PutawayRacking { get; private set; }
    public Guid PalletId { get; private set; }
    public Guid PickupAreaId { get; private set; }
    public Guid PutawayRackingId { get; private set; }

    internal override bool CleanUp( Employee employee )
    {
        return PutawayRacking.UnAssignFrom( employee )
            && Pallet.UnAssignFrom( employee );
    }
    internal bool Initialize( Employee employee )
    {
        return PutawayRacking.AssignTo( employee )
            && Pallet.AssignTo( employee );
    }
    internal bool ConfirmPutaway( Guid rackingId, Guid palletId )
    {
        IsFinished = rackingId == PutawayRacking.Id
            && palletId == Pallet.Id;

        return IsFinished;
    }
}