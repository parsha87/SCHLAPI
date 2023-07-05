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
    public interface IProductService
    {
        Task<List<ProductType>> GetProductTypes();
        Task<List<CardType>> GetCardTypes();
        Task<NonRechableNode> GetNonRechableNodeByNodeId(int id);
        Task<RechableNode> GetRechableNodeByNodeId(int id);
        Task<GatewayNode> GetGatewayNodeByNodeId(int id);

        Task<NonRechableNode> CreateNonRechableNode(NonRechableNode model);
        Task<RechableNode> CreateRechableNode(RechableNode model);
        Task<GatewayNode> CreateGatewayNode(GatewayNode model);

        Task<bool> EditNonRechableNode(NonRechableNode model);
        Task<bool> EditRechableNode(RechableNode model);
        Task<bool> EditGatewayNode(GatewayNode model);
    }
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<ProductService> _logger;
        private IZoneTimeService _zoneTimeService;
        public ProductService(ILogger<ProductService> logger,
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
        public async Task<List<ProductType>> GetProductTypes()
        {
            try
            {
                var products = _mainDBContext.ProductType.AsQueryable();
                List<ProductType> productlst = await products.ToListAsync();
                return productlst;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetProductTypes) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<CardType>> GetCardTypes()
        {
            try
            {
                var cardType = _mainDBContext.CardType.AsQueryable();
                List<CardType> cardTypeLst = await cardType.ToListAsync();
                return cardTypeLst;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetCardTypes) + "]" + ex);
                throw ex;
            }
        }

        
        public async Task<NonRechableNode> GetNonRechableNodeByNodeId(int id)
        {
            try
            {
                NonRechableNode products = await _mainDBContext.NonRechableNode.Where(x => x.NodeId == id).FirstOrDefaultAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetProductTypes) + "]" + ex);
                throw ex;
            }
        }

        public async Task<RechableNode> GetRechableNodeByNodeId(int id)
        {
            try
            {
                RechableNode products = await _mainDBContext.RechableNode.Where(x => x.NodeId == id).FirstOrDefaultAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetProductTypes) + "]" + ex);
                throw ex;
            }
        }

        public async Task<GatewayNode> GetGatewayNodeByNodeId(int id)
        {
            try
            {
                GatewayNode products = await _mainDBContext.GatewayNode.Where(x => x.NodeId == id).FirstOrDefaultAsync();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(GetProductTypes) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Create Non Rechable Node 
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<NonRechableNode></returns>
        public async Task<NonRechableNode> CreateNonRechableNode(NonRechableNode models)
        {
            try
            {
                await _mainDBContext.NonRechableNode.AddRangeAsync(models);
                await _mainDBContext.SaveChangesAsync();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(CreateNonRechableNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Create Rechable Node 
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<RechableNode></returns>
        public async Task<RechableNode> CreateRechableNode(RechableNode models)
        {
            try
            {
                await _mainDBContext.RechableNode.AddRangeAsync(models);
                await _mainDBContext.SaveChangesAsync();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(CreateRechableNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Create Gateway Node
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<GatewayNode></returns>
        public async Task<GatewayNode> CreateGatewayNode(GatewayNode models)
        {
            try
            {
                await _mainDBContext.GatewayNode.AddRangeAsync(models);
                await _mainDBContext.SaveChangesAsync();
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(CreateGatewayNode) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Create Non Rechable Node 
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<NonRechableNode></returns>
        public async Task<bool> EditNonRechableNode(NonRechableNode model)
        {
            try
            {
                var node = await _mainDBContext.NonRechableNode.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
              
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(EditNonRechableNode) + "]" + ex);
                return false;
            }
        }

        /// <summary>
        /// Create Rechable Node 
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<RechableNode></returns>
        public async Task<bool> EditRechableNode(RechableNode model)
        {
            try
            {
                var node = await _mainDBContext.RechableNode.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(EditRechableNode) + "]" + ex);
                return false;
            }
        }

        /// <summary>
        /// Create Gateway Node
        /// </summary>
        /// <param name="models"></param>
        /// <returns>List<GatewayNode></returns>
        public async Task<bool> EditGatewayNode(GatewayNode model)
        {
            try
            {
                var node = await _mainDBContext.GatewayNode.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeService) + "." + nameof(EditGatewayNode) + "]" + ex);
                return false;
            }
        }
    }
}
