using OperationsDomain.Shipping.Models;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Equipment;
using OperationsDomain.Warehouse.Infrastructure;
using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations.Loading.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;
using OperationsDomain.Warehouse.Operations.Putaways.Models;
using OperationsDomain.Warehouse.Operations.Receiving.Models;

namespace OperationsDomain.Warehouse;

public sealed class Root
{
    public Guid Id { get; private set; }
    
    public List<Employee> Employees { get; private set; } = [];

    public List<Forklift> Forklifts { get; private set; } = [];
    public List<Scanner> Scanners { get; private set; } = [];
    
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];

    public ReceivingOperations ReceivingOperations { get; set; } = default!;
    public PutawayOperations PutawayOperations { get; set; } = default!;
    public PickingOperations ReplenishingOperations { get; set; } = default!;
    public PickingOperations PickingOperations { get; set; } = default!;
    public LoadingOperations LoadingOperations { get; set; } = default!;
}