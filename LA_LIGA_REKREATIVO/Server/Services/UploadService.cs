using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public class UploadService : IUploadService
    {
        public async Task<IList<UploadResult>> PostFile([FromForm] IEnumerable<IFormFile> files)
        {
            var maxAllowedFiles = 3;
            long maxFileSize = 1024 * 1024;
            var filesProcessed = 0;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

            List<UploadResult> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                var trustedFileNameForDisplay =
                    WebUtility.HtmlEncode(untrustedFileName);

                if (filesProcessed < maxAllowedFiles)
                {
                    if (file.Length == 0)
                    {
                        //logger.LogInformation("{FileName} length is 0 (Err: 1)",
                        //    trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                    }
                    else if (file.Length > maxFileSize)
                    {
                        //logger.LogInformation("{FileName} of {Length} bytes is " +
                        //    "larger than the limit of {Limit} bytes (Err: 2)",
                        //    trustedFileNameForDisplay, file.Length, maxFileSize);
                        uploadResult.ErrorCode = 2;
                    }
                    else
                    {
                        try
                        {
                            var fileNames = Directory.GetFiles("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot");
                            var fnames = new List<string>();
                            foreach (string f in fileNames)
                                fnames.Add(Path.GetFileName(f));

                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            var path = Path.Combine("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot",
                          file.FileName);
                            //var path = "C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot";

                            await using FileStream fs = new(path, FileMode.Create);
                            await file.CopyToAsync(fs);

                            //logger.LogInformation("{FileName} saved at {Path}",
                            //    trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = trustedFileNameForFileStorage;
                        }
                        catch (IOException ex)
                        {
                            //logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                            //    trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }
                    }

                    filesProcessed++;
                }
                else
                {
                    //logger.LogInformation("{FileName} not uploaded because the " +
                    //    "request exceeded the allowed {Count} of files (Err: 4)",
                    //    trustedFileNameForDisplay, maxAllowedFiles);
                    uploadResult.ErrorCode = 4;
                }

                uploadResults.Add(uploadResult);
            }

            return uploadResults;
        }
    }
}
