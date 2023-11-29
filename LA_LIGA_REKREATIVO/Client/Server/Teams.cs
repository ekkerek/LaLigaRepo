using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Teams
    {
        private readonly HttpClient _httpClient;

        public Teams(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> Add(TeamDto team)
        {
            //team.ParticipantOf = "2016";
            //team.Players = new List<Player>();

            var message = new HttpRequestMessage(HttpMethod.Post, "api/team/addNew");
            message.Content = new StringContent(JsonConvert.SerializeObject(team));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                await Console.Out.WriteLineAsync("test failed");
            }
            return result.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<TeamDto>> Get()
        {
            var result = await _httpClient.GetAsync("api/team");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamDto>>(json);
            }
            return Enumerable.Empty<TeamDto>();
        }
    }
}
