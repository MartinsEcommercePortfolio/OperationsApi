using OperationsDomain.Warehouse.Infrastructure.Units;
using OperationsDomain.Warehouse.Operations.Loading.Models;

namespace OperationsDomain.Warehouse.Employees.Models;

public sealed class LoadingEmployee : Employee
{
    public LoadingEmployee( Guid id, string name, Pallet? palletEquipped, LoadingTask? task ) 
        : base( id, name, palletEquipped, task ) { }
    public LoadingTask? LoadingTask => 
        TaskAs<LoadingTask>();

    public bool StartLoading( LoadingOperations loading, Guid taskId, Guid trailerId, Guid dockId )
    {
        if (LoadingTask is not null)
            return false;
        
        var task = loading.GetTask( taskId );
        
        return task is not null
            && StartTask( task )
            && task.VerifyLoadingTask( trailerId, dockId )
            && loading.ActivateTask( task );
    }
    public bool StartLoadingPallet( Guid palletId )
    {
        var pallet = LoadingTask?.GetLoadingPallet( palletId );
        return pallet?.Area != null
            && UnStagePallet( pallet.Area, pallet );
    }
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        return LoadingTask is not null
            && PalletEquipped is not null
            && LoadingTask.FinishLoadingPallet( trailerId, palletId )
            && LoadPallet( LoadingTask.Trailer, PalletEquipped );
    }
    public bool FinishLoading( LoadingOperations loading )
    {
        return LoadingTask is not null
            && EndTask()
            && loading.CompleteTask( LoadingTask );
    }
}