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
            CreateMap<Match, MatchDto>();
            CreateMap<MatchDto, Match>();
            CreateMap<League, LeagueDto>();
            CreateMap<LeagueDto, League>();
        }
    }
}
