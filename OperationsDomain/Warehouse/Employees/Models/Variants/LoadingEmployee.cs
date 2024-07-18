using OperationsDomain.Warehouse.Operations.Loading.Models;

namespace OperationsDomain.Warehouse.Employees.Models.Variants;

public sealed class LoadingEmployee : Employee
{
    public LoadingTask? LoadingTask => TaskAs<LoadingTask>();

    public bool StartLoading( LoadingOperations loading, Guid taskId, Guid trailerId, Guid dockId, Guid areaId )
    {
        if (LoadingTask is not null)
            return false;
        
        var task = loading.GetTask( taskId );
        
        return task is not null
            && task.InitializeLoadingTask( trailerId, dockId, areaId )
            && StartTask( task )
            && loading.AcceptTask( task );
    }
    public bool StartLoadingPallet( Guid areaId, Guid palletId )
    {
        var pallet = LoadingTask?.GetLoadingPallet( areaId, palletId );
        return pallet?.Area != null
            && UnStagePallet( pallet.Area, pallet );
    }
    public bool FinishLoadingPallet( Guid trailerId, Guid palletId )
    {
        return LoadingTask is not null
            && Pallet is not null
            && LoadingTask.FinishLoadingPallet( trailerId, palletId )
            && LoadPallet( LoadingTask.TrailerToLoad, Pallet );
    }
    public bool FinishLoading( LoadingOperations loading )
    {
        return LoadingTask is not null
            && EndTask()
            && loading.CompleteTask( LoadingTask );
    }
}