using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingTask : WarehouseTask
{
    public Trailer TrailerToLoad { get; private set; } = null!;
    public Dock DockToUse { get; private set; } = null!;
    public List<Area> AreasToPickFrom { get; private set; } = null!;
    public List<Pallet> PalletsToLoad { get; private set; } = null!;

    internal bool InitializeLoadingTask( Guid trailerId, Guid dockId, Guid areaId )
    {
        var isValid = TrailerToLoad.Id == trailerId
            && DockToUse.Id == dockId
            && AreasToPickFrom.Any( a => a.Id == areaId );
        
        return isValid;
    }
    internal bool StartLoadingPallet( Guid areaId, Guid palletId )
    {
        var area = AreasToPickFrom.FirstOrDefault( a => a.Id == areaId );
        var pallet = PalletsToLoad.FirstOrDefault( p => p.Id == palletId );

        return area is not null
            && pallet is not null
            && area.RemovePallet( pallet )
            && Employee.TakePallet( pallet )
            && pallet.AssignTo( Employee );
    }
    internal bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = PalletsToLoad.FirstOrDefault( p => p.Id == palletId );

        var loaded = pallet is not null
            && TrailerToLoad.Id == trailerId
            && PalletsToLoad.Remove( pallet )
            && TrailerToLoad.AddPallet( pallet )
            && pallet.Load( TrailerToLoad );

        if (!loaded)
            return false;

        if (PalletsToLoad.Count <= 0)
            IsFinished = true;

        return true;
    }
}