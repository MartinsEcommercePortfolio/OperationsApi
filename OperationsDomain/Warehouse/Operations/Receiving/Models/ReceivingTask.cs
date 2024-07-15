using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; private set; } = default!;
    public Dock Dock { get; private set; } = default!;
    public Area? Area { get; private set; }
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> StagedPallets { get; private set; } = [];
    
    public Guid TrailerId { get; set; }
    public Guid DockId { get; set; }
    public Guid AreaId { get; set; }

    internal bool InitializeStagingArea( Guid trailerId, Guid dockId, Area area )
    {
        var validArea = trailerId == Trailer.Id
            && dockId == Dock.Id
            && area.CanUse();

        if (validArea)
            Area = area;

        return validArea;
    }
    internal bool StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = Trailer.GetPallet( palletId );

        var unloaded = Trailer.Id == trailerId
            && pallet is not null
            && CurrentPallet is null
            && !StagedPallets.Contains( pallet )
            && Trailer.UnloadPallet( pallet )
            && pallet.GiveTo( Employee );

        if (unloaded)
            CurrentPallet = pallet;
        
        return unloaded;
    }
    internal bool FinishReceivingPallet( Guid areaId, Guid palletId )
    {
        var staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && Area is not null
            && areaId == Area.Id
            && Area.TakePallet( CurrentPallet )
            && CurrentPallet.Stage( Employee, Area );
        
        if (!staged)
            return false;

        StagedPallets.Add( CurrentPallet! );
        CurrentPallet = null;
        return staged;
    }
}