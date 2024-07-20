using OperationsDomain.Employees.Models;
using OperationsDomain.Operations.Shipping.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Operations.Intake.Models;

public sealed class IntakeTask : WarehouseTask
{
    IntakeTask(
        Guid id, Employee? employee, bool isStarted, bool isFinished, Trailer trailer, Dock dock, Area area )
        : base( id, employee, isStarted, isFinished )
    {
        Trailer = trailer;
        Dock = dock;
        Area = area;
    }
    
    public static IntakeTask New( Trailer trailer, Dock dock, Area area ) => 
        new( Guid.NewGuid(), null, false, false, trailer, dock, area );
    
    public Trailer Trailer { get; private set; }
    public Dock Dock { get; private set; }
    public Area Area { get; private set; }
    public Pallet? CurrentPallet { get; private set; }
    public List<Pallet> StagedPallets { get; private set; } = [];
    
    internal override bool StartWith( Employee employee )
    {
        return base.StartWith( employee )
            && Dock.AssignTo( employee )
            && Trailer.AssignTo( employee )
            && Trailer.Pallets.All( p => p.AssignTo( employee ) );
    }
    internal override bool CleanUp( Employee employee )
    {
        return base.CleanUp( employee )
            && Dock.UnAssignFrom( employee )
            && Trailer.Pallets.All( p => p.UnAssignFrom( employee ) );
    }
    internal bool VerifyStart( Guid trailerId, Guid dockId, Guid areaId )
    {
        return Employee is not null
            && IsStarted
            && !IsFinished
            && trailerId == Trailer.Id
            && dockId == Dock.Id
            && areaId == Area.Id;
    }
    internal Pallet? StartUnloadingPallet( Guid trailerId, Guid palletId )
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
    internal bool FinishUnloadingPallet( Guid areaId, Guid palletId )
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