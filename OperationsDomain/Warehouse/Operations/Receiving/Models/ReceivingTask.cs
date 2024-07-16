using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; private set; } = default!;
    public Dock Dock { get; private set; } = default!;
    public Area Area { get; private set; } = default!;
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> StagedPallets { get; private set; } = [];

    public bool InitializeReceiving( Guid trailerId, Guid dockId, Guid areaId )
    {
        var validArea = trailerId == Trailer.Id
            && dockId == Dock.Id
            && areaId == Area.Id
            && Trailer.AssignTo( Employee )
            && Dock.AssignTo( Employee )
            && Area.AssignTo( Employee );

        return validArea;
    }
    public bool StartReceivingPallet( Guid trailerId, Guid palletId )
    {
        var pallet = Trailer.GetPallet( palletId );
        
        CurrentPallet = pallet;
        
        return Trailer.Id == trailerId
            && pallet is not null
            && CurrentPallet is null
            && !StagedPallets.Contains( pallet )
            && Employee.UnloadPallet( Trailer, pallet );
    }
    public bool FinishReceivingPallet( Guid areaId, Guid palletId )
    {
        var staged = CurrentPallet is not null
            && palletId == CurrentPallet.Id
            && areaId == Area.Id
            && !StagedPallets.Contains( CurrentPallet )
            && Employee.StagePallet( Area, CurrentPallet );
        
        if (!staged)
            return false;

        StagedPallets.Add( CurrentPallet! );
        CurrentPallet = null;
        
        return staged;
    }
}