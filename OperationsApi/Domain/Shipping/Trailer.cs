using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Domain.Shipping;

internal sealed class Trailer
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public List<Pallet> Pallets { get; set; } = [];

    public Pallet? CheckPallet( Guid palletId )
    {
        return Pallets.FirstOrDefault();
    }
    public bool UnloadPallet( Pallet pallet )
    {
        return Pallets.Remove( pallet );
    }
}