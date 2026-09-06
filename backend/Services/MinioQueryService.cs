using BizDataRouter.Models;
using BizDataRouter.Settings;
using BizDataRouter.Status;
using Minio;
using Minio.DataModel.Args;

namespace BizDataRouter.Services;

public sealed class MinioQueryService
{
    private readonly IMinioClient _minioClient;
    private readonly AppSettings _settings;
    private readonly PipelineStatus _status;

    public MinioQueryService(
        IMinioClient minioClient,
        AppSettings settings,
        PipelineStatus status)
    {
        _minioClient = minioClient;
        _settings = settings;
        _status = status;
    }

    public async Task<IReadOnlyList<MinioFileDto>> GetFilesAsync(CancellationToken cancellationToken)
    {
        var files = new List<MinioFileDto>();

        try
        {
            await foreach (var item in _minioClient.ListObjectsEnumAsync(
                new ListObjectsArgs().WithBucket(_settings.Minio.BucketName),
                cancellationToken))
            {
                DateTime? lastModified = DateTime.TryParse(item.LastModified, out DateTime value)
                    ? value
                    : null;
                files.Add(new MinioFileDto(item.Key, checked((long)item.Size), lastModified));
            }

            _status.MinioConnected = true;
            return files.OrderByDescending(file => file.LastModified).ToList();
        }
        catch
        {
            _status.MinioConnected = false;
            throw;
        }
    }

    public async Task<MinioFileDto?> GetFileAsync(string fileName, CancellationToken cancellationToken)
    {
        try
        {
            var stat = await _minioClient.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(_settings.Minio.BucketName)
                    .WithObject(fileName),
                cancellationToken);

            _status.MinioConnected = true;
            return new MinioFileDto(fileName, stat.Size, stat.LastModified);
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            _status.MinioConnected = true;
            return null;
        }
        catch
        {
            _status.MinioConnected = false;
            throw;
        }
    }
}
