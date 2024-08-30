using LA_LIGA_REKREATIVO.Client.Services;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Leagues
    {
        private readonly HttpClient _httpClient;
        JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;

        public Leagues(HttpClient httpClient, JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public async Task<bool> Add(LeagueDto league)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/league");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(LeagueDto league)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Post, $"api/league/delete");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Content = new StringContent(JsonConvert.SerializeObject(league));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(LeagueDto league)
        {
            var token = await _jwtAuthenticationStateProvider.GetJwtToken();

            var message = new HttpRequestMessage(HttpMethod.Put, $"api/league/{league.Id}");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

        public async Task<IEnumerable<LeagueDto>> GetNonPlayoffLeague()
        {
            var result = await _httpClient.GetAsync("api/league/getNonPlayoffLeague");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<LeagueDto>>(json);
            }
            return Enumerable.Empty<LeagueDto>();
        }
    }
}
