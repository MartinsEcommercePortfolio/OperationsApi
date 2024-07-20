using OperationsDomain.Employees.Models;
using OperationsDomain.Equipment;
using OperationsDomain.Inbound.Intake.Models;
using OperationsDomain.Inbound.Putaways.Models;
using OperationsDomain.Outbound.Loading.Models;
using OperationsDomain.Outbound.Picking.Models;
using OperationsDomain.Outbound.Shipping.Models;
using OperationsDomain.Units;

namespace OperationsDomain;

public sealed class Warehouse
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