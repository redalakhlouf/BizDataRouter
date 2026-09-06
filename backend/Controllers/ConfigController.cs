using BizDataRouter.Models;
using BizDataRouter.Settings;
using Microsoft.AspNetCore.Mvc;

namespace BizDataRouter.Controllers;

[ApiController]
[Route("api/config")]
public sealed class ConfigController : ControllerBase
{
    private readonly AppSettings _settings;

    public ConfigController(AppSettings settings)
    {
        _settings = settings;
    }

    [HttpGet]
    [ProducesResponseType<ConfigDto>(StatusCodes.Status200OK)]
    public ActionResult<ConfigDto> GetConfiguration()
    {
        return Ok(ConfigDto.FromSettings(_settings));
    }
}
