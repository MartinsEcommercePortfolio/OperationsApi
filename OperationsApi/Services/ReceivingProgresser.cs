using OperationsApi.Services.Dtos;
using OperationsApi.Utilities;
using OperationsDomain.Operations.Inbound;
using OperationsDomain.Operations.Receiving;
using OperationsDomain.Operations.Receiving.Models;

namespace OperationsApi.Services;

internal sealed class ReceivingProgresser(
    IServiceProvider provider,
    ILogger<OrderingProgresser> logger )
    : BackgroundService
{
    readonly IServiceProvider _provider = provider;
    readonly ILogger<OrderingProgresser> _logger = logger;

    static readonly TimeSpan ExecutionInterval = TimeSpan.FromMinutes( 3 );

    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        _logger.LogInformation( "ReceivingProgresser has started." );

        stoppingToken.Register( () =>
            _logger.LogInformation( "ReceivingProgresser is stopping." ) );

        while ( !stoppingToken.IsCancellationRequested )
        {
            _logger.LogInformation( "ReceivingProgresser is executing." );

            try
            {
                await using AsyncServiceScope scope = _provider.CreateAsyncScope();

                HttpMessenger messenger = GetHttpMessenger( scope );
                
                var inboundRepository = GetOrderingRepository( scope );
                var receivingRepository = GetReceivingRepository( scope );

                await HandleTrailersAwaitingDock( messenger, inboundRepository );
                await HandleTrailersAwaitingTask( inboundRepository, receivingRepository );
                await HandleCompletedReceivements( messenger, inboundRepository, receivingRepository );
            }
            catch ( Exception e )
            {
                _logger.LogError( e, "ReceivingProgresser threw an exception during execution." );
            }

            await Task.Delay( ExecutionInterval, stoppingToken );
        }

        _logger.LogInformation( "ReceivingProgresser has stopped." );
    }

    async Task HandleTrailersAwaitingDock( HttpMessenger messenger, IInboundRepository inboundRepository )
    {
        var inbound = await inboundRepository.GetInboundOperations();
        
        if (inbound is null)
        {
            _logger.LogError( "ReceivingProgresser HandleTrailersAwaitingDock() failed to generate models from repositories during execution." );
            return;
        }

        var trailers = inbound.GetTrailersAwaitingDock();

        foreach ( var trailer in trailers )
        {
            await using var transaction = await inboundRepository.Context.Database.BeginTransactionAsync();
            
            if (!inbound.ReserveDock( trailer, out var dock ))
                break;

            var body = new TrailerDockDto( trailer.Number, dock!.Number );
            if (!await messenger.TryPut<bool>( Consts.AssignDockToSimulation, body ))
            {
                _logger.LogWarning( "ReceivingProgresser HandleTrailersAwaitingDock() http failed to notify trailer-dock assign during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            if (!await inboundRepository.SaveAsync())
            {
                await transaction.RollbackAsync();
                throw new Exception( "ReceivingProgresser HandleTrailersAwaitingDock() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "ReceivingProgresser HandleTrailersAwaitingDock() successfully handled waiting trailer." );
        }
    }
    async Task HandleTrailersAwaitingTask( IInboundRepository inboundRepository, IReceivingRepository receivingRepository )
    {
        var inbound = await inboundRepository.GetInboundOperations();
        var receiving = await receivingRepository.GetReceivingOperationsWithTasks();

        if (inbound is null || receiving is null)
        {
            _logger.LogError( "ReceivingProgresser HandleTrailersAwaitingTask() failed to generate models from repositories during execution." );
            return;
        }

        var trailers = inbound.GetTrailersAwaitingTask();

        foreach ( var trailer in trailers )
        {
            await using var transaction = await inboundRepository.Context.Database.BeginTransactionAsync();

            var handled = receiving.AddNewTask( trailer )
                && inbound.ConfirmTaskAssigned( trailer );
            
            if (!handled)
            {
                _logger.LogWarning( "ReceivingProgresser HandleTrailersAwaitingTask() failed to generate new receiving task during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            if (!await inboundRepository.SaveAsync())
            {
                await transaction.RollbackAsync();
                throw new Exception( "ReceivingProgresser HandleTrailersAwaitingTask() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "ReceivingProgresser HandleTrailersAwaitingTask() successfully handled waiting trailer." );
        }
    }
    async Task HandleCompletedReceivements( HttpMessenger messenger, IInboundRepository inboundRepository, IReceivingRepository receivingRepository )
    {
        var inbound = await inboundRepository.GetInboundOperations();
        var receiving = await receivingRepository.GetReceivingOperationsWithTasks();

        if (inbound is null || receiving is null)
        {
            _logger.LogError( "ReceivingProgresser HandleCompletedReceivements() failed to generate models from repositories during execution." );
            return;
        }

        var completedReceivingTasks = receiving.GetCompletedTasks();
        var notified = new List<ReceivingTask>();
        
        foreach ( var task in completedReceivingTasks )
        {
            var trailerNotified = await messenger.TryPut<bool>( Consts.NotifySimulationUndockTrailer, task.Trailer.Number );
            if (!trailerNotified)
            {
                _logger.LogError( "ReceivingProgresser HandleCompletedReceivements() failed to notify trailer to leave during execution." );
                continue;
            }

            notified.Add( task );
        }

        foreach ( var task in notified )
        {
            await using var transaction = await inboundRepository.Context.Database.BeginTransactionAsync();
            
            if (!receiving.RemoveTask( task ))
            {
                _logger.LogWarning( "ReceivingProgresser HandleCompletedReceivements() failed to remove receiving task during execution." );
                await transaction.RollbackAsync();
                continue;
            }

            if (!await receivingRepository.SaveAsync())
            {
                await transaction.RollbackAsync();
                throw new Exception( "ReceivingProgresser HandleCompletedReceivements() failed to save changes." );
            }

            await transaction.CommitAsync();
            _logger.LogInformation( "ReceivingProgresser HandleTrailersAwaitingTask() successfully handled waiting trailer." );
        }
    }
    
    static HttpMessenger GetHttpMessenger( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<HttpMessenger>() ?? throw new Exception( $"Failed to get {nameof( HttpMessenger )} from provider." );
    static IInboundRepository GetOrderingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IInboundRepository>() ?? throw new Exception( $"Failed to get {nameof( IInboundRepository )} from provider." );
    static IReceivingRepository GetReceivingRepository( AsyncServiceScope scope ) =>
        scope.ServiceProvider.GetService<IReceivingRepository>() ?? throw new Exception( $"Failed to get {nameof( IReceivingRepository )} from provider." );
}