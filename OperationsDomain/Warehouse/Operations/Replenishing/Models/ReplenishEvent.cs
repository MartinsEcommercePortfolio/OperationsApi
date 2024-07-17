using OperationsDomain.Warehouse.Infrastructure;

namespace OperationsDomain.Warehouse.Operations.Replenishing.Models;

public sealed class ReplenishEvent
{
    public ReplenishEvent( Racking racking )
    {
        Racking = racking;
    }

    public Racking Racking { get; private set; }
}