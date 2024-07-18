using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingTask : WarehouseTask
{
    public LoadingTask( Trailer trailer, Dock dock, List<Pallet> pallets )
    {
        Trailer = trailer;
        Dock = dock;
        Pallets = pallets;
    }
    
    public Trailer Trailer { get; private set; }
    public Dock Dock { get; private set; }
    public List<Pallet> Pallets { get; private set; }

    internal bool InitializeLoadingTask( Employee employee, Guid trailerId, Guid dockId )
    {
        return Trailer.Id == trailerId
            && Dock.Id == dockId
            && Trailer.Pallets.All( p => p.AssignTo( employee ) );
    }
    internal Pallet? GetLoadingPallet( Guid palletId )
    {
        return Pallets.FirstOrDefault( p => p.Id == palletId );
    }
    internal bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = Pallets.FirstOrDefault( p => p.Id == palletId );

        var loaded = pallet is not null
            && Trailer.Id == trailerId
            && Pallets.Remove( pallet );

        if (!loaded)
            return false;

        if (Pallets.Count <= 0)
            IsFinished = true;

        return true;
    }
    internal override bool Finish( Employee employee )
    {
        return Trailer.Pallets.All( p => p.UnAssignFrom( employee ) );
    }

}