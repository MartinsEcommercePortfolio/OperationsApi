using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct PickSummary(
    PalletDto Pallet,
    RackingDto Racking )
{
    internal static PickSummary FromModel( Pallet pallet ) => new(
        PalletDto.FromModel( pallet ),
        RackingDto.FromModel( pallet.Racking ) );
}