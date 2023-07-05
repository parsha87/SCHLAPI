using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IConfigurationService
    {
        Task<bool> DeleteAllConf();
        Task<List<RechableNode>> GetRechableNode();
        Task<List<NonRechableNode>> GetNonRechableNode();
        Task<List<GatewayNode>> GetGatewayAsNode();
        Task<List<Vrtsetting>> GeVrt();
        Task<List<Analog05vsensor>> GetSensorAnalog0_5VSensor();
        Task<List<Analog420mAsensor>> GetSensorAnalog4_20mASensor();

        Task<List<PressureTransmiterViewModel>> GetPressureTransmiterView();
        Task<List<DigitalNoNctypeSensor>> GetSensorDigitalNO_NCTypeSensor();
        Task<List<DigitalCounterTypeSensor>> GetSensorDigitalCounterTypeSensor();
        Task<List<WaterMeterSensorSetting>> GetSensorWaterMeterSensorSetting();
        Task<List<RechableNode>> GetRechableNodeByNodeNetwork(int nodeId, int networkId);
        Task<List<NonRechableNode>> GetNonRechableNodeByNodeNetwork(int nodeId, int networkId);
        Task<List<GatewayNode>> GetGatewayAsNodeByNodeNetwork(int nodeId, int networkId);
        Task<List<Vrtsetting>> GeVrtByNodeNetwork(int nodeId, int networkId);
        Task<List<Analog05vsensor>> GetSensorAnalog0_5VSensorByNodeNetwork(int nodeId, int networkId);
        Task<List<Analog420mAsensor>> GetSensorAnalog4_20mASensorByNodeNetwork(int nodeId, int networkId);
        Task<List<DigitalNoNctypeSensor>> GetSensorDigitalNO_NCTypeSensorByNodeNetwork(int nodeId, int networkId);
        Task<List<DigitalCounterTypeSensor>> GetSensorDigitalCounterTypeSensorByNodeNetwork(int nodeId, int networkId);
        Task<List<WaterMeterSensorSetting>> GetSensorWaterMeterSensorSettingByNodeNetwork(int nodeId, int networkId);




        Task<UpdateIdProjSchViewModel> GetUpdateIdsProject();
        Task<List<UpdateIdsRequired>> GetUpdateIdsServerRequired();

        Task<List<UpdateIds>> GetUpdateIdsGateway();
    }


    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<ConfigurationService> _logger;
        public ConfigurationService(ILogger<ConfigurationService> logger,
               MainDBContext mainDBContext, IConfiguration config,
               IMapper mapper
           )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
            _config = config;
        }

        public async Task<bool> DeleteAllConf()
        {
            try
            {
                var dbConnectionString = DbManager.GetDbConnectionString(_config["SiteName"], "Main");
                using (var sqlConnection = new SqlConnection(dbConnectionString))
                {
                    await sqlConnection.OpenAsync();
                    var resultSP = await sqlConnection.QueryMultipleAsync("MultiDelteConfiguration", null, null, null, CommandType.StoredProcedure);
                }
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<RechableNode>> GetRechableNode()
        {
            try
            {
                List<RechableNode> Node = new List<RechableNode>();
                var nodes = _mainDBContext.RechableNode.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetRechableNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<NonRechableNode>> GetNonRechableNode()
        {
            try
            {
                List<NonRechableNode> Node = new List<NonRechableNode>();
                var nodes = _mainDBContext.NonRechableNode.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetNonRechableNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<GatewayNode>> GetGatewayAsNode()
        {
            try
            {
                List<GatewayNode> Node = new List<GatewayNode>();
                var nodes = _mainDBContext.GatewayNode.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetGatewayAsNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Vrtsetting>> GeVrt()
        {
            try
            {
                List<Vrtsetting> lst = new List<Vrtsetting>();
                var nodes = _mainDBContext.Vrtsetting.AsQueryable();
                lst = await nodes.ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetGatewayAsNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Analog05vsensor>> GetSensorAnalog0_5VSensor()
        {
            try
            {
                List<Analog05vsensor> Node = new List<Analog05vsensor>();
                var nodes = _mainDBContext.Analog05vsensor.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorAnalog0_5VSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Analog420mAsensor>> GetSensorAnalog4_20mASensor()
        {
            try
            {
                List<Analog420mAsensor> Node = new List<Analog420mAsensor>();
                var nodes = _mainDBContext.Analog420mAsensor.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorAnalog4_20mASensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<PressureTransmiterViewModel>> GetPressureTransmiterView()
        {
            try
            {
                List<PressureTransmiterViewModel> pressureTransmiter1 = new List<PressureTransmiterViewModel>();
                List<PressureTransmiterViewModel> pressureTransmiter2 = new List<PressureTransmiterViewModel>();

                List<Analog420mAsensor> sensors = new List<Analog420mAsensor>();
                pressureTransmiter1 = await _mainDBContext.Analog420mAsensor.AsQueryable().Select(x => new PressureTransmiterViewModel
                {
                    Id = x.Id,
                    GwSrn = x.GwSrn,
                    NodeId = x.NodeId,
                    NodePorductId = x.NodePorductId,
                    Priority = 0,
                    ProductType = x.ProductType,
                    SsNo = x.SsNo,
                    SsType = x.SsType,
                    TagName = x.TagName,
                    SensorName = x.SensorName,
                    
                }).ToListAsync();
                pressureTransmiter2 = await _mainDBContext.Analog05vsensor.AsQueryable().Select(x => new PressureTransmiterViewModel
                {
                    Id = x.Id,
                    GwSrn = x.GwSrn,
                    NodeId = x.NodeId,
                    NodePorductId = x.NodePorductId,
                    Priority = 0,
                    ProductType = x.ProductType,
                    SsNo = x.SsNo,
                    SsType = x.SsType,
                    TagName = x.TagName,
                    SensorName = x.SensorName
                }).ToListAsync();
                pressureTransmiter1.AddRange(pressureTransmiter2);
                return pressureTransmiter1;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorAnalog4_20mASensor) + "]" + ex);
                throw ex;
            }
        }
        public async Task<List<DigitalNoNctypeSensor>> GetSensorDigitalNO_NCTypeSensor()
        {
            try
            {
                List<DigitalNoNctypeSensor> Node = new List<DigitalNoNctypeSensor>();
                var nodes = _mainDBContext.DigitalNoNctypeSensor.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorDigitalNO_NCTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<DigitalCounterTypeSensor>> GetSensorDigitalCounterTypeSensor()
        {
            try
            {
                List<DigitalCounterTypeSensor> Node = new List<DigitalCounterTypeSensor>();
                var nodes = _mainDBContext.DigitalCounterTypeSensor.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorDigitalCounterTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<WaterMeterSensorSetting>> GetSensorWaterMeterSensorSetting()
        {
            try
            {
                List<WaterMeterSensorSetting> Node = new List<WaterMeterSensorSetting>();
                var nodes = _mainDBContext.WaterMeterSensorSetting.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorWaterMeterSensorSetting) + "]" + ex);
                throw ex;
            }



        }







        public async Task<List<RechableNode>> GetRechableNodeByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<RechableNode> Node = new List<RechableNode>();
                var nodes = _mainDBContext.RechableNode.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetRechableNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<NonRechableNode>> GetNonRechableNodeByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<NonRechableNode> Node = new List<NonRechableNode>();
                var nodes = _mainDBContext.NonRechableNode.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetNonRechableNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<GatewayNode>> GetGatewayAsNodeByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<GatewayNode> Node = new List<GatewayNode>();
                var nodes = _mainDBContext.GatewayNode.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetGatewayAsNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Vrtsetting>> GeVrtByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<Vrtsetting> lst = new List<Vrtsetting>();
                var nodes = _mainDBContext.Vrtsetting.AsQueryable();
                lst = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return lst;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetGatewayAsNode) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Analog05vsensor>> GetSensorAnalog0_5VSensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<Analog05vsensor> Node = new List<Analog05vsensor>();
                var nodes = _mainDBContext.Analog05vsensor.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorAnalog0_5VSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<Analog420mAsensor>> GetSensorAnalog4_20mASensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<Analog420mAsensor> Node = new List<Analog420mAsensor>();
                var nodes = _mainDBContext.Analog420mAsensor.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorAnalog4_20mASensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<DigitalNoNctypeSensor>> GetSensorDigitalNO_NCTypeSensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<DigitalNoNctypeSensor> Node = new List<DigitalNoNctypeSensor>();
                var nodes = _mainDBContext.DigitalNoNctypeSensor.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorDigitalNO_NCTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<DigitalCounterTypeSensor>> GetSensorDigitalCounterTypeSensorByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<DigitalCounterTypeSensor> Node = new List<DigitalCounterTypeSensor>();
                var nodes = _mainDBContext.DigitalCounterTypeSensor.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorDigitalCounterTypeSensor) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<WaterMeterSensorSetting>> GetSensorWaterMeterSensorSettingByNodeNetwork(int nodeId, int networkId)
        {
            try
            {
                List<WaterMeterSensorSetting> Node = new List<WaterMeterSensorSetting>();
                var nodes = _mainDBContext.WaterMeterSensorSetting.AsQueryable();
                Node = await nodes.Where(x => x.NodeId == nodeId && x.GwSrn == networkId).ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetSensorWaterMeterSensorSetting) + "]" + ex);
                throw ex;
            }



        }







        public async Task<List<UpdateIds>> GetUpdateIdsGateway()
        {
            try
            {
                List<UpdateIds> Node = new List<UpdateIds>();
                var nodes = _mainDBContext.UpdateIds.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetUpdateIdsGateway) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<UpdateIdsRequired>> GetUpdateIdsServerRequired()
        {
            try
            {
                List<UpdateIdsRequired> Node = new List<UpdateIdsRequired>();
                var nodes = _mainDBContext.UpdateIdsRequired.AsQueryable();
                Node = await nodes.ToListAsync();
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetUpdateIdsServerRequired) + "]" + ex);
                throw ex;
            }
        }

        public async Task<UpdateIdProjSchViewModel> GetUpdateIdsProject()
        {
            try
            {
                UpdateIdProjSchViewModel node = new UpdateIdProjSchViewModel();
                var nodesPrj = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                var nodessch = await _mainDBContext.UpdateIdsMainSch.FirstOrDefaultAsync();
                List<GatewayMaxSch> lst = await _mainDBContext.GatewayMaxSch.ToListAsync();
                node.ProjUpId = (int)nodesPrj.ProjectUpId;
                node.MaxSchUpId = (int)nodessch.SeqMaxUpid;
                node.GatewayMaxSches = lst;
                return node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ConfigurationService) + "." + nameof(GetUpdateIdsServerRequired) + "]" + ex);
                throw ex;
            }
        }
    }
}
