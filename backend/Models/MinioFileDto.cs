namespace BizDataRouter.Models;

public sealed record MinioFileDto(string Name, long SizeBytes, DateTime? LastModified);
