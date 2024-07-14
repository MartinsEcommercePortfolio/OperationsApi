namespace OperationsDomain.Domain.WarehouseBuilding;

public sealed class Trailer
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public List<Pallet> Pallets { get; set; } = [];
    
    public Pallet? GetPallet( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
    public bool UnloadPallet( Pallet pallet ) => 
        Pallets.Remove( pallet );
    public bool IsEmpty() =>
        Pallets.Count <= 0;
}