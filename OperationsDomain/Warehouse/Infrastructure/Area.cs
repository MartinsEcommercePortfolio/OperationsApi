namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Area
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public List<Pallet> Pallets { get; set; } = [];

    public bool TakePallet( Pallet pallet )
    {
        bool staged = IsAvailable
            && !Pallets.Contains( pallet );

        if (staged)
            Pallets.Add( pallet );

        return staged;
    }
    public bool RemovePallet( Pallet pallet )
    {
        bool removed = Pallets.Remove( pallet );
        return removed;
    }

    public bool CanUse()
    {
        return IsAvailable;
    }
}