namespace BizDataRouter.Models;

public sealed record PipelineCommandResponseDto(
    bool Succeeded,
    StatusDto Status);
