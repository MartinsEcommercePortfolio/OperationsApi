using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Shipping;
using OperationsApi.Domain.Warehouses.Receiving;

namespace OperationsApi.Domain.Warehouses;

internal sealed class Warehouse
{
    public Guid Id { get; set; }
    public List<Employee> Employees { get; set; } = [];
    public List<Trailer> Trailers { get; set; } = [];
    public List<Dock> Docks { get; set; } = [];
    public List<Area> Areas { get; set; } = [];
    public List<Racking> Racks { get; set; } = [];
    public List<Pallet> Pallets { get; set; } = [];
    public List<Item> Items { get; set; } = [];

    public List<ReceivingTask> PendingReceivingTasks { get; set; } = [];
    public List<ReceivingTask> ActiveReceivingTasks { get; set; } = [];

    public Area? GetAreaById( Guid areaId ) =>
        Areas.FirstOrDefault( a => a.Id == areaId );
    public Pallet? GetPalletById( Guid palletId ) =>
        Pallets.FirstOrDefault( p => p.Id == palletId );
}