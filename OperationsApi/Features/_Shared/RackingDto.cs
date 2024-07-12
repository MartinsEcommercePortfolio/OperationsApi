using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Features._Shared;

internal readonly record struct RackingDto(
    Guid Id,
    string Aisle,
    string Bay,
    string Level )
{
    public static RackingDto FromModel( Racking model ) =>
        new( model.Id, model.Aisle, model.Bay, model.Level );
}