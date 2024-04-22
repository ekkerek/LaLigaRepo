using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LA_LIGA_REKREATIVO.Client.Services
{
    public class FileService
    {
        private HttpClient _httpClient;
        private const string BUCKET_PATH = "https://test-la-liga-bucket.s3.amazonaws.com/";

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private List<UploadResult> uploadResults = new();
        private int maxAllowedFiles = 3;
        private bool shouldRender;

        public async Task<List<UploadResult>> UploadFile(InputFileChangeEventArgs e)
        {
            shouldRender = false;
            long maxFileSize = 1024 * 1024;
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                if (uploadResults.SingleOrDefault(
                    f => f.FileName == file.Name) is null)
                {
                    try
                    {
                        var fileContent =
                            new StreamContent(file.OpenReadStream(maxFileSize));

                        fileContent.Headers.ContentType =
                            new MediaTypeHeaderValue(file.ContentType);

                        content.Add(
                            content: fileContent,
                            name: "\"files\"",
                            fileName: file.Name);

                        upload = true;
                    }
                    catch (Exception ex)
                    {
                        uploadResults.Add(
                            new()
                            {
                                FileName = file.Name,
                                ErrorCode = 6,
                                Uploaded = false
                            });
                    }
                }
            }

            if (upload)
            {
                var response = await _httpClient.PostAsync("/api/upload/postFile", content);
                var newUploadResults = await response.Content
                .ReadFromJsonAsync<IList<UploadResult>>();

                if (newUploadResults is not null)
                {
                    uploadResults = new List<UploadResult>(newUploadResults);//uploadResults.Concat(newUploadResults).ToList();
                }
            }
            shouldRender = true;
            return uploadResults;
        }

        public string GetImagePath(string imageName)
        {
            return BUCKET_PATH + imageName;
        }
    }
}
