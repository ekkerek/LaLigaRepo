using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Helpers;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.EntityFrameworkCore;

namespace LA_LIGA_REKREATIVO.Server.Services;
public class TeamStatsService : ITeamStatsService
{
    private readonly LaLigaContext _context;
    private readonly IMapper _mapper;

    public TeamStatsService(LaLigaContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public List<TeamStatsDto> GetCommonStanding()
    {
        var matches = _context.Matches.Include(x => x.League).Where(x => (x.Players.Count() > 0 || x.IsOfficialResult) && !x.League.IsPlayOff).ToList();
        var teams = _context.Teams.Include(x => x.Leagues);
        var leagueCoefficients = _context.Leagues.Select(x => x.Coefficient).Distinct().ToList();
        var teamStatsList = new List<TeamStatsDto>();
        foreach (var team in teams)
        {
            var teamStatsDto = new TeamStatsDto();
            teamStatsDto.Team = _mapper.Map<TeamDto>(team);
            teamStatsDto.Matches = _mapper.Map<List<MatchDto>>(matches.Where(x => x.HomeTeamId == team.Id || x.AwayTeamId == team.Id));

            teamStatsDto.GamePlayed = CalculateTeamGamePlayed(team.Id, matches);
            teamStatsDto.Goals = CalculateTeamGoals(team.Id, matches);
            teamStatsDto.GoalsConceded = CalculateTeamGoalsConceded(team.Id, matches);
            teamStatsDto.GoalDifference = teamStatsDto.Goals - teamStatsDto.GoalsConceded;
            teamStatsDto.Wins = CalculateTeamWins(team.Id, matches);
            teamStatsDto.Losts = CalculateTeamLosts(team.Id, matches);
            teamStatsDto.Draws = CalculateTeamLosts(team.Id, matches);
            teamStatsDto.TotalPoints = GetPointsByTeam(team.Id, matches);
            teamStatsDto.PointsByCoefficient = GetCoefficientPointsByTeam(team.Id, matches, leagueCoefficients);

            teamStatsList.Add(teamStatsDto);
        }
        return teamStatsList.OrderByDescending(x => x.PointsByCoefficient)
                            .ThenBy(x => x, new StandingsComparer())
                            .ThenByDescending(x => x.GoalDifference)
                            .ThenByDescending(x => x.Goals)
                            .ToList();
    }

    public List<TeamStatsDto> GetStandingsByLeague(int id)
    {
        var matches = _context.Matches.Include(x => x.League).Where(x => x.League.Id == id && (x.Players.Count() > 0 || x.IsOfficialResult)).ToList();
        var teams = _context.Leagues.Include(x => x.Teams).FirstOrDefault(x => x.Id == id).Teams;
        var teamStatsList = new List<TeamStatsDto>();
        foreach (var team in teams)
        {
            var teamStatsDto = new TeamStatsDto();
            teamStatsDto.Team = _mapper.Map<TeamDto>(team);
            teamStatsDto.Matches = _mapper.Map<List<MatchDto>>(matches.Where(x => x.HomeTeamId == team.Id || x.AwayTeamId == team.Id));
            teamStatsDto.GamePlayed = CalculateTeamGamePlayed(team.Id, matches);
            teamStatsDto.Goals = CalculateTeamGoals(team.Id, matches);
            teamStatsDto.GoalsConceded = CalculateTeamGoalsConceded(team.Id, matches);
            teamStatsDto.GoalDifference = teamStatsDto.Goals - teamStatsDto.GoalsConceded;
            teamStatsDto.Wins = CalculateTeamWins(team.Id, matches);
            teamStatsDto.Losts = CalculateTeamLosts(team.Id, matches);
            teamStatsDto.Draws = CalculateTeamLosts(team.Id, matches);
            teamStatsDto.TotalPoints = GetPointsByTeam(team.Id, matches);

            var leagueCoefficient = _context.Leagues.Where(x => x.Id == id).Select(x => x.Coefficient).Distinct().ToList();
            teamStatsDto.PointsByCoefficient = GetCoefficientPointsByTeam(team.Id, matches, leagueCoefficient);

            teamStatsList.Add(teamStatsDto);
        }
        return teamStatsList.OrderByDescending(x => x.PointsByCoefficient)
                            .ThenByDescending(x => x.GoalDifference)
                            .ThenByDescending(x => x.Goals)
                            .ToList();
    }

    private int GetPointsByTeam(int teamId, List<Match> matches)
    {
        int totalPoints = 0;

        var wins = CalculateTeamWins(teamId, matches);
        var draws = CalculateTeamDraws(teamId, matches);

        var regularPoints = wins * 3 + draws;

        totalPoints += regularPoints;

        //negative points
        var negativePoints = matches.Where(x => x.HomeTeamId == teamId && x.IsMatchSuspended && x.HomeTeamNegativePoints.HasValue).Sum(x => x.HomeTeamNegativePoints) +
                             matches.Where(x => x.AwayTeamId == teamId && x.IsMatchSuspended && x.AwayTeamNegativePoints.HasValue).Sum(x => x.AwayTeamNegativePoints);

        totalPoints -= negativePoints ?? 0;

        return totalPoints;
    }

    private decimal GetCoefficientPointsByTeam(int teamId, List<Match> matches, List<decimal> leagueCoefficients)
    {
        decimal pointsByCoefficient = 0;

        foreach (var coefficient in leagueCoefficients)
        {
            var wins = matches.Count(x => x.HomeTeamId == teamId &&
                                          x.HomeTeamGoals > x.AwayTeamGoals &&
                                          x.League.Coefficient.Equals(coefficient)) +
                       matches.Count(x => x.AwayTeamId == teamId &&
                                          x.HomeTeamGoals < x.AwayTeamGoals &&
                                          x.League.Coefficient.Equals(coefficient));

            var draws = matches.Count(x => x.HomeTeamId == teamId &&
                                           x.HomeTeamGoals == x.AwayTeamGoals &&
                                           x.League.Coefficient.Equals(coefficient)) +
                        matches.Count(x => x.AwayTeamId == teamId &&
                                           x.HomeTeamGoals == x.AwayTeamGoals &&
                                           x.League.Coefficient.Equals(coefficient));

            var pointByCoefficinet = wins * 3 * coefficient + draws * coefficient;

            pointsByCoefficient += pointByCoefficinet;

            //negative points
            var negativePoints = matches.Where(x => x.HomeTeamId == teamId && x.IsMatchSuspended && x.HomeTeamNegativePoints.HasValue && x.League.Coefficient.Equals(coefficient)).Sum(x => x.HomeTeamNegativePoints) +
                                matches.Where(x => x.AwayTeamId == teamId && x.IsMatchSuspended && x.AwayTeamNegativePoints.HasValue && x.League.Coefficient.Equals(coefficient)).Sum(x => x.AwayTeamNegativePoints);

            pointsByCoefficient -= negativePoints * coefficient ?? 0;
        }
        return pointsByCoefficient;
    }

    private int CalculateTeamGamePlayed(int teamId, List<Match> matches)
    {
        return matches.Count(x => x.HomeTeamId == teamId || x.AwayTeamId == teamId);
    }

    private int CalculateTeamGoals(int teamId, List<Match> matches)
    {
        return matches.Where(x => x.HomeTeamId == teamId).Select(x => x.HomeTeamGoals).Sum() +
               matches.Where(x => x.AwayTeamId == teamId).Select(x => x.AwayTeamGoals).Sum();
    }

    private int CalculateTeamGoalsConceded(int teamId, List<Match> matches)
    {
        return matches.Where(x => x.HomeTeamId == teamId).Select(x => x.AwayTeamGoals).Sum() +
               matches.Where(x => x.AwayTeamId == teamId).Select(x => x.HomeTeamGoals).Sum();
    }

    private int CalculateTeamWins(int teamId, List<Match> matches)
    {
        return matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals > x.AwayTeamGoals) +
               matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals < x.AwayTeamGoals);
    }

    private int CalculateTeamLosts(int teamId, List<Match> matches)
    {
        return matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals < x.AwayTeamGoals) +
               matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals > x.AwayTeamGoals);
    }

    private int CalculateTeamDraws(int teamId, List<Match> matches)
    {
        return matches.Count(x => x.HomeTeamId == teamId && x.HomeTeamGoals == x.AwayTeamGoals) +
               matches.Count(x => x.AwayTeamId == teamId && x.HomeTeamGoals == x.AwayTeamGoals);
    }

    public List<TeamStatsHistoryDto> GetTeamStatsForNonActiveLeagues(int teamId)
    {
        var team = _context.Teams.FirstOrDefault(t => t.Id == teamId);
        if (team == null) return new List<TeamStatsHistoryDto>();

        var matches = _context.Matches
            .Include(m => m.League)
            .Where(m => !m.League.IsActive && (m.HomeTeamId == teamId || m.AwayTeamId == teamId))
            .ToList();

        if (!matches.Any()) return new List<TeamStatsHistoryDto>();

        var groupedByYear = matches.GroupBy(m => m.League.Year).Select(g =>
        {
            var yearMatches = g.ToList();

            var wins = CalculateTeamWins(teamId, yearMatches);

            var draws = CalculateTeamDraws(teamId, yearMatches);// yearMatches.Count(m => m.HomeTeamGoals == m.AwayTeamGoals);
            var losts = CalculateTeamLosts(teamId, yearMatches);// yearMatches.Count - wins - draws;

            var goals = yearMatches.Sum(m =>
                m.HomeTeamId == teamId ? m.HomeTeamGoals : m.AwayTeamId == teamId ? m.AwayTeamGoals : 0);

            var goalsConceded = yearMatches.Sum(m =>
                m.HomeTeamId == teamId ? m.AwayTeamGoals : m.AwayTeamId == teamId ? m.HomeTeamGoals : 0);


            var playoffMatches = yearMatches.Where(m => m.IsPlayOff).ToList();

            string? playoffRoundFinished;

            if (!playoffMatches.Any())
            {
                playoffRoundFinished = "Ispali u grupnoj fazi";
            }
            else
            {
                var finalMatch = playoffMatches
                    .Where(m => m.PlayOffRound == PlayOffRound.Final)
                    .FirstOrDefault(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId);

                if (finalMatch != null)
                {
                    var teamWon = (finalMatch.HomeTeamId == teamId && finalMatch.HomeTeamGoals > finalMatch.AwayTeamGoals) ||
                                  (finalMatch.AwayTeamId == teamId && finalMatch.AwayTeamGoals > finalMatch.HomeTeamGoals);

                    playoffRoundFinished = teamWon ? "1. mjesto" : "2. mjesto";
                }
                else
                {
                    var lastMatch = playoffMatches
                        .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                        .OrderByDescending(m => m.PlayOffRound)
                        .FirstOrDefault();

                    playoffRoundFinished = lastMatch != null
                        ? $"{lastMatch.PlayOffRound.GetDisplayName()}"
                        : "Ispali u ranijoj eliminacionoj fazi";
                }
            }


            return new TeamStatsHistoryDto
            {
                Year = g.Key,
                GamePlayed = yearMatches.Count,
                Wins = wins,
                Draws = draws,
                Losts = losts,
                Goals = goals,
                GoalsConceded = goalsConceded,
                PlayoffRoundFinished = playoffRoundFinished,
                Team = _mapper.Map<TeamDto>(team)
            };
        }).ToList();

        return groupedByYear;
    }

}


