using Amazon.S3;
using Amazon.S3.Model;
using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LA_LIGA_REKREATIVO.Server.Services
{
    public class UploadService : IUploadService
    {
        private readonly IAmazonS3 _s3Client;
        private const string bucketName = "test-la-liga-bucket";

        public UploadService(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<IList<UploadResult>> PostFile([FromForm] IEnumerable<IFormFile> files)
        {
            //var bucketName = "test-la-liga-bucket";
            //var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            //if (!bucketExists) return NotFound($"Bucket {bucketName} does not exist.");
            var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(files?.FirstOrDefault()?.FileName);

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = uniqueFileName, //files?.FirstOrDefault()?.FileName,
                InputStream = files?.FirstOrDefault()?.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", files?.FirstOrDefault()?.ContentType);
            await _s3Client.PutObjectAsync(request);


            var maxAllowedFiles = 3;
            long maxFileSize = 20024 * 1024;
            var filesProcessed = 0;
            //var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            // test
            List<UploadResult> uploadResults = new();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                //string trustedFileNameForFileStorage;
                //var untrustedFileName = file.FileName;
                //uploadResult.FileName = untrustedFileName;
                //var trustedFileNameForDisplay =
                //    WebUtility.HtmlEncode(untrustedFileName);

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
                            //var fileNames = Directory.GetFiles("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot");
                            //var fnames = new List<string>();
                            //foreach (string f in fileNames)
                            //    fnames.Add(Path.GetFileName(f));

                          //  trustedFileNameForFileStorage = Path.GetRandomFileName();
                          //  var path = Path.Combine("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot",
                          //file.FileName);
                            //var path = "C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot";

                            //await using FileStream fs = new(path, FileMode.Create);
                            //await file.CopyToAsync(fs);

                            //logger.LogInformation("{FileName} saved at {Path}",
                            //    trustedFileNameForDisplay, path);
                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = uniqueFileName;
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
