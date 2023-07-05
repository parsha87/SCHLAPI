using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface INodeUpdateDataService
    {
       Task<NodeUpdateData> GetNodeUpdateDataById(int nodeID);

        Task<NodeUpdateData> CreateNodeUpdateData(NodeUpdateData models);
        Task<NodeUpdateData> EditNodeUpdateData(NodeUpdateData models);
        Task<bool> DeleteByNodeUpdateDataId(int NodeId);
        Task<bool> IsNodeUpdateDataExists(int id, string NodeName);
    }
    public class NodeUpdateDataService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<NodeUpdateDataService> _logger;
        private IZoneTimeService _zoneTimeService;
        public NodeUpdateDataService(ILogger<NodeUpdateDataService> logger,
               MainDBContext mainDBContext,
               IMapper mapper,
               IZoneTimeService zoneTimeService
           )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
            _zoneTimeService = zoneTimeService;
        }

        /// <summary>
        /// Get Node Update Data By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<List<NodeUpdateData>> GetNodeUpdateDataById(int nodeId)
        {
            try
            {
                List<NodeUpdateData> Node = new List<NodeUpdateData>();
                var nodes = _mainDBContext.NodeUpdateData.AsQueryable();
                
                Node = await nodes.Where(x => x.NodeId == nodeId).ToListAsync();

                
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeUpdateDataService) + "." + nameof(GetNodeUpdateDataById) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// CreateNodeUpdateData
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<Node></returns>
        public async Task<NodeUpdateData> CreateNodeUpdateData(NodeUpdateData models)
        {
            try
            {
                await _mainDBContext.NodeUpdateData.AddRangeAsync(models);
                await _mainDBContext.SaveChangesAsync();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeUpdateDataService) + "." + nameof(CreateNodeUpdateData) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Edit Node Grounding
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<Node></returns>
        public async Task<NodeUpdateData> EditNodeUpdateData(NodeUpdateData models)
        {
            try
            {

                var nodeModel = await _mainDBContext.NodeUpdateData.Where(x => x.Id == models.Id).FirstOrDefaultAsync();
                //nodeModel.NodeName = models.NodeName;
                //nodeModel.NodeNo = models.NodeNo;           
                _mainDBContext.NodeUpdateData.Update(models);
                await _mainDBContext.SaveChangesAsync();

                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeUpdateDataService) + "." + nameof(EditNodeUpdateData) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// remove Node Grounding by Node id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteByNodeUpdateDataId(int id)
        {
            try
            {
                var node = await _mainDBContext.NodeUpdateData.Where(x => x.Id == id).ToListAsync();
                _mainDBContext.NodeUpdateData.RemoveRange(node);
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeUpdateDataService) + "." + nameof(DeleteByNodeUpdateDataId) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Is Node Grounding exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NodeId"></param>
        /// <returns>bool</returns>
        public async Task<bool> IsNodeUpdateDataExists(int id, int NodeId)
        {
            try
            {
                return await _mainDBContext.NodeUpdateData.AnyAsync(x => x.Id == id && x.NodeId == NodeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeUpdateDataService) + "." + nameof(IsNodeUpdateDataExists) + "]" + ex);
                throw ex;
            }
        }
    }
}
