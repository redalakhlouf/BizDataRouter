using BizDataRouter.Models;
using BizDataRouter.Services;
using Microsoft.AspNetCore.Mvc;

namespace BizDataRouter.Controllers;

[ApiController]
[Route("api/data")]
public sealed class DataController : ControllerBase
{
    private readonly DataQueryService _dataQueryService;

    public DataController(DataQueryService dataQueryService)
    {
        _dataQueryService = dataQueryService;
    }

    [HttpGet]
    [ProducesResponseType<DailyDataResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DailyDataResponse>> GetDailyData(
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        DailyDataResponse result = await _dataQueryService.GetDataForDayAsync(date, cancellationToken);

        if (result.TotalFiles == 0)
            return NotFound(new { message = $"Aucune donnée trouvée pour le {date:yyyy-MM-dd}." });

        return Ok(result);
    }
}
