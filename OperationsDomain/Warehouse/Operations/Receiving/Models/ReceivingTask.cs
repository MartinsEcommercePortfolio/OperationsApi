using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingTask : WarehouseTask
{
    internal ReceivingTask() {}
    internal ReceivingTask( Trailer trailer, Dock dock, Area area )
    {
        Trailer = trailer;
        Dock = dock;
        Area = area;
    }
    
    public Trailer Trailer { get; private set; } = null!;
    public Dock Dock { get; private set; } = null!;
    public Area Area { get; private set; } = null!;
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> StagedPallets { get; private set; } = [];

    internal override bool CleanUp( Employee employee )
    {
        return Dock.UnAssignFrom( employee )
            && Trailer.Pallets.All( p => p.UnAssignFrom( employee ) );
    }
    internal bool InitializeReceiving( Employee employee, Guid trailerId, Guid dockId, Guid areaId )
    {
        return trailerId == Trailer.Id
            && dockId == Dock.Id
            && areaId == Area.Id
            && Dock.AssignTo( Employee )
            && Trailer.AssignTo( Employee )
            && Trailer.Pallets.All( p => p.AssignTo( employee ) );
    }
    internal Pallet? StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        CurrentPallet = Trailer.GetPallet( palletId );

        bool received = Trailer.Id == trailerId
            && CurrentPallet is not null
            && CurrentPallet is null
            && !StagedPallets.Contains( CurrentPallet! );

        return received
            ? CurrentPallet
            : null;
    }
    internal bool FinishReceivingPallet( Guid areaId, Guid palletId )
    {
        var staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && areaId == Area.Id
            && !StagedPallets.Contains( CurrentPallet );
        
        if (!staged)
            return false;

        StagedPallets.Add( CurrentPallet! );
        CurrentPallet = null;
        
        return staged;
    }
}