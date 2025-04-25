using LA_LIGA_REKREATIVO.Server.Services;
using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("postFile")]
        public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files)
        {
            var uploadedFiles = await _uploadService.PostFile(files);
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            return new CreatedResult(resourcePath, uploadedFiles);
        }
    }
}
