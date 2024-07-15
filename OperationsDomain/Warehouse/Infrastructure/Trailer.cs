namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Trailer
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public Dock? Dock { get; set; }
    public List<Pallet> Pallets { get; set; } = [];
    
    public Pallet? GetPallet( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
    public bool UnloadPallet( Pallet pallet ) => 
        Pallets.Remove( pallet );
    public bool LoadPallet( Pallet pallet )
    {
        if (Pallets.Contains( pallet ))
            return false;
        Pallets.Add( pallet );
        return true;
    }
    public bool IsEmpty() =>
        Pallets.Count <= 0;
}