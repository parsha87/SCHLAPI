using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilterController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FilterController> _logger;
        private MainDBContext _mainDBContext;
        private IFilterService _filterService;

        public FilterController(
           MainDBContext mainDBContext, UserManager<ApplicationUser> userManager,
           ILogger<FilterController> logger, IFilterService filterService
        )
        {
            _logger = logger;
            _userManager = userManager;
            _mainDBContext = mainDBContext;
            _filterService = filterService;
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FilterValveGroupConfig model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                bool saved = await _filterService.AddUpdate(model, userId);
                return Ok(CustomResponse.CreateResponse(true, "", saved, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(FilterController) }.{ nameof(Post) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult> Get(int zoneId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<GroupDetailsViewModel> data = await _filterService.GetGroupDetailsByZoneId(zoneId, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(FilterController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        [HttpGet("GetGroupById/{id}")]
        public async Task<ActionResult> GetGroupById(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                FilterValveGroupConfigViewModel data = await _filterService.GetGroupDetailsById(id, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(FilterController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        [HttpGet("CreateGroup/{zoneId}")]
        public async Task<ActionResult> CreateGroup(int zoneId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                string data = await _filterService.CreateGroup(zoneId, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(FilterController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }


        [HttpGet("GetZoneGroups")]
        public async Task<ActionResult> GetZoneGroups()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<FilterZone> data = await _filterService.GetMetaData(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }
    }
}