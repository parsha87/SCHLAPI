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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private IGroupService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService service,
            IMapper mapper,
            ILogger<GroupController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get Group Master 
        /// </summary>
        /// <returns>List<MultiGroupMaster></returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiGroupMaster> list = await _service.GetGroupMaster();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(Get) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));

            }
        }


        /// <summary>
        /// Get Group Master 
        /// </summary>
        /// <returns>List<MultiGroupMaster></returns>
        [HttpGet("GetById")]
        public async Task<ActionResult> GetById(int Id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                GroupDetailsViewModel list = await _service.GetById(Id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(GetById) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));

            }
        }
        /// <summary>
        /// Get Group Master 
        /// </summary>
        /// <returns>List<MultiGroupMaster></returns>
        [HttpDelete]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                bool list = await _service.Delete(Id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(Delete) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Add Group Master 
        /// </summary>
        /// <returns>List<MultiGroupMaster></returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GroupDetailsViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (!_service.IfGroupExists(model.MultiGroupMaster.GroupName))
                {
                    GroupDetailsViewModel list = await _service.AddGroup(model);
                    return Ok(list);
                }
                else
                {
                    return BadRequest("Group Name Already Exists.");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(Get) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Add Group Master 
        /// </summary>
        /// <returns>List<MultiGroupMaster></returns>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] GroupDetailsViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                GroupDetailsViewModel list = await _service.EditGroup(model);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(Put) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Get Sensors for Group 
        /// </summary>
        /// <returns>List<MultiGroupData></returns>
        [HttpGet("GetSensorsForGroup")]
        public async Task<ActionResult> GetSensorsForGroup()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiGroupData> list = await _service.GetSensorsForGroup();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(GroupController) + "." + nameof(GetSensorsForGroup) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

    }
}
