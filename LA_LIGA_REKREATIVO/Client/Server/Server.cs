

namespace LA_LIGA_REKREATIVO.Client.Server
{
    public class Server
    {
        private HttpClient _httpClient;
        public Server(HttpClient httpClient)
        {
            Teams = new(httpClient);
            Players = new(httpClient);
            Matches = new(httpClient);
            Leagues = new(httpClient);
            _httpClient = httpClient;
        }

        public Teams Teams { get; }
        public Players Players { get; }
        public Matches Matches { get; }
        public Leagues Leagues { get; }
    }
}
