using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; set; } = default!;
    public Dock Dock { get; set; } = default!;
    public Area? Area { get; set; }
    public Pallet? CurrentPallet { get; set; }
    public List<Pallet> UnloadedPallets { get; set; } = [];
    
    public Guid TrailerId { get; set; }
    public Guid DockId { get; set; }
    public Guid AreaId { get; set; }

    public bool InitializeStagingArea( Guid trailerId, Guid dockId, Area area )
    {
        bool validArea = trailerId == Trailer.Id
            && dockId == Dock.Id
            && area.CanUse();

        if (validArea)
            Area = area;

        return validArea;
    }
    public bool StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        Pallet? pallet = Trailer.GetPallet( palletId );

        bool unloaded = Trailer.Id == trailerId
            && pallet is not null
            && CurrentPallet is null
            && !UnloadedPallets.Contains( pallet )
            && Trailer.UnloadPallet( pallet )
            && pallet.GiveTo( Employee );

        if (!unloaded)
            return false;
        
        UnloadedPallets.Add( pallet! );
        CurrentPallet = pallet;
        return unloaded;
    }
    public bool FinishReceivingPallet( Guid palletId, Guid areaId )
    {
        bool staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && Area is not null
            && areaId == Area.Id
            && Area.TakePallet( CurrentPallet )
            && CurrentPallet.PlaceInArea( Area );

        if (staged)
            CurrentPallet = null;
        
        return staged;
    }
    public bool IsFinished() =>
        CurrentPallet is null && Trailer.IsEmpty();
}