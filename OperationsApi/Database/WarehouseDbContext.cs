using Microsoft.EntityFrameworkCore;
using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Equipment;
using OperationsApi.Domain.Shipping;
using OperationsApi.Domain.Warehouses;
using OperationsApi.Domain.Warehouses.Loading;
using OperationsApi.Domain.Warehouses.Picking;
using OperationsApi.Domain.Warehouses.Putaways;
using OperationsApi.Domain.Warehouses.Receiving;
using OperationsApi.Domain.Warehouses.Replenishing;

namespace OperationsApi.Database;

internal class WarehouseDbContext( DbContextOptions<WarehouseDbContext> options ) 
    : DbContext( options )
{
    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        
    }

    // SHIPPING
    public required DbSet<Delivery> Deliveries { get; init; }
    public required DbSet<Load> Loads { get; init; }
    public required DbSet<Shipment> Shipments { get; init; }
    public required DbSet<Trailer> Trailers { get; init; }
    
    // TASKS
    public required DbSet<LoadingTask> PendingLoadingTasks { get; init; }
    public required DbSet<PickingTask> PendingPickingTasks { get; init; }
    public required DbSet<PutawayTask> PendingPutawayTasks { get; init; }
    public required DbSet<ReceivingTask> PendingReceivingTasks { get; init; }
    public required DbSet<ReplenishingTask> PendingReplenTasks { get; init; }
    
    public required DbSet<LoadingTask> ActiveLoadingTasks { get; init; }
    public required DbSet<PickingTask> ActivePickingTasks { get; init; }
    public required DbSet<PutawayTask> ActivePutawayTasks { get; init; }
    public required DbSet<ReceivingTask> ActiveReceivingTasks { get; init; }
    public required DbSet<ReplenishingTask> ActiveReplenTasks { get; init; }
    
    // WAREHOUSE
    public required DbSet<Area> Areas { get; init; }
    public required DbSet<Dock> Docks { get; init; }
    public required DbSet<Employee> Employees { get; init; }
    public required DbSet<Forklift> Forklifts { get; init; }
    public required DbSet<Item> Items { get; init; }
    public required DbSet<Pallet> Pallets { get; init; }
    public required DbSet<Racking> Rackings { get; init; }
    public required DbSet<Scanner> Scanners { get; init; }
    public required DbSet<Warehouse> Warehouses { get; init; }
}