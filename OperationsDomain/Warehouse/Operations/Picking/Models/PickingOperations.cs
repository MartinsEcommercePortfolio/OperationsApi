using OperationsDomain.Ordering.Types;
using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Picking.Models;

public sealed class PickingOperations
{
    public Guid Id { get; private set; }
    public List<Racking> Rackings { get; private set; } = [];
    public List<PickingTask> PendingPickingTasks { get; private set; } = [];
    public List<PickingTask> ActivePickingTasks { get; private set; } = [];
    public List<PickingTask> CompletedPickingTasks { get; private set; } = [];

    public bool GeneratePickingTasks( WarehouseOrderGroup orderGroup )
    {
        foreach ( var order in orderGroup.Orders )
        {
            List<PickingLine> lines = [];
            foreach ( var i in order.Items )
            {
                var racking = FindRackingForPickLine( i.ProductId );

                if (racking is null)
                    return false;
                
                lines.Add( new PickingLine( racking, i.Quantity ) );
            }
            
            var task = new PickingTask( 
                order.OrderId, orderGroup.Dock, orderGroup.Area, lines );
            
            PendingPickingTasks.Add( task );
        }

        return true;
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

    Racking? FindRackingForPickLine( Guid productId ) =>
        Rackings.FirstOrDefault( r =>
            r.IsPickSlot &&
            r.Product.Id == productId );
}