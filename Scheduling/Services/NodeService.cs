using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using Scheduling.ViewModels.Lib;
using Scheduling.ViewModels.ResourceParamaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface INodeService
    {
        Task<List<Node>> GetNodeByNetworkId(int networkId);
        Task<Node> GetNodeById(int nodeID);
        Task<Node> CreateNode(Node models);
        Task<Node> EditNode(Node models);
        Task<bool> DeleteByNodeId(int NodeId);
        Task<bool> IsNodeExists(int id, string NodeName);

        //MultiNodeNwDataFrame
        Task<PagedList<MultiNodeNwDataFrame>> GetNodeNetworkDataFrame(ResourceParameter resourceParameter);
        Task<IQueryable<MultiNodeNwDataFrame>> DownloadNodeNetworkDataFrame(PostEvents model);
        Task<PagedList<MultiNodeNwDataFrame>> GetNodeNetworkDataFrameByDate(PostEvents model);

        //MultiRtuAnalysis
        Task<PagedList<MultiRtuAnalysis>> GetMultiRtuAnalysis(PostEventsDataLlogger model);
        Task<PagedList<MultiRtuAnalysis>> GetMultiRtuAnalysisNodes(int NwId, int NodeId, MultiRtuAnalysisResourceParamater resourceParameter);

        //MultiNodeJoinDataFrame
        Task<List<NodeJoinForMobileViewModel>> GetMultiNodeJoinDataFrameForMobile();
        Task<PagedList<MultiNodeJoinDataFrame>> GetMultiNodeJoinDataFrame(ResourceParameter resourceParameter);
        Task<IQueryable<MultiNodeJoinDataFrame>> DownloadMultiNodeJoinDataFrame(PostEvents model);
        Task<PagedList<MultiNodeJoinDataFrame>> GetMultiNodeJoinDataFrameByDate(PostEvents model);

        //MultiHandShakeNonReach
        Task<PagedList<MultiHandShakeNonReach>> GetMultiHandShakeNonReach(ResourceParameter resourceParameter);
        Task<PagedList<MultiHandShakeNonReach>> GetMultiHandShakeNonReachByDate(PostEvents model);
        Task<IQueryable<MultiHandShakeNonReach>> DownloadMultiHandShakeNonReachByDate(PostEvents model);

        //MultiHandShakeReach
        Task<PagedList<MultiHandShakeReach>> GetMultiHandShakeReach(ResourceParameter resourceParameter);
        Task<PagedList<MultiHandShakeReach>> GetMultiHandShakeReachByDate(PostEvents model);
        Task<IQueryable<MultiHandShakeReach>> DownloadMultiHandShakeReachByDate(PostEvents model);

        //GwstatusData
        Task<PagedList<GwstatusData>> GetGwstatusData(ResourceParameter resourceParameter);
        Task<NetworkSumamryViewModel> GetNetworkSumamry(PostEventsDataLlogger model);
        Task<PagedList<GwstatusData>> GetGwstatusDataByDate(PostEvents model);

        Task<List<MissingRtuViewModel>> GeMissingRTU(PostEventsDataLlogger model);
    }
    public class NodeService : INodeService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<NodeService> _logger;
        private IZoneTimeService _zoneTimeService;
        public NodeService(ILogger<NodeService> logger,
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
        /// Get Node Grounding By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<List<Node>> GetNodeByNetworkId(int networkId)
        {
            try
            {
                List<Node> Node = new List<Node>();
                var nodes = _mainDBContext.Node.AsQueryable();
                if (networkId == 0)
                {
                    Node = await nodes.ToListAsync();

                }
                else
                {
                    Node = await nodes.Where(x => x.NetworkId == networkId).ToListAsync();

                }
                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }


        public async Task<PagedList<MultiRtuAnalysis>> GetMultiRtuAnalysisNodes(int NwId, int NodeId, MultiRtuAnalysisResourceParamater resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                List<MultiRtuAnalysis> Node = new List<MultiRtuAnalysis>();
                DateTime dtstr = Convert.ToDateTime(resourceParameter.StartDateTime);
                DateTime dtend = Convert.ToDateTime(resourceParameter.EndDateTime);
                var nodes = _mainDBContext.MultiRtuAnalysis.Where(x => x.AddedDateTime >= dtstr
                    && x.AddedDateTime <= dtend &&
                    x.NetworkNo == NwId && x.NodeNo == NodeId).AsQueryable().OrderByDescending(y => y.AddedDateTime);
                return PagedList<MultiRtuAnalysis>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }

        public async Task<PagedList<MultiRtuAnalysis>> GetMultiRtuAnalysis(PostEventsDataLlogger model)
        {
            try
            {
                List<MultiRtuAnalysis> Node = new List<MultiRtuAnalysis>();
                var nodes = _mainDBContext.MultiRtuAnalysis.Where(x => x.AddedDateTime >= model.StartDateTime
                    && x.AddedDateTime <= model.EndDateTime
                    && x.NetworkNo == model.GwidNo).AsEnumerable().GroupBy(x => x.NodeNo, (key, g) => g.OrderByDescending(y => y.AddedDateTime).First()).AsQueryable();
                return PagedList<MultiRtuAnalysis>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }

        public async Task<NetworkSumamryViewModel> GetNetworkSumamry(PostEventsDataLlogger model)
        {
            try
            {
                NetworkSumamryViewModel networkSumamryViewModel = new NetworkSumamryViewModel();

                var nwDataLsit = _mainDBContext.MultiNodeNwDataFrame.Where(x => x.NetworkNo == model.GwidNo && x.AddedDatetime >= model.StartDateTime
                    && x.AddedDatetime <= model.EndDateTime).AsQueryable().ToList();

                var nodeNwDataFrameList = nwDataLsit.GroupBy(x => x.NodeId, (key, g) => g.OrderByDescending(y => y.AddedDatetime).First()).ToList();

                List<MultiNodeNwDataFrameViewModel> modelList = new List<MultiNodeNwDataFrameViewModel>();
                modelList = _mapper.Map<List<MultiNodeNwDataFrameViewModel>>(nodeNwDataFrameList);

                modelList.ForEach(async item =>
                {
                    var nodeNwDataFrameListSecondRec = nwDataLsit.
                   Where(x => x.NodeId == item.NodeId).OrderByDescending(y => y.AddedDatetime).Skip(1).Take(1).FirstOrDefault();
                    if (nodeNwDataFrameListSecondRec != null)
                    {
                        if (item.NgcurrentSf >= nodeNwDataFrameListSecondRec.NgcurrentSf)
                        {
                            item.Arrow = 1;
                        }
                        else
                        {
                            item.Arrow = 0;
                        }
                    }
                });

                var nodesNonReach = _mainDBContext.MultiHandShakeNonReach.Where(x => x.AddedDatetime >= model.StartDateTime
                       && x.AddedDatetime <= model.EndDateTime
                       && x.NetworkNo == model.GwidNo).AsQueryable().ToList().GroupBy(x => x.NodeId, (key, g) => g.OrderByDescending(y => y.AddedDatetime).First()).ToList();

                var nodesReach = _mainDBContext.MultiHandShakeReach.Where(x => x.AddedDatetime >= model.StartDateTime
                    && x.AddedDatetime <= model.EndDateTime
                    && x.NetworkNo == model.GwidNo).AsQueryable().ToList().GroupBy(x => x.NodeId, (key, g) => g.OrderByDescending(y => y.AddedDatetime).First()).ToList();

                var nodesGwData = _mainDBContext.GwstatusData.Where(x => x.AddedDateTime >= model.StartDateTime
                    && x.AddedDateTime <= model.EndDateTime
                    ).AsQueryable().
                   OrderByDescending(x => x.AddedDateTime).ToList();


                networkSumamryViewModel.NodeNwDataFrameList = modelList;
                networkSumamryViewModel.MultiHandShakeNonReachList = nodesNonReach;
                networkSumamryViewModel.MultiHandShakeReachList = nodesReach;
                networkSumamryViewModel.GwstatusDataList = nodesGwData;
                return networkSumamryViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNetworkSumamry) + "]" + ex);
                throw ex;
            }
        }


        public async Task<List<MissingRtuViewModel>> GeMissingRTU(PostEventsDataLlogger model)
        {
            try
            {
                MissingRtuViewModel missingRtuModel = new MissingRtuViewModel();
                List<MissingRtuViewModel> allRtuList = new List<MissingRtuViewModel>();
                List<MissingRtuViewModel> missingRtuList = new List<MissingRtuViewModel>();

                var dataList = _mainDBContext.MultiRtuAnalysis.AsEnumerable().
                     Where(x => x.AddedDateTime >= model.StartDateTime
                     && x.AddedDateTime <= model.EndDateTime
                     && x.NetworkNo == model.GwidNo).Select(x => new MissingRtuViewModel
                     {
                         NetworkNo = x.NetworkNo,
                         NodeId = x.NodeNo,
                         FrameName = x.EventType,
                         AddedDateTime = x.AddedDateTime

                     }).GroupBy(x => x.NodeId, (key, g) => g.OrderByDescending(y => y.AddedDateTime).First());

                foreach (var item in dataList)
                {
                    if (item.AddedDateTime < DateTime.Now.AddHours(-3)) //Check 3 hours data logic
                    {
                        missingRtuList.Add(item);
                    }
                }
                return missingRtuList.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNetworkSumamry) + "]" + ex);
                throw ex;
            }
        }



        //
        /// <summary>
        /// Get Node Grounding By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<IQueryable<MultiNodeNwDataFrame>> DownloadNodeNetworkDataFrame(PostEvents model)
        {
            try
            {
                var nodes = _mainDBContext.MultiNodeNwDataFrame.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();

                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(DownloadNodeNetworkDataFrame) + "]" + ex);
                throw ex;
            }
        }

        //
        /// <summary>
        /// Get Node Grounding By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<PagedList<MultiNodeNwDataFrame>> GetNodeNetworkDataFrame(ResourceParameter resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                var nodes = _mainDBContext.MultiNodeNwDataFrame.OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiNodeNwDataFrame>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }

        public async Task<PagedList<MultiHandShakeNonReach>> GetMultiHandShakeNonReach(ResourceParameter resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                List<MultiHandShakeNonReach> Node = new List<MultiHandShakeNonReach>();
                var nodes = _mainDBContext.MultiHandShakeNonReach.OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiHandShakeNonReach>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetMultiHandShakeNonReach) + "]" + ex);
                throw ex;
            }
        }

        public async Task<PagedList<MultiHandShakeReach>> GetMultiHandShakeReach(ResourceParameter resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                List<MultiHandShakeReach> Node = new List<MultiHandShakeReach>();
                var nodes = _mainDBContext.MultiHandShakeReach.OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiHandShakeReach>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetMultiHandShakeReach) + "]" + ex);
                throw ex;
            }
        }



        public async Task<List<NodeJoinForMobileViewModel>> GetMultiNodeJoinDataFrameForMobile()
        {
            try
            {
                var nodes = _mainDBContext.MultiNodeJoinDataFrame.AsQueryable();
                List<NodeJoinForMobileViewModel> nodeJoinForMobileViewModels = nodes.Select(x =>
                new NodeJoinForMobileViewModel
                {
                    NodeId = x.NodeId,
                    NodeNo = x.NodeNo,
                    GatewayId = x.GatewayId,
                    NetworkNo = x.NetworkNo
                }).Distinct().ToList();
                return nodeJoinForMobileViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }

        public async Task<PagedList<MultiNodeJoinDataFrame>> GetMultiNodeJoinDataFrame(ResourceParameter resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                var nodes = _mainDBContext.MultiNodeJoinDataFrame.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiNodeJoinDataFrame>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }

        //
        /// <summary>
        /// Get Node Grounding By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<IQueryable<MultiNodeJoinDataFrame>> DownloadMultiNodeJoinDataFrame(PostEvents model)
        {
            try
            {
                var nodes = _mainDBContext.MultiNodeJoinDataFrame.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();

                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(DownloadNodeNetworkDataFrame) + "]" + ex);
                throw ex;
            }
        }
        public async Task<PagedList<GwstatusData>> GetGwstatusData(ResourceParameter resourceParameter)
        {
            try
            {
                if (resourceParameter == null)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                if (resourceParameter.PageNumber == 0 & resourceParameter.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(resourceParameter));
                }
                var nodes = _mainDBContext.GwstatusData.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<GwstatusData>.Create(nodes, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }




        public async Task<PagedList<MultiHandShakeNonReach>> GetMultiHandShakeNonReachByDate(PostEvents model)
        {
            List<MultiHandShakeNonReach> list = new List<MultiHandShakeNonReach>();
            try
            {
                var nodes = _mainDBContext.MultiHandShakeNonReach.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiHandShakeNonReach>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IQueryable<MultiHandShakeNonReach>> DownloadMultiHandShakeNonReachByDate(PostEvents model)
        {
            List<MultiHandShakeNonReach> list = new List<MultiHandShakeNonReach>();
            try
            {
                var nodes = _mainDBContext.MultiHandShakeNonReach.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return nodes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IQueryable<MultiHandShakeReach>> DownloadMultiHandShakeReachByDate(PostEvents model)
        {
            try
            {
                var nodes = _mainDBContext.MultiHandShakeReach.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return nodes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<PagedList<MultiHandShakeReach>> GetMultiHandShakeReachByDate(PostEvents model)
        {
            try
            {
                var nodes = _mainDBContext.MultiHandShakeReach.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiHandShakeReach>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<PagedList<MultiNodeNwDataFrame>> GetNodeNetworkDataFrameByDate(PostEvents model)
        {
            try
            {
                var nodes = _mainDBContext.MultiNodeNwDataFrame.Where(x => x.AddedDatetime >= model.StartDateTime && x.AddedDatetime <= model.EndDateTime).OrderByDescending(x => x.AddedDatetime).AsQueryable();
                return PagedList<MultiNodeNwDataFrame>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<MultiNodeJoinDataFrame>> GetMultiNodeJoinDataFrameByDate(PostEvents model)
        {
            List<MultiNodeJoinDataFrame> list = new List<MultiNodeJoinDataFrame>();
            try
            {
                var nodes = _mainDBContext.MultiNodeJoinDataFrame.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiNodeJoinDataFrame>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<GwstatusData>> GetGwstatusDataByDate(PostEvents model)
        {
            List<GwstatusData> list = new List<GwstatusData>();
            try
            {
                var nodes = _mainDBContext.GwstatusData.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<GwstatusData>.Create(nodes, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        //
        /// <summary>
        /// Get Node Grounding By Node Id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>List<Node></returns>
        public async Task<Node> GetNodeById(int nodeId)
        {
            try
            {
                Node Node = new Node();
                var nodes = _mainDBContext.Node.AsQueryable();
                Node = await nodes.Where(x => x.Id == nodeId).FirstOrDefaultAsync();

                return Node;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetNodeByNetworkId) + "]" + ex);
                throw ex;
            }
        }
        /// <summary>
        /// Create Node Grounding
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<Node></returns>
        public async Task<Node> CreateNode(Node models)
        {
            try
            {
                //  List<Node> Node = _mapper.Map<List<Node>>(models);
                await _mainDBContext.Node.AddRangeAsync(models);
                await _mainDBContext.SaveChangesAsync();
                //List<Node> NodeNode = _mapper.Map<List<Node>>(Node);
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(CreateNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Edit Node Grounding
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<Node></returns>
        public async Task<Node> EditNode(Node models)
        {
            try
            {

                var nodeModel = await _mainDBContext.Node.Where(x => x.Id == models.Id && x.Id == models.Id).FirstOrDefaultAsync();
                nodeModel.NodeName = models.NodeName;
                nodeModel.NodeNo = models.NodeNo;
                nodeModel.Description = models.Description;
                nodeModel.NetworkId = models.NetworkId;
                nodeModel.NetworkNo = models.NetworkNo;
                nodeModel.RtuId = models.RtuId;
                nodeModel.ProductTypeId = models.ProductTypeId;
                nodeModel.IsAddonCard = models.IsAddonCard;
                await _mainDBContext.SaveChangesAsync();

                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(CreateNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// remove Node Grounding by Node id
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns>bool</returns>
        public async Task<bool> DeleteByNodeId(int NodeId)
        {
            try
            {
                var node = await _mainDBContext.Node.Where(x => x.Id == NodeId).ToListAsync();
                _mainDBContext.Node.RemoveRange(node);
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(DeleteByNodeId) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Is Node Grounding exists
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NodeId"></param>
        /// <returns>bool</returns>
        public async Task<bool> IsNodeExists(int id, string NodeName)
        {
            try
            {
                return await _mainDBContext.Node.AnyAsync(x => x.Id == id && x.NodeName == NodeName);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(IsNodeExists) + "]" + ex);
                throw ex;
            }
        }
    }
}
