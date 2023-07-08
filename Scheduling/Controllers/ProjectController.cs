using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Scheduling.Services;
using Scheduling.Data.Entities;
using Scheduling.Data;
using System.Text;
using System.Security.Claims;
using Scheduling.ViewModels;
using Scheduling.Auth;
using Scheduling.Data.GlobalEntities;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ProjectController> _logger;
        private readonly IMapper _mapper;
        private IProjectService _projectService;
        private MainDBContext _mainDBContext;
        private IZoneTimeService _zoneTimeService;
        private GlobalDBContext _globalDBContext;

        public ProjectController(IZoneTimeService zoneTimeService,
            IConfiguration config,
            MainDBContext mainDBContext,
            ILogger<ProjectController> logger, GlobalDBContext globalDBContext,
            IProjectService projectService)
        {
            _logger = logger;
            _config = config;
            _mainDBContext = mainDBContext;
            _projectService = projectService;
            _zoneTimeService = zoneTimeService;
            _globalDBContext = globalDBContext;
        }


        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpPost("SaveValveSensorLatLong")]
        public async Task<ActionResult> SaveValveSensorLatLong([FromBody] MultiLatLongValveSensor multiLatLongValveSensor)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                var result = await _projectService.SaveValveSensorLatLong(multiLatLongValveSensor);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(SaveValveSensorLatLong)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpPost("SaveNodeLatLong")]
        public async Task<ActionResult> SaveNodeLatLong([FromBody] MultiNodeLatLong multiLatLongValveSensor)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                var result = await _projectService.SaveNodeLatLong(multiLatLongValveSensor);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(SaveNodeLatLong)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProjectConfiguration>>> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                List<ProjectConfiguration> projectList = await _projectService.GetProjectList();
                return projectList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(Get)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet("GetMultiNetworkRtu")]
        public async Task<ActionResult<List<MultiNetworkRtu>>> GetMultiNetworkRtu()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                List<MultiNetworkRtu> proList = await _projectService.GetMultiNetworkRtu();
                return proList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetMultiNetworkRtu)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet("GetVersion")]
        public async Task<ActionResult<MultiUiversion>> GetVersion()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                MultiUiversion proList = await _projectService.GetMultiUiversion();
                return proList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetVersion)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet("GetGateways")]
        public async Task<ActionResult<List<Gateway>>> GetGateways()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                List<Gateway> proList = await _projectService.GetGateways();
                return proList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetMultiNetworkRtu)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet("GetNodeLatLong")]
        public async Task<ActionResult<List<MultiNodeDashbordData>>> GetMultiNodeLatLong()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                List<MultiNodeDashbordData> proList = await _projectService.GetMultiNodeLatLong();
                return proList;
            } 
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetMultiNetworkRtu)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpPut("UpdateGateway/{id}")]
        public async Task<ActionResult> UpdateGateway([FromBody] Gateway model, int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (_globalDBContext.ProjectGatewayMapping.Any(x => x.GatewaySrNo == (int)model.SerialNo && x.ProjectName != DbManager.SiteName))
                {
                    return BadRequest();
                }
                List<ProjectGatewayMapping> projectGatewayMappinga = _globalDBContext.ProjectGatewayMapping.Where(x => x.ProjectName == DbManager.SiteName).ToList();
                if (projectGatewayMappinga != null)
                {
                    if (!projectGatewayMappinga.Any(x => x.GatewaySrNo == model.SerialNo))
                    {
                        ProjectGatewayMapping projectGatewayMapping = new ProjectGatewayMapping();
                        projectGatewayMapping.ProjectId = _mainDBContext.Project.FirstOrDefault().PrjId;
                        projectGatewayMapping.ProjectName = DbManager.SiteName;
                        projectGatewayMapping.GatewaySrNo = model.SerialNo;
                        projectGatewayMapping.GatewayNo = model.GatewayNo;
                        projectGatewayMapping.IsUsed = 1;
                        _globalDBContext.ProjectGatewayMapping.Add(projectGatewayMapping);
                        _globalDBContext.SaveChanges();
                    }
                }
                else
                {
                    ProjectGatewayMapping projectGatewayMapping = new ProjectGatewayMapping();
                    projectGatewayMapping.ProjectId = _mainDBContext.Project.FirstOrDefault().PrjId;
                    projectGatewayMapping.ProjectName = DbManager.SiteName;
                    projectGatewayMapping.GatewaySrNo = model.SerialNo;
                    projectGatewayMapping.GatewayNo = model.GatewayNo;
                    projectGatewayMapping.IsUsed = 1;
                    _globalDBContext.ProjectGatewayMapping.Update(projectGatewayMapping);
                    _globalDBContext.SaveChanges();
                }
                bool srnnoexists = await _projectService.CheckProjectSerialNo(id, (int)model.SerialNo);
                if (srnnoexists)
                {
                    return BadRequest();
                }
                var proList = await _projectService.UpdateGateway(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetMultiNetworkRtu)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User model</returns>
        [HttpGet("GetUpdateIdRequired")]
        public async Task<ActionResult<List<UpdateIdsRequired>>> GetUpdateIdRequired()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                List<UpdateIdsRequired> proList = await _projectService.GetUpdateIds();
                return proList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectController)}.{nameof(GetUpdateIdRequired)}]{ex}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

       
    }
}