using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;

namespace LA_LIGA_REKREATIVO.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();
            CreateMap<Player, PlayerDto>();
            CreateMap<PlayerDto, Player>();
            CreateMap<Summary, SummaryDto>();
            CreateMap<SummaryDto, Summary>();
            CreateMap<Match, MatchDto>().ForMember(sum => sum.Summaries,
                map => map.MapFrom(source =>
                source.Summaries.ToList()));
            CreateMap<MatchDto, Match>().ForMember(sum => sum.Summaries,
                map => map.MapFrom(source =>
                source.Summaries.ToList()));
            CreateMap<Match, MatchByTeamDto>().ForMember(sum => sum.Summaries,
               map => map.MapFrom(source =>
               source.Summaries.ToList()));
            CreateMap<MatchByTeamDto, Match>().ForMember(sum => sum.Summaries,
                map => map.MapFrom(source =>
                source.Summaries.ToList()));
            CreateMap<League, LeagueDto>();
            CreateMap<LeagueDto, League>();
        }
    }
}
