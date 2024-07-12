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
    
    public bool ReceivePallet( Employee employee, Pallet pallet )
    {
        bool received = CurrentPallet is null 
            && Trailer.UnloadPallet( pallet );
        
        if (received)
            pallet.AssignTo( employee );
        
        return received;
    }
    public bool StagePallet( Employee employee, Pallet pallet, Area area )
    {
        bool staged = CurrentPallet == pallet
            && Area == area
            && Area.StagePallet( pallet );

        if (staged)
            pallet.UnassignFrom( employee );
        
        return staged;
    }
}