using AutoMapper;
using LA_LIGA_REKREATIVO.Server.Data;
using LA_LIGA_REKREATIVO.Server.Models;
using LA_LIGA_REKREATIVO.Server.Services;
using LA_LIGA_REKREATIVO.Shared;
using LA_LIGA_REKREATIVO.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LA_LIGA_REKREATIVO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly LaLigaContext _context;
        private readonly IMapper _mapper;
        private readonly IHostEnvironment _env;
        private readonly IUploadService _uploadService;

        public LeagueController(LaLigaContext context, IMapper mapper, IHostEnvironment env, IUploadService uploadService)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
            _uploadService = uploadService;
        }

        // GET: api/<LeagueController>
        [HttpGet]
        public IEnumerable<LeagueDto> Get()
        {
            var leagues = _context.Leagues.Include(x => x.Teams);
            return _mapper.Map<List<LeagueDto>>(leagues);
        }

        // GET api/<LeagueController>/5
        [HttpGet("{id}")]
        public LeagueDto Get(int id)
        {
            var league = _context.Leagues.Include(x => x.Teams).FirstOrDefault(x => x.Id == id);
            return _mapper.Map<LeagueDto>(league);
        }

        // POST api/<LeagueController>
        [HttpPost]
        public void Post([FromBody] LeagueDto league)
        {
            _context.Leagues.Add(_mapper.Map<League>(league));
            _context.SaveChanges();
        }

        // PUT api/<LeagueController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] LeagueDto league)
        {
            var updatedLeague = _context.Leagues.FirstOrDefault(x => x.Id == id);
            updatedLeague.Name = league.Name;
            updatedLeague.Year = league.Year;
            _context.Leagues.Update(updatedLeague);
            _context.SaveChanges();
        }

        // DELETE api/<LeagueController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //[HttpPost("addTeams")]
        //public void AddTeams([FromBody] LeagueDto league)
        //{
        //    var l = _context.Leagues.FirstOrDefault(x => x.Id == league.Id);
        //    //var teams = _mapper.Map<List<Team>>(league.Teams);
        //    var tttteams = _context.Teams.Where(x => league.Teams.Select(x => x.Id).Contains(x.Id)).ToList();

        //    foreach (var t in tttteams)
        //    {
        //        l.Teams.Add(t);
        //    }
        //    _context.SaveChanges();
        //}


        [HttpPost("postFile")]
        public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files)
        {
            var tt = await _uploadService.PostFile(files);
            //var maxAllowedFiles = 3;
            //long maxFileSize = 1024 * 1024;
            //var filesProcessed = 0;
            var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
            //List<UploadResult> uploadResults = new();

            //foreach (var file in files)
            //{
            //    var uploadResult = new UploadResult();
            //    string trustedFileNameForFileStorage;
            //    var untrustedFileName = file.FileName;
            //    uploadResult.FileName = untrustedFileName;
            //    var trustedFileNameForDisplay =
            //        WebUtility.HtmlEncode(untrustedFileName);

            //    if (filesProcessed < maxAllowedFiles)
            //    {
            //        if (file.Length == 0)
            //        {
            //            //logger.LogInformation("{FileName} length is 0 (Err: 1)",
            //            //    trustedFileNameForDisplay);
            //            uploadResult.ErrorCode = 1;
            //        }
            //        else if (file.Length > maxFileSize)
            //        {
            //            //logger.LogInformation("{FileName} of {Length} bytes is " +
            //            //    "larger than the limit of {Limit} bytes (Err: 2)",
            //            //    trustedFileNameForDisplay, file.Length, maxFileSize);
            //            uploadResult.ErrorCode = 2;
            //        }
            //        else
            //        {
            //            try
            //            {
            //                var fileNames = Directory.GetFiles("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot");
            //                var fnames = new List<string>();
            //                foreach (string f in fileNames)
            //                    fnames.Add(Path.GetFileName(f));

            //                trustedFileNameForFileStorage = Path.GetRandomFileName();
            //                var path = Path.Combine("C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot",
            //              file.FileName);
            //                //var path = "C:\\Users\\uic80946\\source\\repos\\LaLigaRepo\\LA_LIGA_REKREATIVO\\Client\\wwwroot";

            //                await using FileStream fs = new(path, FileMode.Create);
            //                await file.CopyToAsync(fs);

            //                //logger.LogInformation("{FileName} saved at {Path}",
            //                //    trustedFileNameForDisplay, path);
            //                uploadResult.Uploaded = true;
            //                uploadResult.StoredFileName = trustedFileNameForFileStorage;
            //            }
            //            catch (IOException ex)
            //            {
            //                //logger.LogError("{FileName} error on upload (Err: 3): {Message}",
            //                //    trustedFileNameForDisplay, ex.Message);
            //                uploadResult.ErrorCode = 3;
            //            }
            //        }

            //        filesProcessed++;
            //    }
            //    else
            //    {
            //        //logger.LogInformation("{FileName} not uploaded because the " +
            //        //    "request exceeded the allowed {Count} of files (Err: 4)",
            //        //    trustedFileNameForDisplay, maxAllowedFiles);
            //        uploadResult.ErrorCode = 4;
            //    }

            //    uploadResults.Add(uploadResult);
            //}

            return new CreatedResult(resourcePath, tt);
        }
    }
}
