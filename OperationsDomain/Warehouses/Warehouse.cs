using OperationsDomain.Warehouses.Employees;
using OperationsDomain.Warehouses.Infrastructure;
using OperationsDomain.Warehouses.Operations.Putaways.Models;
using OperationsDomain.Warehouses.Operations.Receiving.Models;

namespace OperationsDomain.Warehouses;

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

    public ReceivingOperations ReceivingOperations { get; set; } = default!;
    public PutawayOperations PutawayOperations { get; set; } = default!;

    public Area? GetAreaById( Guid areaId ) =>
        Areas.FirstOrDefault( a => a.Id == areaId );
    public Racking? GetRackingById( Guid rackingId ) =>
        Rackings.FirstOrDefault( r => r.Id == rackingId );
    public Pallet? GetPalletById( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
}