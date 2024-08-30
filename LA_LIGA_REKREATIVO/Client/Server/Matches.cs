using LA_LIGA_REKREATIVO.Client.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Matches
    {
        private readonly HttpClient _httpClient;
        JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;

        public Matches(HttpClient httpClient, JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public async Task<IEnumerable<MatchDto>> Get()
        {
            var result = await _httpClient.GetAsync("api/match");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchDto>>(json);
            }
            return Enumerable.Empty<MatchDto>();
        }

        public async Task<MatchDto> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/match/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MatchDto>(json);
            }
            return new MatchDto();
        }

        public async Task<IEnumerable<MatchDto>> GetByDate(DateTime dateTime)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match/getByDate");
            message.Content = new StringContent(JsonConvert.SerializeObject(dateTime));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchDto>>(json);
            }
            return Enumerable.Empty<MatchDto>();
        }

        public async Task<IEnumerable<MatchesByRoundDto>> GetByRound(int leagueId)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match/getByRound");
            message.Content = new StringContent(JsonConvert.SerializeObject(leagueId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByRoundDto>>(json);
            }
            return Enumerable.Empty<MatchesByRoundDto>();
        }

        public async Task<IEnumerable<MatchesByRoundDto>> GetByRoundOverall()
        {
            var result = await _httpClient.GetAsync($"api/match/getByRoundOverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByRoundDto>>(json);
            }
            return new List<MatchesByRoundDto>();
        }

        public async Task<IEnumerable<MatchesByGameTimeDto>> GetByGameTime(int leagueId)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match/getByGameTime");
            message.Content = new StringContent(JsonConvert.SerializeObject(leagueId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByGameTimeDto>>(json);
            }
            return Enumerable.Empty<MatchesByGameTimeDto>();
        }

        public async Task<IEnumerable<MatchesByGameTimeDto>> GetByGameTimeOverall()
        {
            var result = await _httpClient.GetAsync($"api/match/getByGameTimeOverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByGameTimeDto>>(json);
            }
            return new List<MatchesByGameTimeDto>();
        }

        public async Task<IEnumerable<MatchDto>> GetPlayOffMatches()
        {
            var result = await _httpClient.GetAsync($"api/match/getPlayOffMatches");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchDto>>(json);
            }
            return new List<MatchDto>();
        }

        public async Task<IEnumerable<MatchesByGameTimeDto>> GetFixturesByLeague(int leagueId)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match/getFixturesByLeague");
            message.Content = new StringContent(JsonConvert.SerializeObject(leagueId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByGameTimeDto>>(json);
            }
            return Enumerable.Empty<MatchesByGameTimeDto>();
        }

        public async Task<IEnumerable<MatchesByGameTimeDto>> GetFixturesOverall()
        {
            var result = await _httpClient.GetAsync($"api/match/getFixturesOverall");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchesByGameTimeDto>>(json);
            }
            return new List<MatchesByGameTimeDto>();
        }

        public async Task<IEnumerable<MatchByTeamDto>> GetByTeam(int teamId)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/match/getByTeam/{teamId}");
            message.Content = new StringContent(JsonConvert.SerializeObject(teamId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchByTeamDto>>(json);
            }
            return Enumerable.Empty<MatchByTeamDto>();
        }

        public async Task<IEnumerable<MatchByPlayerDto>> GetByPlayer(int playerId)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/match/getByPlayer/{playerId}");
            message.Content = new StringContent(JsonConvert.SerializeObject(playerId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);

            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<MatchByPlayerDto>>(json);
            }
            return Enumerable.Empty<MatchByPlayerDto>();
        }

        public async Task<LeagueStatisticDto> GetLeagueStatistic()
        {
            var result = await _httpClient.GetAsync($"api/match/getLeagueStatistic");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LeagueStatisticDto>(json);
            }
            return new LeagueStatisticDto();
        }

        public async Task<bool> Add(MatchDto match)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, "api/match");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(match));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            if (result.IsSuccessStatusCode)
            {
                await Console.Out.WriteLineAsync("test failed");
            }
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int matchId)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/match/delete");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(matchId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(MatchDto match)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Put, $"api/match/{match.Id}");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(match));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }
    }
}
