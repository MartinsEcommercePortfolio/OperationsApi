using OperationsDomain.Warehouse.Operations.Loading.Models;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct LoadingTaskSummary(
    string Trailer,
    string Dock,
    List<string> Areas,
    List<Guid> Pallets )
{
    internal static LoadingTaskSummary FromModel( LoadingTask model ) => new(
        model.TrailerToLoad.Number,
        model.DockToUse.Number,
        model.AreasToPickFrom.Select( static a => a.Number ).ToList(),
        model.PalletsToLoad.Select( static p => p.Id ).ToList() );
}