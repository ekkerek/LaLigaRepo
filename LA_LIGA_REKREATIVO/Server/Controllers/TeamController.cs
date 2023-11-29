using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        public TeamController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<TeamController>
        [HttpGet]
        public IEnumerable<Team> Get()
        {
            return _context.Teams.ToList();
        }

        // GET api/<TeamController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [AllowAnonymous]
        [HttpPost("addNew")]
        public void AddNew([FromBody] TeamDto teamDto)
        {
            Team team = _mapper.Map<Team>(teamDto);
            var entry = _context.Teams.Add(team);
            if (entry != null)
            {
                _context.SaveChanges();
            }
        }

        // PUT api/<TeamController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TeamController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
