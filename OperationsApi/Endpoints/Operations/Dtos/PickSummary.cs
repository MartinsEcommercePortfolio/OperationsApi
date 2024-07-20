using OperationsDomain.Infrastructure.Units;

namespace OperationsApi.Endpoints.Operations.Dtos;

internal readonly record struct PickSummary(
    PalletDto Pallet,
    RackingDto Racking )
{
    internal static PickSummary FromModel( Pallet pallet, Racking racking ) => new(
        PalletDto.FromModel( pallet ),
        RackingDto.FromModel( racking ) );
}