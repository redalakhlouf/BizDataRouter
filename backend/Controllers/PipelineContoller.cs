using BizDataRouter.Models;
using BizDataRouter.Services;
using BizDataRouter.Status;
using Microsoft.AspNetCore.Mvc;

namespace BizDataRouter.Controllers;

[ApiController]
[Route("api/pipeline")]
public sealed class PipelineController : ControllerBase
{
    private readonly PipelineCoordinator _coordinator;
    private readonly PipelineStatus _status;

    public PipelineController(PipelineCoordinator coordinator, PipelineStatus status)
    {
        _coordinator = coordinator;
        _status = status;
    }

    [HttpPost("start")]
    [ProducesResponseType<PipelineCommandResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PipelineCommandResponseDto>> Start(CancellationToken cancellationToken)
    {
        bool started = await _coordinator.StartPipelineAsync(cancellationToken);
        return Ok(new PipelineCommandResponseDto(started, StatusDto.FromStatus(_status)));
    }

    [HttpPost("stop")]
    [ProducesResponseType<PipelineCommandResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PipelineCommandResponseDto>> Stop()
    {
        bool stopped = await _coordinator.StopPipelineAsync();
        return Ok(new PipelineCommandResponseDto(stopped, StatusDto.FromStatus(_status)));
    }

    [HttpPost("restart")]
    [ProducesResponseType<PipelineCommandResponseDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PipelineCommandResponseDto>> Restart(CancellationToken cancellationToken)
    {
        await _coordinator.RestartPipelineAsync(cancellationToken);
        return Ok(new PipelineCommandResponseDto(true, StatusDto.FromStatus(_status)));
    }
}
