using Microsoft.EntityFrameworkCore;
using OperationsDomain.Domain.Equipment;
using OperationsDomain.Warehouses;
using OperationsDomain.Warehouses.Employees;
using OperationsDomain.Warehouses.Infrastructure;
using OperationsDomain.Warehouses.Operations.Loading.Models;
using OperationsDomain.Warehouses.Operations.Picking.Models;
using OperationsDomain.Warehouses.Operations.Putaways.Models;
using OperationsDomain.Warehouses.Operations.Receiving.Models;
using OperationsDomain.Warehouses.Operations.Replenishing.Models;

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
    public required DbSet<ReceivingOperations> Receiving { get; set; }
    public required DbSet<PutawayOperations> Putaways { get; set; }
    public required DbSet<PickingOperations> Picking { get; set; }
    public required DbSet<ReplenishingOperations> Replenishing { get; set; }
    public required DbSet<LoadingOperations> Loading { get; set; }
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