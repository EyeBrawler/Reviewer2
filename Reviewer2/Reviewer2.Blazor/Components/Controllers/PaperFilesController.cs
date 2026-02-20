using System.Security.Claims;
using Reviewer2.Data.Models;

namespace Reviewer2.Blazor.Components.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Submissions.PaperSubmission;

[ApiController]
[Route("api/papers/{paperId:guid}/files/{fileType}")]
[Authorize] // optional: require authenticated users
public class PaperFilesController(IPaperQueryService paperQueryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaperFile(Guid paperId, string fileType)
    {
        // Convert string to FileType enum
        if (!Enum.TryParse(fileType, ignoreCase: true, out FileType parsedFileType))
            return BadRequest("Invalid file type.");

        // Use current user ID
        var userId = Guid.Parse(User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var fileResult = await paperQueryService.GetPaperFileAsync(paperId, parsedFileType, userId);

        if (!fileResult.Success || fileResult.Stream == null)
            return NotFound(fileResult.ErrorMessage);

        return File(fileResult.Stream, "application/pdf", enableRangeProcessing: true);
    }
}
