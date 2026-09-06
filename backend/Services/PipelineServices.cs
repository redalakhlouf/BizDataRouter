using BizDataRouter.CsvFilesInfo;
using BizDataRouter.MinioStorage;
using BizDataRouter.Models;
using BizDataRouter.Settings;
using BizDataRouter.Status;
using MinioStorageService = BizDataRouter.MinioStorage.MinioStorage;

namespace BizDataRouter.Services;

public sealed class PipelineService
{
    private readonly PipelineStatus _status;
    private readonly PiApiReader _reader;
    private readonly MinioStorageService _minio;
    private readonly AppSettings _settings;

    public PipelineService(
        PipelineStatus status,
        PiApiReader reader,
        MinioStorageService minio,
        AppSettings settings)
    {
        _status = status;
        _reader = reader;
        _minio = minio;
        _settings = settings;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _status.IsRunning = true;
        _status.StartedAt = DateTime.UtcNow;

        try
        {
            await _minio.CheckConnectionAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    PiBatch batch = await _reader.ReadAsync(cancellationToken);
                    var csvFile = new CsvFileInfo(batch, _settings, _status);
                    string? fileToUpload = csvFile.WriteCsvFile();

                    if (fileToUpload is not null)
                        await _minio.UploadAsync(fileToUpload, cancellationToken);

                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch
                {
                    // Les services PI et MinIO mettent Ã  jour leur Ã©tat de connexion.
                    // La boucle continue pour permettre une nouvelle tentative.
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    cancellationToken);
            }
        }
        finally
        {
            _status.IsRunning = false;
        }
    }
}
