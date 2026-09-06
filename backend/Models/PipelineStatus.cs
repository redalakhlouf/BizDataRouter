namespace BizDataRouter.Status;

public sealed class PipelineStatus
{
    public bool IsRunning { get; set; }
    public DateTime? StartedAt { get; set; }
    public bool PiApiConnected { get; set; }
    public bool MinioConnected { get; set; }
    public DateTime? LastCollectionAt { get; set; }
    public DateTime? LastUploadAt { get; set; }
    public string? CurrentFileName { get; set; }
    public long? CurrentFileSizeBytes { get; set; }
    public string? LastUploadedFile { get; set; }
}
