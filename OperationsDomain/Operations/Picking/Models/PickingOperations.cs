using OperationsDomain.Units;

namespace OperationsDomain.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<PickingTask> Tasks { get; private set; } = [];
    public List<Guid> PendingTasks { get; private set; } = [];
    public List<Guid> ActiveTasks { get; private set; } = [];
    public List<Guid> CompletedTasks { get; private set; } = [];

    public List<PickingTask> GetCompletedTasks() =>
        Tasks.Where( t => CompletedTasks.Contains( t.Id ) ).ToList();
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
        Tasks.Add( task );
        PendingTasks.Add( task.Id );
        return task;
    }
    public PickingTask? GetNextPickingTask() =>
        Tasks.FirstOrDefault( t => PendingTasks.Contains( t.Id ) );
    public bool RemoveCompletedTask( PickingTask task )
    {
        return CompletedTasks.Remove( task.Id )
            && Tasks.Remove( task );
    }
    
    internal PickingTask? GetTask( Guid taskId ) =>
        Tasks.FirstOrDefault( t => t.Id == taskId );
    internal bool ActivateTask( PickingTask task )
    {
        var accepted = task.IsStarted
            && !task.IsFinished
            && !ActiveTasks.Contains( task.Id )
            && PendingTasks.Remove( task.Id );
        
        if (accepted)
            ActiveTasks.Add( task.Id );

        return accepted;
    }
    internal bool HandleCompletedTask( PickingTask task )
    {
        var completed = task.IsFinished
            && ActiveTasks.Remove( task.Id );

        if (completed)
            CompletedTasks.Add( task.Id );

        return completed;
    }

    Area? FindAreaForDock( Dock dock )
    {
        Area? area = null;
        return area;
    }
}