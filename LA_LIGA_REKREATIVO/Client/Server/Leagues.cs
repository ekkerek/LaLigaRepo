using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Leagues
    {
        private readonly HttpClient _httpClient;
        public Leagues(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Add(LeagueDto league)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/league");
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(LeagueDto league)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/league/delete");
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(LeagueDto league)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, $"api/league/{league.Id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<LeagueDto>> Get()
        {
            var result = await _httpClient.GetAsync("api/league");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<LeagueDto>>(json);
            }
            return Enumerable.Empty<LeagueDto>();
        }

        public async Task<LeagueDto> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/league/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LeagueDto>(json);
            }
            return new LeagueDto();
        }

        public async Task<bool> AddTeamsToLeague(LeagueDto league)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/league/addTeams");
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }
    }
}
