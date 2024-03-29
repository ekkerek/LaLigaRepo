using LA_LIGA_REKREATIVO.Shared;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace LA_LIGA_REKREATIVO.Client.Services
{

    public class File
    {
        public string? Name { get; set; }
    }

    public class UploadService
    {
        private HttpClient _httpClient;
        public UploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        private List<File> files = new();
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
                        files.Add(new() { Name = file.Name });

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
                        // Logger.LogInformation(
                        //     "{FileName} not uploaded (Err: 6): {Message}",
                        //     file.Name, ex.Message);

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
                var response = await _httpClient.PostAsync("/api/league/postFile", content);
                // var message = new HttpRequestMessage(HttpMethod.Post, "api/league/postFile");
                // message.Content = new StringContent(JsonConvert.SerializeObject(content));
                // message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                // var result = await _httpClient.SendAsync(message);

                // if (result.IsSuccessStatusCode)
                // {
                //     var json = await result.Content.ReadAsStringAsync();
                //     //return JsonConvert.DeserializeObject<List<MatchDto>>(json);
                // }
                //return Enumerable.Empty<MatchDto>();

                var newUploadResults = await response.Content
                .ReadFromJsonAsync<IList<UploadResult>>();

                if (newUploadResults is not null)
                {
                    uploadResults = uploadResults.Concat(newUploadResults).ToList();
                }
            }
            shouldRender = true;
            return uploadResults;
        }
    }
}
