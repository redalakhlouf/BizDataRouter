using BizDataRouter.Models;
using BizDataRouter.Status;
using Microsoft.AspNetCore.Mvc;

namespace BizDataRouter.Controllers;

[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    private readonly PipelineStatus _status;

    public StatusController(PipelineStatus status)
    {
        _status = status;
    }

    [HttpGet]
    [ProducesResponseType<StatusDto>(StatusCodes.Status200OK)]
    public ActionResult<StatusDto> GetStatus()
    {
        return Ok(StatusDto.FromStatus(_status));
    }
}
