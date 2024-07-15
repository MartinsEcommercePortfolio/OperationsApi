using OperationsDomain.Warehouses.Infrastructure;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct RackingDto(
    Guid Id,
    string Aisle,
    string Bay,
    string Level )
{
    public static RackingDto FromModel( Racking model ) =>
        new( model.Id, model.Aisle, model.Bay, model.Level );
}