using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Putaways.Types;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;

namespace OperationsDomain.Domain;

public sealed class Warehouse
{
    public Guid Id { get; set; }
    public List<Employee> Employees { get; set; } = [];
    public List<Trailer> Trailers { get; set; } = [];
    public List<Dock> Docks { get; set; } = [];
    public List<Area> Areas { get; set; } = [];
    public List<Racking> Rackings { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<Item> Items { get; set; } = [];

    public ReceivingSection ReceivingSection { get; set; } = default!;
    public PutawaySection PutawaySection { get; set; } = default!;

    public Area? GetAreaById( Guid areaId ) =>
        Areas.FirstOrDefault( a => a.Id == areaId );
    public Racking? GetRackingById( Guid rackingId ) =>
        Rackings.FirstOrDefault( r => r.Id == rackingId );
    public Pallet? GetPalletById( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
}