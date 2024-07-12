namespace OperationsApi.Domain.Warehouses;

internal sealed class Area
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public List<Pallet> Pallets { get; set; } = [];

    public bool StagePallet( Pallet pallet )
    {
        bool staged = IsAvailable 
            && !Pallets.Contains( pallet ) 
            && pallet.Stage( this );
        return staged;
    }
}