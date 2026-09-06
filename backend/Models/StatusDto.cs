using BizDataRouter.Status;

namespace BizDataRouter.Models;

public sealed record CurrentFileDto(string Name, long SizeBytes);

public sealed record StatusDto(
    bool IsRunning,
    bool PiApiConnected,
    bool MinioConnected,
    DateTime? LastCollectionAt,
    DateTime? LastUploadAt,
    CurrentFileDto? CurrentFile,
    string? LastUploadedFile,
    DateTime? StartedAt)
{
    public static StatusDto FromStatus(PipelineStatus status)
    {
        CurrentFileDto? currentFile = status.CurrentFileName is null
            ? null
            : new CurrentFileDto(
                status.CurrentFileName,
                status.CurrentFileSizeBytes ?? 0);

        return new StatusDto(
            status.IsRunning,
            status.PiApiConnected,
            status.MinioConnected,
            status.LastCollectionAt,
            status.LastUploadAt,
            currentFile,
            status.LastUploadedFile,
            status.StartedAt);
    }
}
