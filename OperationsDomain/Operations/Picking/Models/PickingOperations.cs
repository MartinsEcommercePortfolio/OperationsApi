using OperationsDomain.Units;

namespace OperationsDomain.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];
    public List<PickingTask> CompletedPickingTasks { get; private set; } = [];

    public PickingTask? GenerateNewPickingTask( Guid warehouseOrderId, Dock dock, List<Guid> productIds )
    {
        var area = FindAreaForDock( dock );
        if (area is null)
            return null;

        var pallets = new List<Pallet>();
        
        foreach ( var id in productIds )
        {
            var racking = Rackings
                .FirstOrDefault( r =>
                    r.Product.Id == id &&
                    r.Pallet is not null &&
                    pallets.Contains( r.Pallet! ) );
            
            if (racking?.Pallet is null)
                return null;
            
            pallets.Add( racking.Pallet );
        }

        var task = PickingTask.New( warehouseOrderId, dock, area, pallets );
        PendingPickingTasks.Add( task );
        return task;
    }
    public PickingTask? GetNextPickingTask() => 
        PendingPickingTasks.FirstOrDefault();
    public bool RemoveCompletedTask( PickingTask task )
    {
        return CompletedPickingTasks.Remove( task );
    }
    
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

    Area? FindAreaForDock( Dock dock )
    {
        Area? area = null;
        return area;
    }
}