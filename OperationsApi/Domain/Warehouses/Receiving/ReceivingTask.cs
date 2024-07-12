using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Shipping;

namespace OperationsApi.Domain.Warehouses.Receiving;

internal sealed class ReceivingTask : WarehouseTask
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
            && Trailer.UnloadPallet( pallet );

        if (!unloaded)
            return null;
        
        pallet?.Receive( Employee );
        CurrentPallet = pallet;
        return pallet;
    }
    public bool StagePallet( Guid palletId, Guid areaId )
    {
        bool staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && areaId == Area.Id
            && Area.StagePallet( CurrentPallet );

        if (!staged)
            return false;
        
        CurrentPallet?.Stage( Area );
        Employee.FinishTask();
        return true;
    }
}