using OperationsDomain.Operations.Shipping.Models;
using OperationsDomain.Units;

namespace OperationsDomain.Operations.Intake.Models;

public sealed class IntakeOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];
    public List<Pallet> Pallets { get; private set; } = [];
    public List<IntakeTask> PendingIntakeTasks { get; private set; } = [];
    public List<IntakeTask> ActiveIntakeTasks { get; private set; } = [];

    public IntakeTask? GetNextTask() =>
        PendingIntakeTasks.FirstOrDefault();

    public bool GenerateTask( Guid trailerId, Guid dockId, Guid areaId, List<Pallet> pallets )
    {
        var trailer = Trailers.FirstOrDefault( t => t.Id == trailerId );
        var dock = Docks.FirstOrDefault( d => d.Id == dockId );
        var area = Areas.FirstOrDefault( a => a.Id == areaId );

        var validTask = trailer is not null
            && dock is not null
            && area is not null
            && dock.Owner is null
            && dock.Trailer is null
            && area.Owner is null;

        if (!validTask)
            return false;
        
        Trailers.Add( trailer! );

        var task = IntakeTask.New( trailer!, dock!, area! );
        PendingIntakeTasks.Add( task );

        return true;
    }
    public IntakeTask? GetTask( Guid taskId ) =>
        PendingIntakeTasks.FirstOrDefault( t => t.Id == taskId );
    public bool ActivateTask( IntakeTask intakeTask )
    {
        var accepted = intakeTask.IsStarted 
            && !intakeTask.IsFinished
            && !ActiveIntakeTasks.Contains( intakeTask ) 
            && PendingIntakeTasks.Remove( intakeTask );

        if (accepted)
            ActiveIntakeTasks.Add( intakeTask );

        return accepted;
    }
    public bool CompleteTask( IntakeTask intakeTask )
    {
        return intakeTask.IsFinished
            && ActiveIntakeTasks.Remove( intakeTask );
    }
}