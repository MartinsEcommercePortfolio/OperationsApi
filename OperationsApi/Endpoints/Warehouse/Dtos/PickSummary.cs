using OperationsDomain;
using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Warehouse.Dtos;

internal readonly record struct PickSummary(
    PalletDto Pallet,
    RackingDto Racking )
{
    internal static PickSummary FromModel( Pallet pallet, Racking racking ) => new(
        PalletDto.FromModel( pallet ),
        RackingDto.FromModel( racking ) );
}