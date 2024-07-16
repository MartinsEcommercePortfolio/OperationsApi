using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Receiving.Models;

public sealed class ReceivingTask : WarehouseTask
{
    public Trailer Trailer { get; private set; } = default!;
    public Dock Dock { get; private set; } = default!;
    public Area Area { get; private set; } = default!;
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> StagedPallets { get; private set; } = [];

    internal bool InitializeReceiving( Guid trailerId, Guid dockId, Guid areaId )
    {
        var validArea = trailerId == Trailer.Id
            && dockId == Dock.Id
            && areaId == Area.Id
            && Trailer.AssignTo( Employee )
            && Dock.AssignTo( Employee )
            && Area.AssignTo( Employee );

        return validArea;
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