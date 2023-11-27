using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Shared.Dto;
using LA_LIGA_REKREATIVO.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public MatchController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<MatchController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MatchController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

            var entry = _context.Matches.Add(mappedMatch);
            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<MatchController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MatchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
