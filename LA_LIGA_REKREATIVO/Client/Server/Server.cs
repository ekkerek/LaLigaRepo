using LA_LIGA_REKREATIVO.Client.Services;

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Server
    {
        private HttpClient _httpClient;
        JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;
        public Server(HttpClient httpClient, JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            Teams = new(httpClient, jwtAuthenticationStateProvider);
            Players = new(httpClient, jwtAuthenticationStateProvider);
            Matches = new(httpClient, jwtAuthenticationStateProvider);
            Leagues = new(httpClient, jwtAuthenticationStateProvider);
            _httpClient = httpClient;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public Teams Teams { get; }
        public Players Players { get; }
        public Matches Matches { get; }
        public Leagues Leagues { get; }
    }
}
