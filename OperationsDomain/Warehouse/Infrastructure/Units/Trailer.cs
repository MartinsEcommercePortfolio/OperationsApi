namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Trailer : PalletHolder
{
    internal Trailer( string number, Dock dock, List<Pallet> pallets ) : base( 12 )
    {
        Id = Guid.NewGuid();
        Number = number;
        Dock = dock;
        Pallets = pallets;
    }
    
    public string Number { get; private set; }
    public Dock Dock { get; private set; }
}