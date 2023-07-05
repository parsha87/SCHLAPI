using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using Scheduling.ViewModels.OutputModels;
using Scheduling.ViewModels.ResourceParamaters;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValveController : ControllerBase
    {
        private readonly ILogger<ValveController> _logger;
        private ISequenceService _sequenceService;
        private IValveService _valveService;
        private MainDBContext _mainDBContext;
        private IZoneTimeService _zoneTimeService;
        private IProjectService _projectService;
        private IZoneService _zoneService;
        private ISubBlockService _subBlockService;
        private ISequenceDatetimeService _sequenceDatetimeService;
        public ValveController(IZoneTimeService zoneTimeService,
            MainDBContext mainDBContext,
            ILogger<ValveController> logger,
            ISequenceService sequenceService,
            IProjectService projectService,
            IValveService valveService,
            IZoneService zoneService,
            ISubBlockService subBlockService,
            ISequenceDatetimeService sequenceDatetimeService)
        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _sequenceService = sequenceService;
            _zoneTimeService = zoneTimeService;
            _projectService = projectService;
            _valveService = valveService;
            _zoneService = zoneService;
            _subBlockService = subBlockService;
            _sequenceDatetimeService = sequenceDatetimeService;
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("getValveEvents")]
        public async Task<IActionResult> GetValveEvents([FromQuery] ResourceParameter resourceParameter) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetAllValvesEventsMulti(resourceParameter);
                var outputModel = new DtoMultiValveEvent
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEvents)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getValveEventsByDate")]
        public async Task<IActionResult> GetValveEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetAllValvesEventsMultiByDate(model);
                var outputModel = new DtoMultiValveEvent
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadValveEventsByDate")]
        public async Task<IActionResult> DownloadValveEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.DownloadValvesEventsByDate(model);

                var multiReason = _mainDBContext.MultiValveReason.ToList();
                var multistate = _mainDBContext.MultiValveState.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ValveEvents");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "OperationTimeMoy";
                    worksheet.Cell(currentRow, 2).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 3).Value = "NodeNo";
                    worksheet.Cell(currentRow, 4).Value = "ValveNo";

                    worksheet.Cell(currentRow, 5).Value = "RequiredState";
                    worksheet.Cell(currentRow, 6).Value = "CurrentState";
                    worksheet.Cell(currentRow, 7).Value = "CurrentStateReason";
                    worksheet.Cell(currentRow, 8).Value = "AddedDateTime";


                    //worksheet.Cell(currentRow, 1).Value = "NodeId";
                    //worksheet.Cell(currentRow, 2).Value = "UpdataeTimeSow";                    
                    //worksheet.Cell(currentRow, 4).Value = "ValveType";
                    //worksheet.Cell(currentRow, 9).Value = "ActiveCurrent";                    
                    //worksheet.Cell(currentRow, 11).Value = "GwoperationTimeMoy";
                    //worksheet.Cell(currentRow, 14).Value = "GatewayId";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.OperationTimeMoy;
                        worksheet.Cell(currentRow, 2).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 3).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 4).Value = user.ValveNo;
                        worksheet.Cell(currentRow, 5).Value = multistate.Where(x => x.Value == user.RequiredState).Select(x => x.State).FirstOrDefault();
                        worksheet.Cell(currentRow, 6).Value = multistate.Where(x => x.Value == user.CurrentState).Select(x => x.State).FirstOrDefault();
                        worksheet.Cell(currentRow, 7).Value = multiReason.Where(x => x.Value == user.CurrentStateReason).Select(x => x.Reason).FirstOrDefault(); worksheet.Cell(currentRow, 8).Value = user.AddedDateTime;


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
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }



        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadNodeAlarmEventsByDate")]
        public async Task<IActionResult> DownloadNodeAlarmEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.DownloadNodeAlarmByDate(model);
                var alarmTypes = await _mainDBContext.MultiAlarmTypes.ToListAsync();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Node Alarm Data");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "UT_Soy";
                    worksheet.Cell(currentRow, 4).Value = "Alarm Type";
                    worksheet.Cell(currentRow, 5).Value = "Server Received DateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.Soy;
                        worksheet.Cell(currentRow, 4).Value = alarmTypes.Where(x => x.Value == user.AlarmType).Select(x => x.Description).FirstOrDefault();
                        worksheet.Cell(currentRow, 5).Value = user.AddedDateTime;
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
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DownloadNodeAlarmEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("getValveAlarmData")]
        public async Task<IActionResult> GetValveAlarmData([FromQuery] ResourceParameter resourceParameter) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetValvesAlarmData(resourceParameter);
                var outputModel = new DtoMultiValveAlarmData
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveAlarmData)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getValveAlarmDataByDate")]
        public async Task<IActionResult> GetValveAlarmDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetValvesAlarmDataByDate(model);
                var outputModel = new DtoMultiValveAlarmData
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveAlarmDataByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadValveAlarmDataByDate")]
        public async Task<IActionResult> DownloadValveAlarmDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.DownloadValvesAlarmDataByDate(model);

                var multiReason = _mainDBContext.MultiValveReason.ToList();
                var multistate = _mainDBContext.MultiValveState.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ValveAlarmData");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "ValveNo";
                    worksheet.Cell(currentRow, 4).Value = "Valve Type";
                    worksheet.Cell(currentRow, 5).Value = "RequiredState";
                    worksheet.Cell(currentRow, 6).Value = "ReqState Reason";
                    worksheet.Cell(currentRow, 7).Value = "CurrentState";
                    worksheet.Cell(currentRow, 8).Value = "CurrentStateReason";
                    worksheet.Cell(currentRow, 9).Value = "Alarm Reason";
                    worksheet.Cell(currentRow, 10).Value = "Update Time Moy";
                    worksheet.Cell(currentRow, 11).Value = "UT Soy";
                    worksheet.Cell(currentRow, 12).Value = "Acv Current Consimption";
                    worksheet.Cell(currentRow, 13).Value = "AddedDateTime";

                    foreach (var user in multiValveEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 2).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 3).Value = user.ValveNo;
                        worksheet.Cell(currentRow, 4).Value = user.ValveType;
                        worksheet.Cell(currentRow, 5).Value = multistate.Where(x => x.Value == user.ReqState).Select(x => x.State).FirstOrDefault();
                        worksheet.Cell(currentRow, 6).Value = multiReason.Where(x => x.Value == user.ReqStateReason).Select(x => x.Reason).FirstOrDefault(); worksheet.Cell(currentRow, 8).Value = user.AddedDateTime;
                        worksheet.Cell(currentRow, 7).Value = multistate.Where(x => x.Value == user.CurrentState).Select(x => x.State).FirstOrDefault();
                        worksheet.Cell(currentRow, 8).Value = multiReason.Where(x => x.Value == user.CstateReason).Select(x => x.Reason).FirstOrDefault(); worksheet.Cell(currentRow, 8).Value = user.AddedDateTime;
                        worksheet.Cell(currentRow, 9).Value = user.AlarmReason;
                        worksheet.Cell(currentRow, 10).Value = user.UpdateTime;
                        worksheet.Cell(currentRow, 11).Value = user.Soy;
                        worksheet.Cell(currentRow, 12).Value = user.AcvcurrentConsimption;
                        worksheet.Cell(currentRow, 13).Value = user.AddedDateTime;

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
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("getSensorAlarmData")]
        public async Task<IActionResult> GetSensorAlarmData([FromQuery] ResourceParameter resourceParameter) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetSensorAlarm(resourceParameter);
                var outputModel = new DtoMultiSensorAlarmData
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetSensorAlarmData)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getSensorAlarmDataByDate")]
        public async Task<IActionResult> GetSensorAlarmDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetSensorAlarmByDate(model);
                var outputModel = new DtoMultiSensorAlarmData
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveAlarmDataByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadSensorAlarmDataByDate")]
        public async Task<IActionResult> DownloadSensorAlarmDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiEvents = await _valveService.DownloadSensorAlarmByDate(model);

                var multiReason = _mainDBContext.MultiSensorAlarmReason.ToList();
                var multiType = _mainDBContext.MultiSensorType.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("SensorAlarmData");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 2).Value = "NodeNo";
                    worksheet.Cell(currentRow, 3).Value = "Sensor No";
                    worksheet.Cell(currentRow, 4).Value = "UT_Soy";
                    worksheet.Cell(currentRow, 5).Value = "Sensor Type";
                    worksheet.Cell(currentRow, 6).Value = "Alarm Reason";
                    worksheet.Cell(currentRow, 7).Value = "Sensor Value";
                    worksheet.Cell(currentRow, 8).Value = "Current state";
                    worksheet.Cell(currentRow, 9).Value = "Default State";
                    worksheet.Cell(currentRow, 10).Value = "Cummulative Count";
                    worksheet.Cell(currentRow, 11).Value = "Frequency";
                    worksheet.Cell(currentRow, 12).Value = "Server Received DateTime";

                    foreach (var user in multiEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 2).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 3).Value = user.Soy;
                        worksheet.Cell(currentRow, 4).Value = user.SensorNo;
                        worksheet.Cell(currentRow, 5).Value = multiType.Where(x => x.Sstype == user.SensorType).Select(x => x.SensorDescription).FirstOrDefault();
                        worksheet.Cell(currentRow, 6).Value = multiReason.Where(x => x.Value == user.AlarmReason).Select(x => x.Description).FirstOrDefault(); ;
                        worksheet.Cell(currentRow, 7).Value = user.SensorValue;
                        worksheet.Cell(currentRow, 8).Value = user.Cstate;
                        worksheet.Cell(currentRow, 9).Value = user.DefaultState;
                        worksheet.Cell(currentRow, 10).Value = user.CummulativeCount;
                        worksheet.Cell(currentRow, 11).Value = user.Frequency;
                        worksheet.Cell(currentRow, 12).Value = user.AddedDateTime;
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
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("getNodeAlarmData")]
        public async Task<IActionResult> GetNodeAlarmData([FromQuery] ResourceParameter resourceParameter) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetNodeAlarm(resourceParameter);
                var outputModel = new DtoMultiNodeAlarm
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetNodeAlarmData)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getNodeAlarmDataByDate")]
        public async Task<IActionResult> GetNodeAlarmDataByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiValveEvents = await _valveService.GetNodeAlarmByDate(model);
                var outputModel = new DtoMultiNodeAlarm
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetNodeAlarmDataByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("getSensorEvents")]
        public async Task<IActionResult> GetSensorEvents([FromQuery] ResourceParameter resourceParameter) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multievents = await _valveService.GetAllSensorEventsMulti(resourceParameter);
                var outputModel = new DtoMultiSensorEvent
                {
                    Paging = multievents.GetHeader(),
                    Items = multievents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetSensorEvents)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("getSensorEventsByDate")]
        public async Task<IActionResult> GetSensorEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multievents = await _valveService.GetAllSensorEventsMultiByDate(model);
                var outputModel = new DtoMultiSensorEvent
                {
                    Paging = multievents.GetHeader(),
                    Items = multievents.ToList(),
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetSensorEvents)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DownloadSensorEventsByDate")]
        public async Task<IActionResult> DownloadSensorEventsByDate([FromBody] PostEvents model) //TODO: get list from UI
        {
            try
            {
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var multiEvents = await _valveService.DownloadensorEventsMultiByDate(model);

                var multiReason = _mainDBContext.MultiSensorAlarmReason.ToList();
                var multiType = _mainDBContext.MultiSensorType.ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("SensorEvents");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "ui_Store_SOY";
                    worksheet.Cell(currentRow, 2).Value = "NetworkNo";
                    worksheet.Cell(currentRow, 3).Value = "NodeNo";
                    worksheet.Cell(currentRow, 4).Value = "SensorNo";
                    worksheet.Cell(currentRow, 5).Value = "SensorType";
                    worksheet.Cell(currentRow, 6).Value = "Priority";
                    worksheet.Cell(currentRow, 7).Value = "uc_SAMP_Rate";
                    worksheet.Cell(currentRow, 8).Value = "ui_Last_ATM_SOY";
                    worksheet.Cell(currentRow, 9).Value = "ui_UT_SOY";
                    worksheet.Cell(currentRow, 10).Value = "AlarmReason";
                    worksheet.Cell(currentRow, 11).Value = "SensorValue";
                    worksheet.Cell(currentRow, 12).Value = "Cstate";
                    worksheet.Cell(currentRow, 13).Value = "DefaultState";
                    worksheet.Cell(currentRow, 14).Value = "CummulativeCount";
                    worksheet.Cell(currentRow, 15).Value = "Frequency";
                    worksheet.Cell(currentRow, 16).Value = "AddedDateTime";

                    foreach (var user in multiEvents)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = user.StoreSoy;
                        worksheet.Cell(currentRow, 2).Value = user.NetworkNo;
                        worksheet.Cell(currentRow, 3).Value = user.NodeNo;
                        worksheet.Cell(currentRow, 4).Value = user.Ssno;
                        worksheet.Cell(currentRow, 5).Value = multiType.Where(x => x.Sstype == user.Sstype).Select(x => x.SensorDescription).FirstOrDefault();
                        worksheet.Cell(currentRow, 6).Value = user.Sspriority;
                        worksheet.Cell(currentRow, 7).Value = user.Samprate;
                        worksheet.Cell(currentRow, 8).Value = user.LastAtmSoy;
                        worksheet.Cell(currentRow, 9).Value = user.Utsoy;
                        worksheet.Cell(currentRow, 10).Value = multiReason.Where(x => x.Value == user.AlarmReason).Select(x => x.Description).FirstOrDefault(); ;
                        worksheet.Cell(currentRow, 11).Value = user.SensorValue;
                        worksheet.Cell(currentRow, 12).Value = user.Cstate;
                        worksheet.Cell(currentRow, 13).Value = user.DefaultState;
                        worksheet.Cell(currentRow, 14).Value = user.CummulativeCount;
                        worksheet.Cell(currentRow, 15).Value = user.Frequency;
                        worksheet.Cell(currentRow, 16).Value = user.AddedDateTime;
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
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetValveEventsByDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }



        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiFrameTypes")]
        public async Task<IActionResult> GetMultiFrameTypes() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiFrameTypes> multievents = new List<MultiFrameTypes>();
                multievents = await _valveService.GetMultiFrameTypes();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiFrameTypes)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiAddonsTypes")]
        public async Task<IActionResult> GetMultiAddonsTypes() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiAddonCardTypes> multievents = new List<MultiAddonCardTypes>();
                multievents = await _valveService.GetMultiAddonCardType();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiAddonsTypes)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiValveTypes")]
        public async Task<IActionResult> GetMultiValveTypes() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiValveType> multievents = new List<MultiValveType>();
                multievents = await _valveService.GetMultiValveType();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiFrameTypes)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiSensorAlarmReason")]
        public async Task<IActionResult> GetMultiSensorAlarmReason() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiSensorAlarmReason> multievents = new List<MultiSensorAlarmReason>();
                multievents = await _valveService.GetMultiSensorAlarmReason();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiSensorAlarmReason)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiValveAlarmReason")]
        public async Task<IActionResult> GetMultiValveAlarmReason() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiValveAlarmReason> multievents = new List<MultiValveAlarmReason>();
                multievents = await _valveService.GetMultiValveAlarmReason();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiValveAlarmReason)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiSensorTypes")]
        public async Task<IActionResult> GetMultiSensorTypes() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiSensorType> multievents = new List<MultiSensorType>();
                multievents = await _valveService.GetMultiSensorType();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiSensorTypes)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiAlarmTypes")]
        public async Task<IActionResult> GetMultiAlarmTypes() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiAlarmTypes> multievents = new List<MultiAlarmTypes>();
                multievents = await _valveService.GetMultiAlarmTypes();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiAlarmTypes)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiValveReason")]
        public async Task<IActionResult> GetMultiValveReason() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiValveReason> multievents = new List<MultiValveReason>();
                multievents = await _valveService.GetMultiValveReason();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiValveReason)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetMultiValveState")]
        public async Task<IActionResult> GetMultiValveState() //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<MultiValveState> multievents = new List<MultiValveState>();
                multievents = await _valveService.GetMultiValveState();
                return Ok(multievents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetMultiValveState)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("deletesinglehorizontalelement")]
        public async Task<IActionResult> DeleteSingleHoriElement([FromBody] DeleteSeqElementViewModel model) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                await _valveService.DeleteAllSingleHoriElement(model);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, null, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DeleteSingleHoriElement)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("deletesingleverticleelement")]
        public async Task<IActionResult> DeleteSingleVerticleElement([FromBody] DeleteVerticleElementViewModel model) //TODO: get list from UI
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                await _valveService.DeleteSingleVerticleElement(model);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, null, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(DeleteSingleVerticleElement)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] List<SequenceValveConfigViewModel> models) //TODO: get list from UI
        {
            try
            {
                string strResult = string.Empty;
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                // validate model
                if (!await IsModelValid(models))
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", ModelState, 5));

                TimeSpan dayStart = new TimeSpan();
                TimeSpan dayEnd = new TimeSpan();
                int seqId = (int)models[0].SeqId;
                var sequence = await _sequenceService.GetSequenceById(seqId);
                var zones = await _zoneService.GetZoneByTypeAndId("All", sequence.NetworkId);

                zones = zones.Where(x => x.PrjId == sequence.PrjId && x.ZoneId == sequence.ZoneId).ToList();
                if (zones.Count > 0)
                {
                    var zone = zones.FirstOrDefault();
                    if (!string.IsNullOrEmpty(zone.DayStartTime))
                    {
                        string[] dayst = zone.DayStartTime.Trim().Split(':');
                        if (dayst.Length == 2)
                        {
                            dayStart = new TimeSpan(Convert.ToInt32(dayst[0]), Convert.ToInt32(dayst[1]), 0);
                            dayEnd = dayStart.Add(TimeSpan.FromHours(23));
                            dayEnd = dayEnd.Add(TimeSpan.FromMinutes(59));
                        }
                    }
                }
                // get sequence master config
                var sequenceMasterConfig = sequence.SequenceMasterConfig.OrderBy(x => x.StartId).FirstOrDefault();
                var sequenceValveConfig = sequence.SequenceValveConfig.ToList();
                foreach (var valve in models)
                {
                    valve.SeqId = sequence.SeqId;
                    //valve.ScheduleNo = sequence. //TODO
                    valve.MstseqId = sequenceMasterConfig.MstseqId;
                    valve.StartId = sequenceMasterConfig.StartId;
                    var startTime = sequenceMasterConfig.StartTime.Split(':');
                    var seqStartTime = new TimeSpan(Convert.ToInt32(startTime[0]), Convert.ToInt32(startTime[1]), 0);
                    // for new horizontal id = 1 (ie. no valve config for this sequence)
                    // or horizontal id = 1 and isHorizontal = false means vartical valves of first horizontal
                    if (sequenceValveConfig.Count == 0 || (valve.HorizGrId == 1 && valve.IsHorizontal == false))
                    {
                        valve.ValveStartTime = seqStartTime.Hours.ToString("00") + ":" + seqStartTime.Minutes.ToString("00");
                    }
                    else
                    {
                        // for other than first horizontal id
                        var prevHorizValve = sequenceValveConfig
                            .Where(x => x.HorizGrId == (valve.HorizGrId - 1))
                            .FirstOrDefault();
                        if (prevHorizValve != null)
                        {
                            string[] preValveStartTime = prevHorizValve.ValveStartTime.Split(':');
                            TimeSpan preValveStartTimeTSpan = new TimeSpan(Convert.ToInt32(preValveStartTime[0]), Convert.ToInt32(preValveStartTime[1]), 0);
                            string[] preValveStartDuration = prevHorizValve.ValveStartDuration.Split(':');
                            TimeSpan preValeStartDurationTSpan = new TimeSpan(Convert.ToInt32(preValveStartDuration[0]), Convert.ToInt32(preValveStartDuration[1]), 0);
                            preValveStartTimeTSpan = preValveStartTimeTSpan.Add(preValeStartDurationTSpan);
                            valve.ValveStartTime = preValveStartTimeTSpan.Hours.ToString("00") + ":" + preValveStartTimeTSpan.Minutes.ToString("00");
                        }
                    }
                }
                //TODO:
                //3508 - save function call SaveSequenceDetails 
                string[] valveStartTimeArray = models[0].ValveStartTime.Split(':');
                var valveStartTime = new TimeSpan(Convert.ToInt32(valveStartTimeArray[0]), Convert.ToInt32(valveStartTimeArray[1]), 0);
                string[] valveDurationArray = models[0].ValveStartDuration.Split(':');
                TimeSpan valveDuration = new TimeSpan(Convert.ToInt32(valveDurationArray[0]), Convert.ToInt32(valveDurationArray[1]), 0);
                var valveEndTime = valveStartTime.Add(valveDuration);
                if (!ValidateDayStartTime(dayStart, dayEnd, valveStartTime, valveEndTime))
                {
                    ModelState.AddModelError("Error", "Day end has reached. You cannot add new duration now.");
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", ModelState, 6));
                }
                // validate                
                if (!ModelState.IsValid)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", ModelState, 6));
                }

                strResult = await _valveService.Add(models, sequence, valveStartTime, valveEndTime);

                return Ok(CustomResponse.CreateResponse(true, strResult, strResult, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(Post)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Update Master Start time
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpdateMasterValveStartTime")]
        public async Task<ActionResult> UpdateMasterValveStartTime([FromBody] UpdateMasterValveStartTimeViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<string> resultText = await _valveService.UpdateMasterValveStartTime(model);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, resultText, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{nameof(UpdateMasterValveStartTime)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Update Valve Duration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("UpdateSingleValveDuration")]
        public async Task<ActionResult> UpdateSingleValveDuration([FromBody] UpdateSingleValveDurationModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<string> resultText = await _valveService.UpdateSingleValveDuration(model);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, resultText, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{nameof(UpdateSingleValveDuration)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        #region private methods

        /// <summary>
        /// validate sequence model
        /// </summary>
        /// <param name="model">SequenceViewModel</param>
        /// <returns>error string</returns>
        private async Task<bool> IsModelValid(List<SequenceValveConfigViewModel> models)
        {
            if (models.Count == 0)
            {
                ModelState.AddModelError("", "Model cannot be null");
                return false;
            }
            foreach (var model in models)
            {
                if (model == null)
                {
                    ModelState.AddModelError("", "Model cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    return false;
                }
            }
            if (models.Any(x => x.HorizGrId > GlobalConstants.NoOfHorizontalCols))
            {
                ModelState.AddModelError("Error", "Number of Max Horizontal element should not be greater than " + GlobalConstants.NoOfHorizontalCols);
            }
            int seqId = (int)models[0].SeqId;
            var totalValvesInSeq = await _valveService.GetValveCountBySeqId(seqId);
            if (totalValvesInSeq > GlobalConstants.MaxValvesDisplayedInOneSequence)
            {
                ModelState.AddModelError("Error", "Max valve allowed to configure in sequence is " + GlobalConstants.MaxValvesDisplayedInOneSequence);
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


        //TODO:PA
        //Also in Service....better to use service call
        /// <summary>
        /// validate valve day start time
        /// </summary>
        /// <param name="dayStart"></param>
        /// <param name="dayEnd"></param>
        /// <param name="valveStart"></param>
        /// <param name="valveEnd"></param>
        /// <returns></returns>
        private bool ValidateDayStartTime(TimeSpan dayStart, TimeSpan dayEnd, TimeSpan valveStart, TimeSpan valveEnd)
        {
            if (valveStart < dayStart && valveEnd <= dayStart)
            {
                return true;
            }
            else if (valveStart >= dayStart && valveEnd >= dayStart && valveEnd <= dayEnd)
            {
                return true;
            }
            else if (valveStart >= dayStart && valveEnd <= dayEnd)
            {
                return true;
            }
            else if (valveStart < dayStart && valveEnd > dayStart)
            {
                return false;
            }
            return false;
        }


        #endregion
    }
}
