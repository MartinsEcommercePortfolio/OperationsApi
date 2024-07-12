namespace OperationsApi.Domain.Warehouses;

internal sealed class Warehouse
{
    public Guid Id { get; set; }
    public List<Employee> Employees { get; set; } = [];
    public List<Dock> Docks { get; set; } = [];
    public List<Racking> Racks { get; set; } = [];
    public List<Area> Areas { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<Item> Items { get; set; } = [];

    public async Task<Racking?> AssignPutaway( Employee employee, Pallet pallet )
    {
        if (pallet.IsOwned())
            return null;
        
        // Because there may be a lot of rackings to check
        return await Task.Run( () => {
            Racking? racking = Racks.FirstOrDefault(
                r => r.CanHoldPallet( pallet ) );

            if (racking is null)
                return null;

            racking.AssignTo( employee );
            pallet.AssignTo( employee );
            return racking;
        } );
    }
}