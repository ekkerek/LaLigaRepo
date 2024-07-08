using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public MatchController(LaLigaContext context, IMapper mapper, IMemoryCache memoryCache)
        {
            _context = context;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        // GET: api/<MatchController>
        [HttpGet]
        public IEnumerable<MatchDto> Get()
        {
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Summaries).ThenInclude(x => x.Player).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(_context.Matches);
            if (matches is null)
            {
                return new List<MatchDto>();
            }
            foreach (var match in matchesDto)
            {
                var homeTeamId = matches.FirstOrDefault(x => x.Id == match.Id).HomeTeamId;
                var awayTeamId = matches.FirstOrDefault(x => x.Id == match.Id).AwayTeamId;
                var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                match.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                match.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            }
            return matchesDto;
        }


        [HttpPost("getByDate")]
        public IEnumerable<MatchDto> GetByDate([FromBody] DateTime date)
        {
            var matches = _context.Matches.Include(x => x.Players).Where(x => x.GameTime.ToUniversalTime().Date == date.ToUniversalTime().Date).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches);
            foreach (var match in matchesDto)
            {
                var homeTeamId = matches.FirstOrDefault(x => x.Id == match.Id).HomeTeamId;
                var awayTeamId = matches.FirstOrDefault(x => x.Id == match.Id).AwayTeamId;
                var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                match.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                match.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            }
            return matchesDto;
        }

        [HttpPost("getByRound")]
        public IEnumerable<MatchesByRoundDto> GetByRound([FromBody] int leagueId)
        {
            List<MatchesByRoundDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Players).Where(x => x.League.Id == leagueId).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => x.GameRound);

            foreach (var match in matchesDto)
            {
                MatchesByRoundDto matchesByRoundDto = new();
                matchesByRoundDto.Round = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByRoundDto.Matches.Add(groupedMatch);
                }
                result.Add(matchesByRoundDto);
            }
            return result.OrderByDescending(x => x.Round);
        }

        [HttpGet("getByRoundOverall")]
        public IEnumerable<MatchesByRoundDto> GetByRoundOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<MatchesByRoundDto>>("getByRoundOverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            List<MatchesByRoundDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Players).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => x.GameRound);

            foreach (var match in matchesDto)
            {
                MatchesByRoundDto matchesByRoundDto = new();
                matchesByRoundDto.Round = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByRoundDto.Matches.Add(groupedMatch);
                }
                result.Add(matchesByRoundDto);
            }

            var expirationTime = DateTimeOffset.Now.AddDays(5);
            cacheData = result.OrderByDescending(x => x.Round);//await _dbContext.Products.ToListAsync();
            _memoryCache.Set("getByRoundOverall", cacheData, expirationTime);
            return cacheData;

            //return result;
        }

        [HttpPost("getByGameTime")]
        public IEnumerable<MatchesByGameTimeDto> GetByGameTime([FromBody] int leagueId)
        {
            var cacheData = _memoryCache.Get<IEnumerable<MatchesByGameTimeDto>>($"getByGameTime-{leagueId}");
            if (cacheData != null)
            {
                return cacheData;
            }

            List<MatchesByGameTimeDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Players).Where(x => x.League.Id == leagueId && x.GameTime < DateTime.UtcNow && (x.Players.Count() > 0 || x.IsOfficialResult)).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => new DateTime(x.GameTime.Year, x.GameTime.Month, x.GameTime.Day));

            foreach (var match in matchesDto)
            {
                MatchesByGameTimeDto matchesByRoundDto = new();
                matchesByRoundDto.GameTime = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByRoundDto.Matches.Add(groupedMatch);
                }
                result.Add(matchesByRoundDto);
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = result.OrderByDescending(x => x.GameTime);
            _memoryCache.Set($"getByGameTime-{leagueId}", cacheData, expirationTime);
            return cacheData;
        }


        [HttpGet("getByGameTimeOverall")]
        public IEnumerable<MatchesByGameTimeDto> GetByGameTimeOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<MatchesByGameTimeDto>>("getByGameTimeOverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            List<MatchesByGameTimeDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Players).Where(x => x.GameTime < DateTime.UtcNow && (x.Players.Count() > 0 || x.IsOfficialResult)).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => new DateTime(x.GameTime.Year, x.GameTime.Month, x.GameTime.Day));

            foreach (var match in matchesDto)
            {
                MatchesByGameTimeDto matchesByGameTime = new();
                matchesByGameTime.GameTime = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByGameTime.Matches.Add(groupedMatch);
                }
                result.Add(matchesByGameTime);
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = result.OrderByDescending(x => x.GameTime);//await _dbContext.Products.ToListAsync();
            _memoryCache.Set("getByGameTimeOverall", cacheData, expirationTime);
            return cacheData;

            //return result;
        }

        [HttpPost("getFixturesByLeague")]
        public IEnumerable<MatchesByGameTimeDto> GetFixturesByLeague([FromBody] int leagueId)
        {
            List<MatchesByGameTimeDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Where(x => x.League.Id == leagueId && (x.GameTime > DateTime.UtcNow || (x.Players.Count() == 0) && !x.IsOfficialResult)).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => new DateTime(x.GameTime.Year, x.GameTime.Month, x.GameTime.Day));

            foreach (var match in matchesDto)
            {
                MatchesByGameTimeDto matchesByRoundDto = new();
                matchesByRoundDto.GameTime = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByRoundDto.Matches.Add(groupedMatch);
                }
                result.Add(matchesByRoundDto);
            }
            return result.OrderByDescending(x => x.GameTime);
        }

        [HttpGet("getFixturesOverall")]
        public IEnumerable<MatchesByGameTimeDto> GetFixturesOverall()
        {
            var cacheData = _memoryCache.Get<IEnumerable<MatchesByGameTimeDto>>("getFixturesOverall");
            if (cacheData != null)
            {
                return cacheData;
            }

            List<MatchesByGameTimeDto> result = new();
            var matches = _context.Matches.Include(x => x.League).Include(x => x.Players).Where(x => x.GameTime > DateTime.UtcNow || (x.Players.Count() == 0) && !x.IsOfficialResult).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches).GroupBy(x => new DateTime(x.GameTime.Year, x.GameTime.Month, x.GameTime.Day));

            foreach (var match in matchesDto)
            {
                MatchesByGameTimeDto matchesByGameTime = new();
                matchesByGameTime.GameTime = match.Key;
                var groupKey = match.Key;
                foreach (var groupedMatch in match)
                {
                    var homeTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).HomeTeamId;
                    var awayTeamId = matches.FirstOrDefault(x => x.Id == groupedMatch.Id).AwayTeamId;
                    var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                    var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                    groupedMatch.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                    groupedMatch.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
                    matchesByGameTime.Matches.Add(groupedMatch);
                }
                result.Add(matchesByGameTime);
            }

            var expirationTime = DateTimeOffset.Now.AddDays(7);
            cacheData = result.OrderByDescending(x => x.GameTime);//await _dbContext.Products.ToListAsync();
            _memoryCache.Set("getFixturesOverall", cacheData, expirationTime);
            return cacheData;

            //return result;
        }

        // GET api/<MatchController>/5
        [HttpGet("{id}")]
        public MatchDto Get(int id)
        {
            var match = _context.Matches.Include(x => x.League).Include(x => x.Players).ThenInclude(x => x.Team).Include(x => x.Summaries).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == id);
            var matchDto = _mapper.Map<MatchDto>(match);

            var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
            matchDto.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
            matchDto.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            return matchDto;
        }

        // POST api/<MatchController>
        [HttpPost]
        public void Post([FromBody] MatchDto match)
        {
            var mappedMatch = _mapper.Map<Match>(match);

            //map players
            var players = _context.Players.Include(x => x.Team).Where(x => match.Players.Select(x => x.Id).Contains(x.Id)).ToList();
            mappedMatch.Players = players;

            //map summaries
            foreach (var summary in mappedMatch.Summaries)
                summary.Player = players.FirstOrDefault(x => x.Id == summary.Player.Id);

            mappedMatch.League = _context.Leagues.FirstOrDefault(x => x.Id == match.League.Id);

            var entry = _context.Matches.Add(mappedMatch);
            if (entry != null)
            {
                _context.SaveChanges();
            }
            RemoveCacheObjects();
        }

        // PUT api/<MatchController>/5
        [HttpPut("{id}")]
        public async void Put(int id, [FromBody] MatchDto match)
        {
            //var mappedMatch = _mapper.Map<Match>(match);
            var players = _context.Players.Include(x => x.Team).Where(x => match.Players.Select(x => x.Id).Contains(x.Id)).ToList();
            var allPlayers = _context.Players.Include(x => x.Team).ToList();

            var matchToBeUpdate = _context.Matches.Include(x => x.Players)
                                                   .Include(x => x.Summaries)
                                                   .ThenInclude(x => x.Player)
                                                   .FirstOrDefault(x => x.Id == match.Id);
            //matchToBeUpdate.Players.Clear();

            matchToBeUpdate.HomeTeamGoals = match.HomeTeamGoals;
            matchToBeUpdate.AwayTeamGoals = match.AwayTeamGoals;
            matchToBeUpdate.GamePlace = match.GamePlace;
            matchToBeUpdate.GameRound = match.GameRound;
            matchToBeUpdate.GameTime = match.GameTime;
            matchToBeUpdate.IsMatchSuspended = match.IsMatchSuspended;
            matchToBeUpdate.IsOfficialResult = match.IsOfficialResult;
            matchToBeUpdate.HomeTeamNegativePoints = match.HomeTeamNegativePoints;
            matchToBeUpdate.AwayTeamNegativePoints = match.AwayTeamNegativePoints;

            //league
            matchToBeUpdate.League = _context.Leagues.FirstOrDefault(x => x.Id == match.League.Id);

            //teams
            matchToBeUpdate.HomeTeamId = match.HomeTeam.Id;
            matchToBeUpdate.AwayTeamId = match.AwayTeam.Id;


            //players on match
            _context.Matches.Include(x => x.Players)
                            .FirstOrDefault(x => x.Id == match.Id)
                            .Players.Clear();

            foreach (var player in players)
            {
                if (match.Players.Select(x => x.Id).Contains(player.Id))
                {
                    matchToBeUpdate.Players.Add(player);
                }
            }

            _context.Matches.Include(x => x.Players)
                            .Include(x => x.Summaries)
                            .ThenInclude(x => x.Player)
                            .FirstOrDefault(x => x.Id == match.Id)
                            .Summaries.Clear();

            var summaries = _context.Summaries.Where(x => x.Match.Id == match.Id);
            foreach (var item in summaries)
            {
                _context.Summaries.Remove(item);
            }

            foreach (var summary in match.Summaries)
            {
                var sum = _mapper.Map<Summary>(summary);
                sum.Player = allPlayers.FirstOrDefault(x => x.Id == summary.Player.Id);
                matchToBeUpdate.Summaries.Add(sum);

            }

            var entry = _context.Matches.Update(matchToBeUpdate);
            if (entry != null)
            {
                _context.SaveChanges();
            }

            RemoveCacheObjects();
        }

        [HttpPost("getByTeam/{teamId}")]
        public IEnumerable<MatchDto> GetByTeam([FromBody] int teamId)
        {
            var matches = _context.Matches.Include(x => x.Players).Where(x => (x.HomeTeamId == teamId || x.AwayTeamId == teamId) && (x.Players.Count() > 0 || x.IsOfficialResult)).ToList();
            var matchesDto = _mapper.Map<List<MatchDto>>(matches);
            foreach (var match in matchesDto)
            {
                var homeTeamId = matches.FirstOrDefault(x => x.Id == match.Id).HomeTeamId;
                var awayTeamId = matches.FirstOrDefault(x => x.Id == match.Id).AwayTeamId;
                var homeTeam = _context.Teams.FirstOrDefault(x => x.Id == homeTeamId);
                var awayTeam = _context.Teams.FirstOrDefault(x => x.Id == awayTeamId);
                match.HomeTeam = _mapper.Map<TeamDto>(homeTeam);
                match.AwayTeam = _mapper.Map<TeamDto>(awayTeam);
            }
            return matchesDto.OrderByDescending(x => x.GameTime);
        }


        [HttpPost("delete")]
        public void Delete([FromBody] int matchId)
        {
            _context.Matches.FirstOrDefault(x => x.Id == matchId).IsDeleted = true;

            // delete also summary related to this match
            foreach (var summary in _context.Summaries.Include(x => x.Match).Where(x => x.Match.Id == matchId))
                summary.IsDeleted = true;

            _context.SaveChanges();

            RemoveCacheObjects();
        }

        private void RemoveCacheObjects()
        {
            _memoryCache.Remove("getByRoundOverall");
            _memoryCache.Remove("getDreamTeamOverall");
            _memoryCache.Remove("getplayersstatsoverall"); //playerstatsbyleague
            _memoryCache.Remove("get2ndDreamTeamOverall");
            _memoryCache.Remove("getByGameTimeOverall");
            _memoryCache.Remove("getFixturesOverall"); //
            _memoryCache.Remove("getTopGoalscorer");
            _memoryCache.Remove("getTopAssitent");
            var leagueIds = _context.Leagues.Where(x => !x.IsOverallLeague).Select(x => x.Id);
            foreach (var leagueId in leagueIds)
            {
                //
                _memoryCache.Remove($"playerstatsbyleague-{leagueId}");//
                _memoryCache.Remove($"getDreamTeam-{leagueId}");
                _memoryCache.Remove($"get2ndDreamTeam-{leagueId}"); //getByGameTime-{leagueId}
                _memoryCache.Remove($"getByGameTime-{leagueId}");
            }
        }
    }
}