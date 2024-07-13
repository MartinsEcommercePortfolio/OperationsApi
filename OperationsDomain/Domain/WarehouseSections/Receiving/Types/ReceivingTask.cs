using OperationsDomain.Domain.WarehouseBuilding;

namespace OperationsDomain.Domain.WarehouseSections.Receiving.Types;

public sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; set; } = default!;
    public Dock Dock { get; set; } = default!;
    public Area Area { get; set; } = default!;
    public Pallet? CurrentPallet { get; set; }
    
    public Guid TrailerId { get; set; }
    public Guid DockId { get; set; }
    public Guid AreaId { get; set; }
    
    public Pallet? ReceivePallet( Guid palletId )
    {
        Pallet? pallet = Trailer.CheckPallet( palletId );

        bool unloaded = pallet is not null
            && CurrentPallet is null
            && Trailer.UnloadPallet( pallet )
            && pallet.ReceiveBy( Employee );

        if (!unloaded)
            return null;
        
        CurrentPallet = pallet;
        return pallet;
    }
    public bool StagePallet( Guid palletId, Guid areaId )
    {
        bool staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && areaId == Area.Id
            && Area.StagePallet( CurrentPallet )
            && CurrentPallet.PutIn( Area );

        if (!staged)
            return false;
        
        if (IsFinished())
            Employee.FinishTask();

        CurrentPallet = null;
        return staged;
    }
    public bool IsFinished() =>
        CurrentPallet is null && Trailer.IsEmpty();
}