using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;

namespace LA_LIGA_REKREATIVO.Server.Helpers
{

    public class StandingsComparer : IComparer<TeamStatsDto>
    {
        public int Compare(TeamStatsDto team1, TeamStatsDto team2)
        {
            if (team1.Team.Leagues.FirstOrDefault().Coefficient > team2.Team.Leagues.FirstOrDefault(x => x.IsActive).Coefficient)
                return -1;
            else if (team1.Team.Leagues.FirstOrDefault().Coefficient < team2.Team.Leagues.FirstOrDefault(x => x.IsActive).Coefficient)
                return 1;
            else return 0;

            //var matchesBetween = team1.Matches.Where(x => x.HomeTeamId == team2.Team.Id || x.AwayTeamId == team2.Team.Id);
            //int team1Points = 0;
            //int team2Points = 0;
            //foreach (var item in matchesBetween)
            //{
            //    if (item.HomeTeamId == team1.Team.Id)
            //    {
            //        if (item.HomeTeamGoals > item.AwayTeamGoals)
            //        {
            //            team1Points += 3;
            //        }
            //        else if (item.HomeTeamGoals < item.AwayTeamGoals)
            //        {
            //            team2Points += 3;
            //        }
            //        else
            //        {
            //            team1Points += 1;
            //            team2Points += 1;
            //        }
            //    }
            //    else if (item.AwayTeamId == team1.Team.Id)
            //    {
            //        if (item.HomeTeamGoals < item.AwayTeamGoals)
            //        {
            //            team1Points += 3;
            //        }
            //        else if (item.HomeTeamGoals > item.AwayTeamGoals)
            //        {
            //            team2Points += 3;
            //        }
            //        else
            //        {
            //            team1Points += 1;
            //            team2Points += 1;
            //        }
            //    }
            //}

            //if (team1Points > team2Points)
            //    return -1;
            //else if (team1Points < team2Points)
            //    return 1;
            //else
            //    return 0;
        }
    }
}