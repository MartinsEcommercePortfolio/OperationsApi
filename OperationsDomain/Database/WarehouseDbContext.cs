using Microsoft.EntityFrameworkCore;
using OperationsDomain.Domain;
using OperationsDomain.Domain.Employees;
using OperationsDomain.Domain.Equipment;
using OperationsDomain.Domain.WarehouseBuilding;
using OperationsDomain.Domain.WarehouseSections.Loading;
using OperationsDomain.Domain.WarehouseSections.Picking.Types;
using OperationsDomain.Domain.WarehouseSections.Putaways.Types;
using OperationsDomain.Domain.WarehouseSections.Receiving.Types;
using OperationsDomain.Domain.WarehouseSections.Replenishing.Types;

namespace OperationsDomain.Database;

public class WarehouseDbContext( DbContextOptions<WarehouseDbContext> options ) 
    : DbContext( options )
{
    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        
    }
    
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
    public required DbSet<Warehouse> Warehouses { get; init; }
    public required DbSet<ReceivingSection> Receiving { get; set; }
    public required DbSet<PutawaySection> Putaways { get; set; }
    public required DbSet<PickingSection> Picking { get; set; }
    public required DbSet<ReplenishingSection> Replenishing { get; set; }
    public required DbSet<Trailer> Trailers { get; init; }
    public required DbSet<Dock> Docks { get; init; }
    public required DbSet<Area> Areas { get; init; }
    public required DbSet<Racking> Rackings { get; init; }
    public required DbSet<Pallet> Pallets { get; init; }
    public required DbSet<Item> Items { get; init; }
    public required DbSet<Employee> Employees { get; init; }
    public required DbSet<Forklift> Forklifts { get; init; }
    public required DbSet<Scanner> Scanners { get; init; }
}