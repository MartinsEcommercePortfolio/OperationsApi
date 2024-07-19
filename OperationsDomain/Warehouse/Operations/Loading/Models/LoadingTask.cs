using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Loading.Models;

public sealed class LoadingTask : WarehouseTask
{
    LoadingTask(
        Guid id, Employee? employee, bool isStarted, bool isFinished, Trailer trailer, Dock dock, List<Pallet> pallets )
        : base( id, employee, isStarted, isFinished )
    {
        Trailer = trailer;
        Dock = dock;
        Pallets = pallets;
    }

    public static LoadingTask New( Trailer trailer, Dock dock, List<Pallet> pallets ) =>
        new( Guid.NewGuid(), null, false, false, trailer, dock, pallets );
    
    public Trailer Trailer { get; private set; }
    public Dock Dock { get; private set; }
    public List<Pallet> Pallets { get; private set; }

    internal override bool StartWith( Employee employee )
    {
        return base.StartWith( employee )
            && Trailer.Pallets.All( p => p.AssignTo( employee ) );
    }
    internal override bool CleanUp( Employee employee )
    {
        return base.CleanUp( employee )
            && Trailer.Pallets.All( p => p.UnAssignFrom( employee ) );
    }
    internal bool VerifyLoadingTask( Guid trailerId, Guid dockId )
    {
        return Trailer.Id == trailerId
            && Dock.Id == dockId;
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
}