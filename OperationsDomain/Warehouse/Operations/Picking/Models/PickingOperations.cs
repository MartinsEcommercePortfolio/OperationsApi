using OperationsDomain.Warehouse.Infrastructure;

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

    public PickingTask? GeneratePickingTask( Guid orderId, List<(Guid, int)> productCounts )
    {
        var dock = FindDockForTask();
        if (dock is null)
            return null;

        var area = FindAreaForDock( dock );
        if (area is null)
            return null;
        
        List<PickingLine> lines = [];
        foreach ( var pc in productCounts )
        {
            var racking = FindRackingForPickLine( pc.Item1 );

            if (racking is null)
                return null;

            lines.Add( new PickingLine( racking, pc.Item2 ) );
        }

        return new PickingTask( orderId, dock, area, lines );
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
    Racking? FindRackingForPickLine( Guid productId ) =>
        Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Product.Id == productId );
}