using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ILogger<DashboardController> _logger;
        private MainDBContext _mainDBContext;
        private IZoneTimeService _zoneTimeService;
        private IProjectService _projectService;
        private IDashboardService _dashboardService;

        public DashboardController(IZoneTimeService zoneTimeService,
            MainDBContext mainDBContext, ILogger<DashboardController> logger,
            ISequenceService sequenceService,
            IProjectService projectService,
            IDashboardService dashboardService)
        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _zoneTimeService = zoneTimeService;
            _projectService = projectService;
            _dashboardService = dashboardService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetZoneBlocksSubBlock")]
        public async Task<ActionResult> GetZoneBlocksSubBlock()
        {
            try
            {
                string result = "Success";
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                var resulta = await _dashboardService.GetZoneBlockSubblock(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, resulta, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetZoneBlocksSubBlock) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        [HttpGet("GetZoneMatrix/{zoneId}")]

        public async Task<ActionResult> GetZoneMatrix(int zoneId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                var result = await _dashboardService.GetZoneMatrix(priviledges, userId, zoneId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetZoneBlocksSubBlock) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }

        }

        [HttpGet("GetBlockMatrix/{zoneId}/{blockId}")]
        public async Task<ActionResult> GetBlockMatrix(int zoneId, int blockId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                DashboardBlockModelList result = await _dashboardService.GetBlockMatrix(priviledges, userId, blockId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetZoneBlocksSubBlock) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }
    }
}
