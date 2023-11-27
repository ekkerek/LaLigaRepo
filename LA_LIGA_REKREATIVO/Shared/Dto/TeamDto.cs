using LA_LIGA_REKREATIVO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LA_LIGA_REKREATIVO.Shared.Dto
{
    public class TeamDto
    {
        public TeamDto()
        {
            Players = new List<PlayerDto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParticipantOf { get; set; }
        public string? LogoSrc { get; set; }

        public List<PlayerDto> Players { get; set; }
    }
}
