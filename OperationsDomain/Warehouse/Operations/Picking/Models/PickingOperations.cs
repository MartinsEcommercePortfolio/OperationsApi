using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];
    public List<PickingTask> CompletedPickingTasks { get; private set; } = [];

    public PickingTask? GeneratePickingTask( Guid orderId, Dictionary<Guid, int> palletCounts )
    {
        var dock = FindDockForTask();
        if (dock is null)
            return null;

        var area = FindAreaForDock( dock );
        if (area is null)
            return null;

        var pallets = FindPalletsToPick( palletCounts );
        return pallets is not null
            ? new PickingTask( orderId, dock, area, pallets )
            : null;
    }
    public bool AddPendingTask( PickingTask task )
    {
        var added = !task.IsStarted
            && !task.IsFinished
            && !PendingPickingTasks.Contains( task )
            && !ActivePickingTasks.Contains( task )
            && !CompletedPickingTasks.Contains( task );

        if (added)
            PendingPickingTasks.Add( task );

        return added;
    }
    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    
    internal PickingTask? GetPendingTask( Guid taskId ) =>
        PendingPickingTasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( PickingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActivePickingTasks.Contains( task )
            && PendingPickingTasks.Remove( task );
        
        if (accepted)
            ActivePickingTasks.Add( task );

        return accepted;
    }
    internal bool HandleCompletedTask( PickingTask task )
    {
        var completed = task.IsFinished
            && ActivePickingTasks.Remove( task );

        if (completed)
            CompletedPickingTasks.Add( task );

        return completed;
    }

    Dock? FindDockForTask() =>
        null;
    Area? FindAreaForDock( Dock dock ) =>
        null;
    List<Pallet>? FindPalletsToPick( Dictionary<Guid, int> palletCounts )
    {
        List<Pallet> pallets = [];

        foreach ( var count in palletCounts )
        {
            for ( int i = 0; i < count.Value; i++ )
            {
                var racking = Rackings
                    .FirstOrDefault( r =>
                        r.Product.Id == count.Key &&
                        r.Pallet is not null );

                if (racking is null)
                    return null;

                pallets.Add( racking.Pallet! );
            }

        }
        
        return pallets;
    }
}