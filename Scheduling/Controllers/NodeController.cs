using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using Scheduling.ViewModels.Lib;
using Scheduling.ViewModels.OutputModels;
using Scheduling.ViewModels.ResourceParamaters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private INodeService _nodeService;
        private readonly IMapper _mapper;
        private readonly ILogger<NodeController> _logger;

        public NodeController(INodeService nodeService,
            IMapper mapper,
            ILogger<NodeController> logger)
        {
            _nodeService = nodeService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet]
        public async Task<List<Node>> Get(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<Node> nodes = await _nodeService.GetNodeByNetworkId(id);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(Get) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetNodeById")]
        public async Task<Node> GetNodeById(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                Node nodes = await _nodeService.GetNodeById(id);
                return nodes;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetNodeById) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get RTU Analysis
        /// </summary>
        /// <returns>List<MultiRtuAnalysis></returns>
        [HttpPost("GetRTUAnalysis")]
        public async Task<IActionResult> GetRTUAnalysis(PostEventsDataLlogger model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiRtuAnalysis(model);
                var outputModel = new DataOutputModel
                {
                    Paging = nodes.GetHeader(),
                    Links = GetLinks(nodes),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetRTUAnalysis) + "]" + ex);
                throw ex;
            }
        }

        [HttpGet("GetRTUAnalysisNodeList/{nwid}/{nodeid}")]
        [ProducesResponseType(200, Type = typeof(List<MultiRtuAnalysis>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetRTUAnalysisNodeList(int nwid, int nodeid, [FromQuery] MultiRtuAnalysisResourceParamater resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiRtuAnalysisNodes(nwid, nodeid, resourceParameter);
                //Response.Headers.Add("X-Pagination", nodes.GetHeader().ToJson());
                var outputModel = new DataOutputModel
                {
                    Paging = nodes.GetHeader(),
                    Links = GetLinks(nodes),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetRTUAnalysis) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadHndNonReachEventsByDate")]
        public async Task<IActionResult> DownloadHndNonReachEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.DownloadMultiHandShakeNonReachByDate(model);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("HandshakeNonReach");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "LastCommTime";
                    worksheet.Cell(currentRow, 4).Value = "System Version Number";
                    worksheet.Cell(currentRow, 5).Value = "Configuration Setting Number";
                    worksheet.Cell(currentRow, 6).Value = "Schedule Update Number";
                    worksheet.Cell(currentRow, 7).Value = "Sensor Update Number";
                    worksheet.Cell(currentRow, 8).Value = "Tempreture Reading";
                    worksheet.Cell(currentRow, 9).Value = "Battery Volt Reading";
                    worksheet.Cell(currentRow, 10).Value = "LVS Start Time";
                    worksheet.Cell(currentRow, 11).Value = "LVS Duration";
                    worksheet.Cell(currentRow, 12).Value = "LVS Opreation";
                    worksheet.Cell(currentRow, 13).Value = "LVS Valve No";
                    worksheet.Cell(currentRow, 14).Value = "LVS ROP";
                    worksheet.Cell(currentRow, 15).Value = "Number of Blank Schedule Rows";
                    worksheet.Cell(currentRow, 16).Value = "Valve Status";
                    worksheet.Cell(currentRow, 17).Value = "Digital Input Status";   
                    worksheet.Cell(currentRow, 18).Value = "Server Received DateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.Soy;
                        worksheet.Cell(currentRow, 4).Value = user.VerNumber;
                        worksheet.Cell(currentRow, 5).Value = user.ConfigVerNumber;
                        worksheet.Cell(currentRow, 6).Value = user.ScheduleUpno;
                        worksheet.Cell(currentRow, 7).Value = user.SensorUpno;
                        worksheet.Cell(currentRow, 8).Value = user.TempReading;
                        worksheet.Cell(currentRow, 9).Value = user.BatteryVoltReading/10;
                        worksheet.Cell(currentRow, 10).Value = user.LvsStartTime;
                        worksheet.Cell(currentRow, 11).Value = user.LvsDuration;
                        worksheet.Cell(currentRow, 12).Value = user.LvsOperation;
                        worksheet.Cell(currentRow, 13).Value = user.LvsValveNo;
                        worksheet.Cell(currentRow, 14).Value = user.LvsRop;
                        worksheet.Cell(currentRow, 15).Value = user.MoUpno;
                        worksheet.Cell(currentRow, 16).Value = user.ValveStatus;
                        worksheet.Cell(currentRow, 17).Value = user.Distatus;
                        worksheet.Cell(currentRow, 18).Value = user.AddedDatetime;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ValveEvents.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DownloadNodeJoinEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }
        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadHndReachEventsByDate")]
        public async Task<IActionResult> DownloadHndReachEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.DownloadMultiHandShakeReachByDate(model);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("HandshakeNonReach");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "LastCommTime";
                    worksheet.Cell(currentRow, 4).Value = "System Version Number";
                    worksheet.Cell(currentRow, 5).Value = "Configuration Setting Number";
                    worksheet.Cell(currentRow, 6).Value = "Schedule Update Number";
                    worksheet.Cell(currentRow, 7).Value = "Sensor Update Number";
                    worksheet.Cell(currentRow, 8).Value = "Filter Config Update Number";
                    worksheet.Cell(currentRow, 9).Value = "Tempreture Reading";
                    worksheet.Cell(currentRow, 10).Value = "Battery Volt Reading";
                    worksheet.Cell(currentRow, 11).Value = "LVS Start Time";
                    worksheet.Cell(currentRow, 12).Value = "LVS Duration";
                    worksheet.Cell(currentRow, 13).Value = "LVS Opreation";
                    worksheet.Cell(currentRow, 14).Value = "LVS Valve No";
                    worksheet.Cell(currentRow, 15).Value = "LVS ROP";
                    worksheet.Cell(currentRow, 16).Value = "Charging Status";
                    worksheet.Cell(currentRow, 17).Value = "Valve Status";
                    worksheet.Cell(currentRow, 18).Value = "Digital Input Status";
                    worksheet.Cell(currentRow, 19).Value = "Server Received DateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.Soy;
                        worksheet.Cell(currentRow, 4).Value = user.VerNumber;
                        worksheet.Cell(currentRow, 5).Value = user.ConfigVerNumber;
                        worksheet.Cell(currentRow, 6).Value = user.ScheduleUpno;
                        worksheet.Cell(currentRow, 7).Value = user.SensorUpno;
                        worksheet.Cell(currentRow, 8).Value = user.FbwConfigUpno;
                        worksheet.Cell(currentRow, 9).Value = user.TempReading;
                        worksheet.Cell(currentRow, 10).Value = user.BatteryVoltReading / 10;
                        worksheet.Cell(currentRow, 11).Value = user.LvsStartTime;
                        worksheet.Cell(currentRow, 12).Value = user.LvsDuration;
                        worksheet.Cell(currentRow, 13).Value = user.LvsOperation;
                        worksheet.Cell(currentRow, 14).Value = user.LvsValveNo;
                        worksheet.Cell(currentRow, 15).Value = user.LvsRop;
                        worksheet.Cell(currentRow, 16).Value = user.ChargingStatus;
                        worksheet.Cell(currentRow, 17).Value = user.ValveStatus;
                        worksheet.Cell(currentRow, 18).Value = user.Distatus;
                        worksheet.Cell(currentRow, 19).Value = user.AddedDatetime;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ValveEvents.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DownloadHndReachEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadNodeNwEventsByDate")]
        public async Task<IActionResult> DownloadNodeNwEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.DownloadNodeNetworkDataFrame(model);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Users");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "Last Comm Time(SOY)";
                    worksheet.Cell(currentRow, 4).Value = "Current_RSSI";
                    worksheet.Cell(currentRow, 5).Value = "Current_SNR";
                    worksheet.Cell(currentRow, 6).Value = "Current_SF";
                    worksheet.Cell(currentRow, 7).Value = "Freq";
                    worksheet.Cell(currentRow, 8).Value = "Current_CR";
                    worksheet.Cell(currentRow, 9).Value = "Gw id6b";
                    worksheet.Cell(currentRow, 10).Value = "Attempt_3b";
                    worksheet.Cell(currentRow, 11).Value = "Power 2b";
                    worksheet.Cell(currentRow, 12).Value = "SF_GW_2b";
                    worksheet.Cell(currentRow, 13).Value = "GN_SNR_Previous_3b";
                    worksheet.Cell(currentRow, 14).Value = "GN_Rssi_Previous_3b";
                    worksheet.Cell(currentRow, 15).Value = "Product_Type_4b";
                    worksheet.Cell(currentRow, 16).Value = "Received Frame Type";
                    worksheet.Cell(currentRow, 17).Value = "Server Received DateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.LastCommTime;
                        worksheet.Cell(currentRow, 4).Value = user.NgcurrentRssi;
                        worksheet.Cell(currentRow, 5).Value = user.NgcurrentSnr;
                        worksheet.Cell(currentRow, 6).Value = user.NgcurrentSf;
                        worksheet.Cell(currentRow, 7).Value = user.Ngfreq;
                        worksheet.Cell(currentRow, 8).Value = user.NgcurrentCr;
                        worksheet.Cell(currentRow, 9).Value = user.Gwid6b;
                        worksheet.Cell(currentRow, 10).Value = user.Ngattempt3b;
                        worksheet.Cell(currentRow, 11).Value = user.Power2b;
                        worksheet.Cell(currentRow, 12).Value = user.Sfgw2b;
                        worksheet.Cell(currentRow, 13).Value = user.GnsnrPrevious3b;
                        worksheet.Cell(currentRow, 14).Value = user.GnrssiPrevious3b;
                        worksheet.Cell(currentRow, 15).Value = user.ProductType4b;
                        worksheet.Cell(currentRow, 16).Value = user.RxFrametype;
                        worksheet.Cell(currentRow, 17).Value = user.AddedDatetime;

                        //worksheet.Cell(currentRow, 1).Value = user.NodeId;
                        //worksheet.Cell(currentRow, 2).Value = user.UpdataeTimeSow;                        
                        //worksheet.Cell(currentRow, 4).Value = user.ValveType;
                        //worksheet.Cell(currentRow, 9).Value = user.ActiveCurrent;                        
                        //worksheet.Cell(currentRow, 11).Value = user.GwoperationTimeMoy;
                        //worksheet.Cell(currentRow, 14).Value = user.GatewayId;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ValveEvents.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DownloadNodeNwEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadNodeJoinEventsByDate")]
        public async Task<IActionResult> DownloadNodeJoinEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.DownloadMultiNodeJoinDataFrame(model);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Users");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "LastCommTime";
                    worksheet.Cell(currentRow, 4).Value = "DeviceResetCause";
                    worksheet.Cell(currentRow, 5).Value = "Attempt";
                    worksheet.Cell(currentRow, 6).Value = "ProjectId";
                    worksheet.Cell(currentRow, 7).Value = "TechId";
                    worksheet.Cell(currentRow, 8).Value = "GW No. 1";

                    worksheet.Cell(currentRow, 9).Value = "GW No. 2";
                    worksheet.Cell(currentRow, 10).Value = "GW No. 3";
                    worksheet.Cell(currentRow, 11).Value = "GW No. 4";
                    worksheet.Cell(currentRow, 12).Value = "Latitude";
                    worksheet.Cell(currentRow, 13).Value = "Longitude";
                    worksheet.Cell(currentRow, 14).Value = "Minute of the year when setting done";

                    worksheet.Cell(currentRow, 15).Value = "Seconds when setting done";
                    worksheet.Cell(currentRow, 16).Value = "Time Offset in millisec";
                    worksheet.Cell(currentRow, 17).Value = "Year when setting done";
                    worksheet.Cell(currentRow, 18).Value = "LatLongAccuracy";
                    worksheet.Cell(currentRow, 19).Value = "Serial Number of device";
                    worksheet.Cell(currentRow, 20).Value = "Server Received DateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.LastCommTime;
                        worksheet.Cell(currentRow, 4).Value = user.DeviceResetCause;
                        worksheet.Cell(currentRow, 5).Value = user.Attempt;
                        worksheet.Cell(currentRow, 6).Value = user.ProjectId;
                        worksheet.Cell(currentRow, 7).Value = user.TechId;
                        worksheet.Cell(currentRow, 8).Value = user.Gwno1;
                        worksheet.Cell(currentRow, 9).Value = user.Gwno2;
                        worksheet.Cell(currentRow, 10).Value = user.Gwno3;
                        worksheet.Cell(currentRow, 11).Value = user.Gwno4;
                        worksheet.Cell(currentRow, 12).Value = user.Latitude;
                        worksheet.Cell(currentRow, 13).Value = user.Longitude ;
                        worksheet.Cell(currentRow, 14).Value = user.Moy;
                        worksheet.Cell(currentRow, 15).Value = user.Seconds;
                        worksheet.Cell(currentRow, 16).Value = user.Time;
                        worksheet.Cell(currentRow, 17).Value = user.Year;
                        worksheet.Cell(currentRow, 18).Value = user.LatLongAccuracy;
                        worksheet.Cell(currentRow, 19).Value = user.DeviceSrno;
                        worksheet.Cell(currentRow, 20).Value = user.AddedDateTime;


                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "ValveEvents.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DownloadNodeJoinEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetNodeNetworkDataFrame")]
        public async Task<IActionResult> GetNodeNetworkDataFrame([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetNodeNetworkDataFrame(resourceParameter);
                var outputModel = new DtoMultiNodeNwDataFrame
                {
                    Paging = nodes.GetHeader(),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetNodeById) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpPost("GetNetworkSumamry")]
        public async Task<IActionResult> GetNetworkSumamry([FromBody] PostEventsDataLlogger model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                NetworkSumamryViewModel summary = await _nodeService.GetNetworkSumamry(model);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetNetworkSumamry) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpPost("GetMissingRTU")]
        public async Task<List<MissingRtuViewModel>> GetMissingRTU([FromBody] PostEventsDataLlogger model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MissingRtuViewModel> summary = await _nodeService.GeMissingRTU(model);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetNodeById) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetNodeNetworkDataFrameByDate")]
        public async Task<IActionResult> GetNodeNetworkDataFrameByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                var multiValveEvents = await _nodeService.GetNodeNetworkDataFrameByDate(model);
                var outputModel = new DtoMultiNodeNwDataFrame
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetNodeNetworkDataFrameByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }



        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiHandShakeNonReach")]
        public async Task<IActionResult> GetMultiHandShakeNonReach([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiHandShakeNonReach(resourceParameter);
                var outputModel = new DtoMultiHandShakeNonReach
                {
                    Paging = nodes.GetHeader(),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetMultiHandShakeNonReach) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetMultiHandShakeNonReachByDate")]
        public async Task<IActionResult> GetMultiHandShakeNonReachByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.GetMultiHandShakeNonReachByDate(model);
                var outputModel = new DtoMultiHandShakeNonReach
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiHandShakeNonReachByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetMultiHandShakeReachByDate")]
        public async Task<IActionResult> GetMultiHandShakeReachByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.GetMultiHandShakeReachByDate(model);
                var outputModel = new DtoMultiHandShakeReach
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiHandShakeNonReachByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiHandShakeReach")]
        public async Task<IActionResult> GetMultiHandShakeReach([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiHandShakeReach(resourceParameter);
                var outputModel = new DtoMultiHandShakeReach
                {
                    Paging = nodes.GetHeader(),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetMultiHandShakeReach) + "]" + ex);
                throw ex;
            }
        }


        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetMultiNodeJoinDataFrame")]
        public async Task<IActionResult> GetMultiNodeJoinDataFrame([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiNodeJoinDataFrame(resourceParameter);
                var outputModel = new DtoMultiNodeJoinDataFrame
                {
                    Paging = nodes.GetHeader(),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetMultiNodeJoinDataFrame) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetMultiNodeJoinDataForMobile")]
        public async Task<IActionResult> GetMultiNodeJoinDataForMobile()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetMultiNodeJoinDataFrameForMobile();
                return Ok(CustomResponse.CreateResponse(true, string.Empty, nodes, 1));
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetMultiNodeJoinDataForMobile) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while fetching data.", "", 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetMultiNodeJoinDataFrameByDate")]
        public async Task<IActionResult> GetMultiNodeJoinDataFrameByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.GetMultiNodeJoinDataFrameByDate(model);
                var outputModel = new DtoMultiNodeJoinDataFrame
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiNodeJoinDataFrameByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }
        /// <summary>
        /// Get Node by  Node Id
        /// </summary>
        /// <returns>List<Node></returns>
        [HttpGet("GetGwstatusData")]
        public async Task<IActionResult> GetGwstatusData([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetGwstatusData(resourceParameter);
                var outputModel = new DtoGwstatusData
                {
                    Paging = nodes.GetHeader(),
                    Items = nodes.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(GetGwstatusData) + "]" + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetGwstatusDataByDate")]
        public async Task<IActionResult> GetGwstatusDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _nodeService.GetGwstatusDataByDate(model);
                var outputModel = new DtoGwstatusData
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetGwstatusDataByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Create Node
        /// </summary>
        /// <param name="models">List<Node></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Node models)
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

                models = await _nodeService.CreateNode(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(Post) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(Get), new { id = models.Id }, true);
        }

        /// <summary>
        /// Edit Node
        /// </summary>
        /// <param name="nodeId">nodeId</param>
        /// <param name="models">List<Node></param>
        /// <returns></returns>
        [HttpPut("{nodeId}")]
        public async Task<IActionResult> Put(string nodeId, [FromBody] Node models)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                if (Convert.ToInt32(nodeId) != models.Id || models.Id == 0)
                {
                    return BadRequest();
                }
                if (!IsModelValid(models))
                {
                    return BadRequest(new CustomBadRequest(ModelState));
                }
                //check for estimate dc plant
                bool isDCCablingExists = await _nodeService.IsNodeExists(models.Id, models.NodeName);
                if (!isDCCablingExists)
                {
                    return NotFound(new { error = $"Estimate Node for type ({models.NodeName}) not found!" });
                }

                models = await _nodeService.EditNode(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(Put) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(models);
        }

        /// <summary>
        /// Delete estimate dc plant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var nodes = await _nodeService.GetNodeByNetworkId(id);
                if (nodes.Count == 0)
                {
                    return NotFound(new { error = $"Node for given estimate not found." });
                }
                var result = await _nodeService.DeleteByNodeId(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(NodeController) + "." + nameof(Delete) + "]" + ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        #region private methods


        /// <summary>
        /// check if model valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelValid(Node model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return false;
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
        private List<LinkInfo> GetLinks(PagedList<MultiRtuAnalysis> list)
        {
            var links = new List<LinkInfo>();

            if (list.HasPreviousPage)
                links.Add(CreateLink("GetAllUsersNew", list.PreviousPageNumber, list.PageSize, "previousPage", "GET"));

            links.Add(CreateLink("GetAllUsersNew", list.PageNumber, list.PageSize, "self", "GET"));

            if (list.HasNextPage)
                links.Add(CreateLink("GetAllUsersNew", list.NextPageNumber, list.PageSize, "nextPage", "GET"));

            return links;
        }
        private LinkInfo CreateLink(
            string routeName, int pageNumber, int pageSize,
            string rel, string method)
        {
            return new LinkInfo
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Rel = rel,
                Method = method
            };
        }

        #endregion
    }
}
