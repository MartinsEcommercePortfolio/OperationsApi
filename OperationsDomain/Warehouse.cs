using OperationsDomain.Catalog;
using OperationsDomain.Employees.Models;
using OperationsDomain.Equipment;
using OperationsDomain.Units;

namespace OperationsDomain;

public abstract class Warehouse
{
    public Guid Id { get; private set; }
    
    public List<Employee> Employees { get; private set; } = [];

    public List<Forklift> Forklifts { get; private set; } = [];
    public List<Scanner> Scanners { get; private set; } = [];
    
    public List<Product> Products { get; private set; } = [];
    
    public List<Trailer> ReceivingTrailers { get; private set; } = [];
    public List<Trailer> ShippingTrailers { get; private set; } = [];
    public List<Dock> ReceivingDocks { get; private set; } = [];
    public List<Dock> ShippingDocks { get; private set; } = [];
    public List<Area> ReceivingAreas { get; private set; } = [];
    public List<Area> ShippingAreas { get; private set; } = [];
    public List<Racking> Rackings { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
}