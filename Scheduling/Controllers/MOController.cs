using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MOController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MOController> _logger;
        private MainDBContext _mainDBContext;
        private IManualOverrideService _manualOverrideService;
        public MOController(
            MainDBContext mainDBContext, UserManager<ApplicationUser> userManager,
            ILogger<MOController> logger,
            IManualOverrideService manualOverrideService)
        {
            _logger = logger; _userManager = userManager;
            _mainDBContext = mainDBContext;
            _manualOverrideService = manualOverrideService;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<MOGetDataModel> data = await _manualOverrideService.GetMo(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                ManualOverrideMasterViewModel data = await _manualOverrideService.GetMoById(id, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {

                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }

        }

        /// <summary>
        /// GetOutputValves
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <param name="blockId"></param>
        /// <param name="overridefor"></param>
        /// <returns></returns>
        [HttpGet("GetOutputValves/{networkId}/{zoneId}/{blockId}/{overridefor}")]
        public async Task<ActionResult> GetOutputValves(int networkId, int zoneId, int blockId, string overridefor)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<ElementNo> data = await _manualOverrideService.GetOutputValves(networkId, zoneId, blockId, overridefor, priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {

                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        /// <summary>
        /// GetValveScheduleForElements
        /// </summary>
        /// <param name="strElements"></param>
        /// <param name="overridefor"></param>
        /// <returns></returns>
        [HttpGet("GetValveScheduleForElements/{moId}/{strElements}/{overridefor}")]
        public async Task<ActionResult> GetValveScheduleForElements(int moId, string strElements, string overridefor)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                List<ValveScheduleForMO> data = await _manualOverrideService.GetValveScheduleForElements(moId,strElements, overridefor);
                return Ok(CustomResponse.CreateResponse(true, "", data, 0));
            }
            catch (Exception ex)
            {

                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }

        }

        // GET api/<controller>/5
        [HttpGet("GetMoLstData")]
        public async Task<ActionResult> GetMoLstData()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                MOMetaDataViewModel mOMetaDataViewModel = await _manualOverrideService.GetMOMetaData(priviledges, userId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, mOMetaDataViewModel, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(MOController) }.{ nameof(GetMoLstData) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ManualOverrideMasterViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var priviledges = User.Claims.Single(c => c.Type == CustomClaimTypes.Permission).Value;
                var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
                //ClaimsPrincipal currentUser = this.User;
                //var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                //ApplicationUser user = await _userManager.FindByNameAsync(currentUserName); 
                bool saved = await _manualOverrideService.AddUpdate(model, userId);
                return Ok(CustomResponse.CreateResponse(true, "", saved, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(MOController) }.{ nameof(Post) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]ManualOverrideMasterViewModel value)
        //{
        //}

        // DELETE api/<controller>/5
        [HttpPost("DeleteMos")]
        public async Task<IActionResult> DeleteMos([FromBody] List<int> ids)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                bool result = await _manualOverrideService.DeleteSequenceBySeqId(ids);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, result, 0)); ;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(MOController) }.{ nameof(GetMoLstData) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
            }
        }
    }
}
