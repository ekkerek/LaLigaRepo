using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Mvc;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public interface IUploadService
    {
        Task<IList<UploadResult>> PostFile([FromForm] IEnumerable<IFormFile> files);
    }
}
