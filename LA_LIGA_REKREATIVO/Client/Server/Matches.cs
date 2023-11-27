using LA_LIGA_REKREATIVO.Shared.Dto;
using LA_LIGA_REKREATIVO.Shared.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Matches
    {
        private readonly HttpClient _httpClient;

        public Matches(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Add(MatchDto match)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match");
            message.Content = new StringContent(JsonConvert.SerializeObject(match));

            var kk = JsonConvert.SerializeObject(match);
            //message.Content = new StringContent(JsonConvert.SerializeObject(match, Formatting.Indented, new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            //}));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                await Console.Out.WriteLineAsync("test failed");
            }
            return result.IsSuccessStatusCode;
        }
    }
}
