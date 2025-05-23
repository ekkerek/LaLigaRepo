﻿using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;

        public LeagueController(LaLigaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/<LeagueController>
        [HttpGet]
        public IEnumerable<LeagueDto> Get()
        {
            var leagues = _context.Leagues.Include(x => x.Teams).Where(x => x.IsActive);
            return _mapper.Map<List<LeagueDto>>(leagues).OrderByDescending(x => x?.IsOverallLeague).ThenByDescending(x => x.Id);
        }

        [HttpGet("getAll")]
        public IEnumerable<LeagueDto> GetAll()
        {
            return _mapper.Map<List<LeagueDto>>(_context.Leagues.Include(x => x.Teams));
        }

        // GET api/<LeagueController>/5
        [HttpGet("{id}")]
        public LeagueDto Get(int id)
        {
            var league = _context.Leagues.Include(x => x.Teams).FirstOrDefault(x => x.Id == id);
            return _mapper.Map<LeagueDto>(league);
        }

        //getNonPlayoffLeague
        [HttpGet("getNonPlayoffLeague")]
        public IEnumerable<LeagueDto> GetNonPlayoffLeague()
        {
            var leagues = _context.Leagues.Include(x => x.Teams);
            return _mapper.Map<List<LeagueDto>>(leagues).Where(x => x.IsActive && !x.IsPlayOff).OrderByDescending(x => x?.IsOverallLeague).ThenByDescending(x => x.Id);
        }

        [HttpGet("getLeagueRounds/{leagueId}")]
        public IEnumerable<int> GetLeagueRounds(int leagueId)
        {
            return _context.Matches.Include(x => x.League)
                                                .Where(x => x.League.Id == leagueId)
                                                .Select(m => m.GameRound)
                                                .Distinct()
                                                .OrderByDescending(r => r)
                                                .ToList();
        }

        // POST api/<LeagueController>
        [HttpPost]
        [Authorize]
        public void Post([FromBody] LeagueDto league)
        {
            _context.Leagues.Add(_mapper.Map<League>(league));
            _context.SaveChanges();
        }

        // PUT api/<LeagueController>/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody] LeagueDto league)
        {
            var updatedLeague = _context.Leagues.FirstOrDefault(x => x.Id == id);
            updatedLeague.Name = league.Name;
            updatedLeague.Year = league.Year;
            updatedLeague.Coefficient = league.Coefficient;
            updatedLeague.IsOverallLeague = league.IsOverallLeague;
            updatedLeague.IsActive = league.IsActive;
            _context.Leagues.Update(updatedLeague);
            _context.SaveChanges();
        }

        [HttpPost("delete")]
        [Authorize]
        public void Delete([FromBody] LeagueDto league)
        {
            _context.Leagues.FirstOrDefault(x => x.Id == league.Id).IsDeleted = true;
            _context.SaveChanges();
        }
    }
}
