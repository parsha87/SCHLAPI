using AutoMapper;
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
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService,
            IMapper mapper,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<EstimateDCCablingViewModel></returns>
        [HttpGet]
        public async Task<List<ProductType>> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<ProductType> productTypes = await _productService.GetProductTypes();
                return productTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ProductController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// Get CardType
        /// </summary>
        /// <returns>List<GetCardType></returns>
        [HttpGet("GetCardType")]
        public async Task<List<CardType>> GetCardType()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<CardType> cardType = await _productService.GetCardTypes();
                return cardType;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ProductController) + "." + nameof(GetCardType) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<EstimateDCCablingViewModel></returns>
        [HttpGet("GetNonRechableNodeByNodeId")]
        public async Task<NonRechableNode> GetNonRechableNodeByNodeId(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                NonRechableNode productTypes = await _productService.GetNonRechableNodeByNodeId(id);
                return productTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ProductController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<EstimateDCCablingViewModel></returns>
        [HttpGet("GetRechableNodeByNodeId")]
        public async Task<RechableNode> GetRechableNodeByNodeId(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                RechableNode productTypes = await _productService.GetRechableNodeByNodeId(id);
                return productTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ProductController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<EstimateDCCablingViewModel></returns>
        [HttpGet("GetGatewayNodeByNodeId")]
        public async Task<GatewayNode> GetGatewayNodeByNodeId(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                GatewayNode productTypes = await _productService.GetGatewayNodeByNodeId(id);
                return productTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(ProductController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="models">NonRechableNode</param>
        /// <returns></returns>
        [HttpPost("AddNonRechargeableNode")]
        public async Task<IActionResult> AddNonRechargeableNode([FromBody] NonRechableNode models)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                //if (models.Id == 0)
                //{
                //    return BadRequest();
                //}
                if (!IsModelValid(models))
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                models = await _productService.CreateNonRechableNode(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(AddNonRechargeableNode) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(AddNonRechargeableNode), new { id = models.Id }, true);
        }

        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="models">NonRechableNode</param>
        /// <returns></returns>
        [HttpPost("AddRechargeableNode")]
        public async Task<IActionResult> AddRechargeableNode([FromBody] RechableNode models)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                //if (models.Id == 0)
                //{
                //    return BadRequest();
                //}
                if (!IsModelValidRechableNode(models))
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                models = await _productService.CreateRechableNode(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(AddRechargeableNode) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(AddRechargeableNode), new { id = models.Id }, true);
        }

        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="models">NonRechableNode</param>
        /// <returns></returns>
        [HttpPost("AddGatewayNode")]
        public async Task<IActionResult> AddGatewayNode([FromBody] GatewayNode models)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (!ModelState.IsValid)
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                //if (models.Id == 0)
                //{
                //    return BadRequest();
                //}
                if (!IsModelValidGatewayNode(models))
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }

                models = await _productService.CreateGatewayNode(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(AddGatewayNode) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(AddGatewayNode), new { id = models.Id }, true);
        }



        /// <summary>
        /// Edit Non Rechargable node
        /// </summary>
        /// <param name="model">NonRechableNode</param>
        /// <returns>RoleViewModel</returns>
        [HttpPut("EditNonRechargeableNode/{id}")]
        public async Task<ActionResult<OperationResult<NonRechableNode>>> EditNonRechargeableNode(int id, [FromBody] NonRechableNode model)
        {
            DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
            if (id != model.Id)
            {
                return BadRequest();
            }
            //if (model.Name == "BackendAdmin")
            //{
            //    ModelState.AddModelError("Error", "Role name is already in use, please contact administration.");
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            //if (!IsModelValid(model))
            //{
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            var res = await _productService.EditNonRechableNode(model);
            if (res)
            {
                return Ok(new OperationResult<NonRechableNode> { Succeeded = true, Data = model });
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Edit Non Rechargable node
        /// </summary>
        /// <param name="model">NonRechableNode</param>
        /// <returns>RoleViewModel</returns>
        [HttpPut("EditRechargeableNode/{id}")]
        public async Task<ActionResult<OperationResult<NonRechableNode>>> EditRechargeableNode(int id, [FromBody] RechableNode model)
        {
            DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
            if (id != model.Id)
            {
                return BadRequest();
            }
            //if (model.Name == "BackendAdmin")
            //{
            //    ModelState.AddModelError("Error", "Role name is already in use, please contact administration.");
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            //if (!IsModelValid(model))
            //{
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            var res = await _productService.EditRechableNode(model);
            if (res)
            {
                return Ok(new OperationResult<RechableNode> { Succeeded = true, Data = model });
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Edit Non Rechargable node
        /// </summary>
        /// <param name="model">NonRechableNode</param>
        /// <returns>RoleViewModel</returns>
        [HttpPut("EditGatewayNode/{id}")]
        public async Task<ActionResult<OperationResult<NonRechableNode>>> EditGatewayNode(int id, [FromBody] GatewayNode model)
        {
            DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
            if (id != model.Id)
            {
                return BadRequest();
            }
            //if (model.Name == "BackendAdmin")
            //{
            //    ModelState.AddModelError("Error", "Role name is already in use, please contact administration.");
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            //if (!IsModelValid(model))
            //{
            //    return BadRequest(new CustomBadRequest(ModelState));
            //}
            var res = await _productService.EditGatewayNode(model);
            if (res)
            {
                return Ok(new OperationResult<GatewayNode> { Succeeded = true, Data = model });
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// check if model valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelValid(NonRechableNode model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }
            if (model.NodeId == 0)
            {
                ModelState.AddModelError("", "Node not assigned.");
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (allErrors.ToList().Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// check if model valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelValidRechableNode(RechableNode model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }
            if (model.NodeId == 0)
            {
                ModelState.AddModelError("", "Node not assigned.");
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (allErrors.ToList().Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// check if model valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelValidGatewayNode(GatewayNode model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }
            if (model.NodeId == 0)
            {
                ModelState.AddModelError("", "Node not assigned.");
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (allErrors.ToList().Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
