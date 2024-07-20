using OperationsDomain.Outbound.Loading.Models;

namespace OperationsApi.Endpoints.Loading.Dtos;

internal readonly record struct LoadingTaskSummary(
    int Trailer,
    int Dock,
    List<Guid> Pallets )
{
    internal static LoadingTaskSummary FromModel( LoadingTask model ) => new(
        model.Trailer.Number,
        model.Dock.Number,
        model.Pallets.Select( static p => p.Id ).ToList() );
}