using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
    public class NetworkController : ControllerBase
    {
        private readonly ILogger<NetworkController> _logger;
        private readonly IMapper _mapper;
        private INetworkService _networkService;
        private MainDBContext _mainDBContext;

        public NetworkController(
            MainDBContext mainDBContext,
            ILogger<NetworkController> logger,
            INetworkService network)
        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _networkService = network;
        }

        [HttpGet("GetShortNetworks")]
        public async Task<ActionResult> GetShortNetworks(int zoneId)
        {
            try
            {
                //TODO: PA - Check for user who dont have network access, now sending null resoponse
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<NetworkShortViewModel> models = await _networkService.GetShortNetworks(zoneId, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetShortNetworks) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        [HttpGet]
        public async Task<List<Network>> GetNetworks(int zoneId)
        {
            try
            {
                //TODO: PA - Check for user who dont have network access, now sending null resoponse
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<Network> models = await _networkService.GetNetworks();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetShortNetworks) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        [HttpGet("GetAvailableNetworks")]
        public async Task<List<AvailableNetworkObject>> GetAvailableNetworks(int zoneId)
        {
            try
            {
                //TODO: PA - Check for user who dont have network access, now sending null resoponse
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<AvailableNetworkObject> models = await _networkService.GetAvailableNetworks();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetAvailableNetworks) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }


    }
}
