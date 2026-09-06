using BizDataRouter.Status;

namespace BizDataRouter.Services;

/// <summary>Contrôle une seule exécution du pipeline à la fois.</summary>
public sealed class PipelineCoordinator : IHostedService
{
    private readonly PipelineService _pipeline;
    private readonly PipelineStatus _status;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _pipelineCancellation;
    private Task? _pipelineTask;

    public PipelineCoordinator(PipelineService pipeline, PipelineStatus status)
    {
        _pipeline = pipeline;
        _status = status;
    }

    public Task StartAsync(CancellationToken cancellationToken) => StartPipelineAsync(cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopPipelineAsync();
    }

    public async Task<bool> StartPipelineAsync(CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_pipelineTask is { IsCompleted: false })
                return false;

            _pipelineCancellation = new CancellationTokenSource();
            _pipelineTask = _pipeline.RunAsync(_pipelineCancellation.Token);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> StopPipelineAsync()
    {
        Task? runningTask;

        await _lock.WaitAsync();
        try
        {
            if (_pipelineTask is null || _pipelineTask.IsCompleted)
                return false;

            _pipelineCancellation!.Cancel();
            runningTask = _pipelineTask;
        }
        finally
        {
            _lock.Release();
        }

        try
        {
            await runningTask;
        }
        catch (OperationCanceledException)
        {
            // L'arrêt est volontaire.
        }

        _status.IsRunning = false;
        return true;
    }

    public async Task RestartPipelineAsync(CancellationToken cancellationToken = default)
    {
        await StopPipelineAsync();
        await StartPipelineAsync(cancellationToken);
    }
}
