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
    public class EventsLogController : ControllerBase
    {
        private readonly ILogger<EventsLogController> _logger;
        private IEventLogService _eventLogService;
        private MainDBContext _mainDBContext;
        private IZoneTimeService _zoneTimeService;

        public EventsLogController(IZoneTimeService zoneTimeService,
            MainDBContext mainDBContext,
            ILogger<EventsLogController> logger,
            IEventLogService eventLogService)

        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _eventLogService = eventLogService;
            _zoneTimeService = zoneTimeService;
        }

        [HttpGet]
        public async Task<ActionResult> GetDDLList()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                EventlogDDlViewModel result = await _eventLogService.GetDDLList(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetBlockMatrix) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        [HttpGet("DDLByNetworkId/{networkId}")]
        public async Task<ActionResult> GetDDLList(int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                EventlogDDlViewModel result = await _eventLogService.GetDDLList(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetBlockMatrix) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        [HttpGet("GetValevents/{zoneId}/{networkId}/{blockId}/{rtuId}")]
        public async Task<ActionResult> GetBlockMatrix(int zoneId, int networkId, int blockId, int rtuId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                var result = await _eventLogService.GetValEvents(zoneId, networkId, blockId, rtuId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(DashboardController) }.{ nameof(GetBlockMatrix) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }
    }
}
