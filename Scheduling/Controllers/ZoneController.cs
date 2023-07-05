using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ZoneController : ControllerBase
    {
        private readonly ILogger<ZoneController> _logger;
        private readonly IMapper _mapper;
        private IZoneService _zoneService;
        private MainDBContext _mainDBContext;

        public ZoneController(
            MainDBContext mainDBContext,
            ILogger<ZoneController> logger,
            IZoneService zone)
        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _zoneService = zone;
        }

        [HttpGet("GetShortZones")]
        public async Task<ActionResult> GetShortZones()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<ZoneShortViewModel> models = await _zoneService.GetShortZones();
                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetShortZones) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }
    }
}
