using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Services;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguraionController : ControllerBase
    {
        private IConfigurationService _configurationService;
        private readonly IMapper _mapper;
        private readonly ILogger<ConfiguraionController> _logger;

        public ConfiguraionController(IConfigurationService configurationService,
            IMapper mapper,
            ILogger<ConfiguraionController> logger)
        {
            _configurationService = configurationService;
            _mapper = mapper;
            _logger = logger;
        }
        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet]
        public async Task<List<RechableNode>> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<RechableNode> nonRechableNodes = await _configurationService.GetRechableNode();
                return nonRechableNodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("NonRechableNode")]
        public async Task<List<NonRechableNode>> GetNonRechableNodes(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<NonRechableNode> nodes = await _configurationService.GetNonRechableNode();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetNonRechableNodes) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("DeleteAllConf")]
        public async Task<ActionResult> DeleteAllConf()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                bool nodes = await _configurationService.DeleteAllConf();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetNonRechableNodes) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetGatewayNode")]
        public async Task<List<GatewayNode>> GetGatewayNode(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<GatewayNode> nodes = await _configurationService.GetGatewayAsNode();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetGatewayNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetVrt")]
        public async Task<List<Vrtsetting>> GetVrt(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Vrtsetting> nodes = await _configurationService.GeVrt();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetVrt) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetSensorAnalog05v")]
        public async Task<List<Analog05vsensor>> GetSensorAnalog05v(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Analog05vsensor> nodes = await _configurationService.GetSensorAnalog0_5VSensor();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetSensorAnalog05v) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetFirmWareData")]
        public async Task<List<Analog05vsensor>> GetFirmWareData([FromBody] RootFirmware model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Analog05vsensor> nodes = await _configurationService.GetSensorAnalog0_5VSensor();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetSensorAnalog05v) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Pressure Transmiter
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetPressureTransmiter")]
        public async Task<List<PressureTransmiterViewModel>> GetPressureTransmiter(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<PressureTransmiterViewModel> nodes = await _configurationService.GetPressureTransmiterView();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetPressureTransmiter) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetSensorAnalog420v")]
        public async Task<List<Analog420mAsensor>> GetSensorAnalog420v(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Analog420mAsensor> nodes = await _configurationService.GetSensorAnalog4_20mASensor();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetSensorAnalog420v) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetDigitalNO_NCTypeSensor")]
        public async Task<List<DigitalNoNctypeSensor>> GetDigitalNO_NCTypeSensor(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<DigitalNoNctypeSensor> nodes = await _configurationService.GetSensorDigitalNO_NCTypeSensor();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetDigitalNO_NCTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetDigitalCounterTypeSensor")]
        public async Task<List<DigitalCounterTypeSensor>> GetDigitalCounterTypeSensor(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<DigitalCounterTypeSensor> nodes = await _configurationService.GetSensorDigitalCounterTypeSensor();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetDigitalCounterTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetWaterMeterSensorSetting")]
        public async Task<List<WaterMeterSensorSetting>> GetWaterMeterSensorSetting(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<WaterMeterSensorSetting> nodes = await _configurationService.GetSensorWaterMeterSensorSetting();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetWaterMeterSensorSetting) + "]" + ex);
                throw ex;
            }
        }







        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetRechargeableByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<RechableNode>> GetRechargeableByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<RechableNode> nonRechableNodes = await _configurationService.GetRechableNodeByNodeNetwork(nodeId, networkId);
                return nonRechableNodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetRechargeableByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("NonRechableNodeByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<NonRechableNode>> NonRechableNodeByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<NonRechableNode> nodes = await _configurationService.GetNonRechableNodeByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(NonRechableNodeByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetGatewayNodeByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<GatewayNode>> GetGatewayNodeByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<GatewayNode> nodes = await _configurationService.GetGatewayAsNodeByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetGatewayNodeByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetVrtByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<Vrtsetting>> GetVrtByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Vrtsetting> nodes = await _configurationService.GeVrtByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetVrtByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetSensorAnalog05vByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<Analog05vsensor>> GetSensorAnalog05vByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Analog05vsensor> nodes = await _configurationService.GetSensorAnalog0_5VSensorByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetSensorAnalog05vByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetSensorAnalog420vByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<Analog420mAsensor>> GetSensorAnalog420vByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Analog420mAsensor> nodes = await _configurationService.GetSensorAnalog4_20mASensorByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetSensorAnalog420vByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetDigitalNO_NCTypeSensorByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<DigitalNoNctypeSensor>> GetDigitalNO_NCTypeSensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<DigitalNoNctypeSensor> nodes = await _configurationService.GetSensorDigitalNO_NCTypeSensorByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetDigitalNO_NCTypeSensorByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetDigitalCounterTypeSensorByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<DigitalCounterTypeSensor>> GetDigitalCounterTypeSensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<DigitalCounterTypeSensor> nodes = await _configurationService.GetSensorDigitalCounterTypeSensorByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetDigitalCounterTypeSensorByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetWaterMeterSensorSettingByNodeNetwork/{nodeId}/{networkId}")]
        public async Task<List<WaterMeterSensorSetting>> GetWaterMeterSensorSettingByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<WaterMeterSensorSetting> nodes = await _configurationService.GetSensorWaterMeterSensorSettingByNodeNetwork(nodeId, networkId);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetWaterMeterSensorSettingByNodeNetwork) + "]" + ex);
                throw ex;
            }
        }








        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetUpdateIdsGateway")]
        public async Task<List<UpdateIds>> GetUpdateIdsGateway(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<UpdateIds> nodes = await _configurationService.GetUpdateIdsGateway();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetUpdateIdsGateway) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetUpdateIdsServerRequired")]
        public async Task<List<UpdateIdsRequired>> GetUpdateIdsServerRequired(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<UpdateIdsRequired> nodes = await _configurationService.GetUpdateIdsServerRequired();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetUpdateIdsServerRequired) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetUpdateIdProjSchViewModel")]
        public async Task<UpdateIdProjSchViewModel> GetUpdateIdProjSchViewModel(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                UpdateIdProjSchViewModel nodes = await _configurationService.GetUpdateIdsProject();
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfiguraionController) + "." + nameof(GetUpdateIdProjSchViewModel) + "]" + ex);
                throw ex;
            }
        }
    }
}
