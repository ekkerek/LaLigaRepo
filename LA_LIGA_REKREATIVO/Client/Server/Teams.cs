using LA_LIGA_REKREATIVO.Client.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Teams
    {
        private readonly HttpClient _httpClient;
        JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;

        public Teams(HttpClient httpClient, JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }
        public async Task<bool> Add(TeamDto team)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, "api/team/addNew");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(team));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                await Console.Out.WriteLineAsync("test failed");
            }
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int teamId)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/team/delete");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(teamId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(TeamDto team)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Put, $"api/team/{team.Id}");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(team));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
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

        public async Task<TeamDto> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/team/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TeamDto>(json);
            }
            return new TeamDto();
        }

        public async Task<List<TeamDto>> GetTeamsByLeague(int leagueId)
        {
            var result = await _httpClient.GetAsync($"api/team/getTeamsByLeague/{leagueId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamDto>>(json);
            }
            return new List<TeamDto>();
        }

        public async Task<TeamStatsDto> GetTeamStats(int teamId)
        {
            var result = await _httpClient.GetAsync($"api/team/getTeamStats/{teamId}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TeamStatsDto>(json);
            }
            return new TeamStatsDto();
        }

        public async Task<List<TeamStatsDto>> GetStandingsByLeague(int id)
        {
            var result = await _httpClient.GetAsync($"api/team/getStandingsByLeague/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamStatsDto>>(json);
            }
            return new List<TeamStatsDto>();
        }

        public async Task<List<TeamStatsDto>> GetCommonStanding()
        {
            var result = await _httpClient.GetAsync($"api/team/getCommonStanding");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamStatsDto>>(json);
            }
            return new List<TeamStatsDto>();
        }
    }
}
