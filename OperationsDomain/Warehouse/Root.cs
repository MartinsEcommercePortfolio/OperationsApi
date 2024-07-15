using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Operations.Putaways.Models;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse;

public sealed class Root
{
    public Guid Id { get; private set; }
    public List<Employee> Employees { get; private set; } = [];
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<Item> Items { get; private set; } = [];

    public ReceivingOperations ReceivingOperations { get; set; } = default!;
    public PutawayOperations PutawayOperations { get; set; } = default!;

    public Area? GetAreaById( Guid areaId ) =>
        Areas.FirstOrDefault( a => a.Id == areaId );
    public Racking? GetRackingById( Guid rackingId ) =>
        Rackings.FirstOrDefault( r => r.Id == rackingId );
    public Pallet? GetPalletById( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
}