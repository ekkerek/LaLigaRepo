using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Players
    {
        private readonly HttpClient _httpClient;

        public Players(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Add(PlayerDto player)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/player");
            message.Content = new StringContent(JsonConvert.SerializeObject(player));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<PlayerDto>> Get()
        {
            var result = await _httpClient.GetAsync("api/player");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PlayerDto>>(json);
            }
            return Enumerable.Empty<PlayerDto>();
        }

        public async Task<PlayerDto> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/player/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PlayerDto>(json);
            }
            return new PlayerDto();
        }

        public async Task<PlayerStatsDto> GetPlayerStats(int id)
        {
            var result = await _httpClient.GetAsync($"api/player/getplayerstats/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PlayerStatsDto>(json);
            }
            return new PlayerStatsDto();
        }

        public async Task<List<PlayerStatsDto>> GetPlayersStats(int leagueId, int count)
        {
            var result = await _httpClient.GetAsync($"api/player/getplayersstats/{leagueId}/{count}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PlayerStatsDto>>(json);
            }
            return new List<PlayerStatsDto>();
        }

        public async Task<List<PlayerStatsDto>> GetDreamTeamByLeague(int leagueId)
        {
            var result = await _httpClient.GetAsync($"api/player/getDreamTeam/{leagueId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PlayerStatsDto>>(json);
            }
            return new List<PlayerStatsDto>();
        }

        public async Task<bool> Update(PlayerDto player)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, $"api/player/{player.Id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(player));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }
    }
}
