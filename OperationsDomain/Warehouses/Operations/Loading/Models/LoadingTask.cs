using OperationsDomain.Warehouses.Infrastructure;

namespace OperationsDomain.Warehouses.Operations.Loading.Models;

public sealed class LoadingTask : WarehouseTask
{
    public Trailer Trailer { get; set; } = null!;
    public Dock Dock { get; set; } = null!;
    public List<Area> Areas { get; set; } = null!;
    public List<Pallet> Pallets { get; set; } = null!;
    public Guid TrailerId { get; set; }
    public Guid DockId { get; set; }
    public Guid AreaId { get; set; }

    public bool InitializeLoadingTask( Guid trailerId, Guid dockId, Guid areaId )
    {
        var isValid = Trailer.Id == trailerId
            && Dock.Id == dockId
            && Areas.Any( a => a.Id == areaId );
        
        return isValid;
    }
    
    public bool StartLoadingPallet( Guid areaId, Guid palletId )
    {
        var area = Areas.FirstOrDefault( a => a.Id == areaId );
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );

        return area is not null
            && pallet is not null
            && area.RemovePallet( pallet )
            && pallet.GiveTo( Employee );
    }
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );

        var loaded = pallet is not null
            && Trailer.Id == trailerId
            && Pallets.Remove( pallet )
            && Trailer.LoadPallet( pallet )
            && pallet.LoadInTrailer( Trailer );

        if (!loaded)
            return false;

        if (Pallets.Count <= 0)
            IsCompleted = true;

        return true;
    }
}