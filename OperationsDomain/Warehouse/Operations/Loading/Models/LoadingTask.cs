using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingTask : WarehouseTask
{
    public Trailer Trailer { get; private set; } = null!;
    public Dock Dock { get; private set; } = null!;
    public List<Area> Areas { get; private set; } = null!;
    public List<Pallet> Pallets { get; private set; } = null!;
    public Guid TrailerId { get; private set; }
    public Guid DockId { get; private set; }
    public Guid AreaId { get; private set; }

    internal bool InitializeLoadingTask( Guid trailerId, Guid dockId, Guid areaId )
    {
        var isValid = Trailer.Id == trailerId
            && Dock.Id == dockId
            && Areas.Any( a => a.Id == areaId );
        
        return isValid;
    }

    internal bool StartLoadingPallet( Guid areaId, Guid palletId )
    {
        var area = Areas.FirstOrDefault( a => a.Id == areaId );
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );

        return area is not null
            && pallet is not null
            && area.RemovePallet( pallet )
            && pallet.GiveTo( Employee );
    }
    internal bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );

        var loaded = pallet is not null
            && Trailer.Id == trailerId
            && Pallets.Remove( pallet )
            && Trailer.LoadPallet( pallet )
            && pallet.Load( Trailer );

        if (!loaded)
            return false;

        if (Pallets.Count <= 0)
            IsFinished = true;

        return true;
    }
}