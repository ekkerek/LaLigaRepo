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

        public async Task<PlayerStatsDto> GetPlayerStats()
        {
            var result = await _httpClient.GetAsync($"api/player/getplayerstats/{4}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PlayerStatsDto>(json);
            }
            return new PlayerStatsDto();
        }
    }
}
