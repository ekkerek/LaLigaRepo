﻿using LA_LIGA_REKREATIVO.Shared.Dto;
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
        public async Task<List<TeamStatsDto>> GetStandingsByLeague(int id)
        {
            var result = await _httpClient.GetAsync($"api/match/getStandingsByLeague/{id}");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamStatsDto>>(json);
            }
            return new List<TeamStatsDto>();
        }

        public async Task<List<TeamStatsDto>> GetCommonStanding()
        {
            var result = await _httpClient.GetAsync($"api/match/getCommonStanding");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TeamStatsDto>>(json);
            }
            return new List<TeamStatsDto>();
        }


        public async Task<List<LeagueStatsDto>> GetStandings()
        {
            var result = await _httpClient.GetAsync($"api/match/getStandings");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<LeagueStatsDto>>(json);
            }
            return new List<LeagueStatsDto>();
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

        public async Task<bool> Add(MatchDto match)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "api/match");
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
            var message = new HttpRequestMessage(HttpMethod.Post, $"api/match/delete");
            message.Content = new StringContent(JsonConvert.SerializeObject(matchId));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Update(MatchDto match)
        {
            var message = new HttpRequestMessage(HttpMethod.Put, $"api/match/{match.Id}");
            message.Content = new StringContent(JsonConvert.SerializeObject(match));
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.SendAsync(message);
            return result.IsSuccessStatusCode;
        }
    }
}
