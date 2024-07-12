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
}