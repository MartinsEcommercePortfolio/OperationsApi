using OperationsDomain.Ordering;
using OperationsDomain.Ordering.Types;
using OperationsDomain.Warehouse.Operations.Picking;

namespace OperationsApi.Services;

internal sealed class PickGenerator( IOrderingRepository orderingRepository, IPickingRepository pickingRepository )
{
    readonly IOrderingRepository _orderingRepository = orderingRepository;
    readonly IPickingRepository _pickingRepository = pickingRepository;

    async Task GeneratePickTasks()
    {
        var ordering = await _orderingRepository.GetOrderingOperationsWithOrderGroups();
        var picking = await _pickingRepository.GetPickingOperationsWithTasks();

        if (ordering is null || picking is null)
            return;

        List<WarehouseOrderGroup> orderGroups = ordering.GenerateOrderGroups();

        foreach ( WarehouseOrderGroup group in orderGroups )
            if (!picking.GeneratePickingTasks( group ))
                return;

        await _pickingRepository.SaveAsync();
    }
}