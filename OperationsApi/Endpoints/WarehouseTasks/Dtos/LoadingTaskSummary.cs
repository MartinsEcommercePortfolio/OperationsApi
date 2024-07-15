using OperationsDomain.Warehouses.Operations.Loading.Models;

namespace OperationsApi.Endpoints.WarehouseTasks.Dtos;

internal readonly record struct LoadingTaskSummary(
    string Trailer,
    string Dock,
    List<string> Areas,
    List<Guid> Pallets )
{
    internal static LoadingTaskSummary FromModel( LoadingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Areas.Select( static a => a.Number ).ToList(),
        model.Pallets.Select( static p => p.Id ).ToList() );
}