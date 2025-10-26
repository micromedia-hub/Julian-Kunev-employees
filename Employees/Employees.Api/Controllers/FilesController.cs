using Employees.Application.Parsing;
using Employees.Application.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class FilesController : ControllerBase
    {
        private readonly ICsvParser _csvParser;
        private readonly IParsedDataStore _parsedDataStore;

        public FilesController(ICsvParser csvParser, IParsedDataStore parsedDataStore)
        {
            _csvParser = csvParser;
            _parsedDataStore = parsedDataStore;
        }

        [HttpPost("parse")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Parse(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }
            try
            {
                await using var stream = file.OpenReadStream();
                var (valid, echo, errors) = await _csvParser.ParseAsync(stream, cancellationToken);
                _parsedDataStore.SetAssignments(valid);
                return Ok(new
                {
                    totalValid = valid.Count,
                    totalErrors = errors.Count,
                    echo
                });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, title: "Parse failed", statusCode: 500);
            }
        }
    }
}
