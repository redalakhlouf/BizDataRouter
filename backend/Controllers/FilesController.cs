using BizDataRouter.Models;
using BizDataRouter.Services;
using Microsoft.AspNetCore.Mvc;

namespace BizDataRouter.Controllers;

[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly MinioQueryService _minioQueryService;

    public FilesController(MinioQueryService minioQueryService)
    {
        _minioQueryService = minioQueryService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MinioFileDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MinioFileDto>>> GetFiles(CancellationToken cancellationToken)
    {
        return Ok(await _minioQueryService.GetFilesAsync(cancellationToken));
    }

    [HttpGet("{fileName}")]
    [ProducesResponseType<MinioFileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MinioFileDto>> GetFile(string fileName, CancellationToken cancellationToken)
    {
        MinioFileDto? file = await _minioQueryService.GetFileAsync(fileName, cancellationToken);
        return file is null ? NotFound() : Ok(file);
    }
}
