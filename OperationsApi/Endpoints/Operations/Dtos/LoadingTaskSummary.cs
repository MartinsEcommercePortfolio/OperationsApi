using OperationsDomain.Operations.Loading.Models;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct LoadingTaskSummary(
    string Trailer,
    string Dock,
    List<Guid> Pallets )
{
    internal static LoadingTaskSummary FromModel( LoadingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Pallets.Select( static p => p.Id ).ToList() );
}