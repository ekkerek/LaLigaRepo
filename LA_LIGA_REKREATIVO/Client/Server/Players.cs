using LA_LIGA_REKREATIVO.Client.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Players
    {
        private readonly HttpClient _httpClient;
        JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;

        public Players(HttpClient httpClient, JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public async Task<bool> Add(PlayerDto player)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/player");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(player));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(PlayerDto player)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/player/delete");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(player));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(PlayerDto player)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Put, $"api/player/{player.Id}");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

        public async Task<IEnumerable<PlayerStatsDto>> GetPlayersStats(int leagueId)
        {
            var result = await _httpClient.GetAsync($"api/player/getplayersstats/{leagueId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetPlayersStatsOverall()
        {
            var result = await _httpClient.GetAsync($"api/player/getplayersstatsoverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetDreamTeamByLeague(int leagueId)
        {
            var result = await _httpClient.GetAsync($"api/player/getDreamTeam/{leagueId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetDreamTeamOverall()
        {
            var result = await _httpClient.GetAsync($"api/player/getDreamTeamOverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> Get2ndDreamTeamByLeague(int leagueId)
        {
            var result = await _httpClient.GetAsync($"api/player/get2ndDreamTeam/{leagueId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> Get2ndDreamTeamOverall()
        {
            var result = await _httpClient.GetAsync($"api/player/get2ndDreamTeamOverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetTeamPlayers(int teamId)
        {
            var result = await _httpClient.GetAsync($"api/player/getTeamPlayers/{teamId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetTopGoalscorer()
        {
            var result = await _httpClient.GetAsync($"api/player/getTopGoalscorer");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }

        public async Task<IEnumerable<PlayerStatsDto>> GetTopAssitent()
        {
            var result = await _httpClient.GetAsync($"api/player/getTopAssitent");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<PlayerStatsDto>>(json);
            }
            return Enumerable.Empty<PlayerStatsDto>();
        }
    }
}
