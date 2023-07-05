using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.GlobalEntities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using Scheduling.ViewModels.OutputModels;
using Scheduling.ViewModels.ResourceParamaters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    /// <summary>
    /// FrameController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FrameController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        //Logger
        private readonly ILogger<FrameController> _logger;
        //Main DB Context
        private MainDBContext _mainDBContext;
        //Global DB Contect
        private GlobalDBContext _globalDBContext;
        //Frame Service
        private IFramesService _framesService;
        //Constructor
        public FrameController(
           MainDBContext mainDBContext, GlobalDBContext globalDBContext,
           IWebHostEnvironment webHostEnvironment,
           ILogger<FrameController> logger, IFramesService framesService
        )
        {
            _framesService = framesService;
            _logger = logger;
            _mainDBContext = mainDBContext;
            _globalDBContext = globalDBContext;
            _webHostEnvironment = webHostEnvironment;
        }
        #region Old Code
        //// POST api/<controller>
        //[HttpPost("Joining")]
        //public async Task<HeartBeatFrame> Post([FromBody] JoiningFrameViewModel model)
        //{
        //    // {"HEAD":["JN",4434,1,2,0,100]} 
        //    try
        //    {
        //        _logger.LogInformation(DateTime.Now + "==" + JsonConvert.SerializeObject(model));
        //        int GWID = Convert.ToInt32(model.HEAD[1]);
        //        int PROJECT_ID = 10001;
        //        int DB_ID = 5;
        //        string FrameType = "HB";

        //        int CMD_NO = Convert.ToInt32(model.HEAD[2]);
        //        int NWT_UPID = Convert.ToInt32(model.HEAD[4]);
        //        int TOTAL_NODE_IN_PROJECT = 0;
        //        int PROGRAM_Day_END_MOD = 0;
        //        int YEAR = DateTime.Now.Year;
        //        int MONTH = DateTime.Now.Month;
        //        int DATE = DateTime.Now.Day;
        //        int HH = DateTime.Now.Hour;
        //        int MM = DateTime.Now.Minute;
        //        int SS = DateTime.Now.Second;
        //        int WEEK_DA = (int)DateTime.Now.DayOfWeek;
        //        TOTAL_NODE_IN_PROJECT = await _mainDBContext.ProjectConfiguration.Select(x => (int)x.MaxNodeInProject).FirstOrDefaultAsync();

        //        HeartBeatFrame heartBeatFrame = new HeartBeatFrame();
        //        List<dynamic> HEAD = new List<dynamic>();
        //        //BODYBlank bODYBlank = new BODYBlank();
        //        HEAD.Add(GWID);
        //        HEAD.Add(PROJECT_ID);
        //        HEAD.Add(DB_ID);
        //        HEAD.Add(FrameType);
        //        HEAD.Add(CMD_NO);
        //        HEAD.Add(NWT_UPID);
        //        HEAD.Add(TOTAL_NODE_IN_PROJECT);
        //        HEAD.Add(PROGRAM_Day_END_MOD);
        //        HEAD.Add(YEAR);
        //        HEAD.Add(MONTH);
        //        HEAD.Add(DATE);
        //        HEAD.Add(HH);
        //        HEAD.Add(MM);
        //        HEAD.Add(SS);
        //        HEAD.Add(WEEK_DA);
        //        heartBeatFrame.HEAD = HEAD;
        //        // heartBeatFrame.BODY = bODYBlank;
        //        return heartBeatFrame;
        //        // return Ok(CustomResponse.CreateResponse(true, "", "", 0));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"[{ nameof(FilterController) }.{ nameof(Post) }]{ ex }");
        //        return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong with your request.", null, 1));
        //    }
        //} 
        #endregion


        /// <summary>
        /// Get sequence valves and groups to configure valve element
        /// </summary>
        /// <param name="seqType"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <returns>SeqValveGroupViewModel</returns>
        [HttpGet("GetDatalogger")]
        [Authorize]
        public async Task<IActionResult> GetGetDatalogger([FromQuery] ResourceParameter resourceParameter)
        {
            try
            {   //Set Site Name
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //DbManager.SiteName = "JainPort76";
                //Get MultiData Logger
                var model = await _framesService.GetMultiDataLogger(resourceParameter);
                //var gwLists = await _framesService.GetGatewayListForDataLogger();
                var outputModel = new DtoMultiDataLogger
                {
                    Paging = model.GetHeader(),
                    Items = model.ToList(),
                    //GwItems = gwLists.ToList()
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{nameof(GetGetDatalogger)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }
        /// <summary>
        /// DeleteDataLogger
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        // //[Authorize(Policy = "Permissions.Operations.Estimate List.ReadOnly,Permissions.Operations.Estimate List.AddUpdateDelete")]
        public async Task<IActionResult> DeleteDataLogger(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
          
                    _framesService.DeletedataLog(id);
                    return Ok();               
            }
            catch (Exception ex)
            {
                string error = $@"[{nameof(FrameController)}.{nameof(FileDownload)}] 
                    Exception = {ex}
                    loggedin user = {User.Identity.Name}
                    Http Request Details:
                    id = {id}";
                _logger.LogError(error);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// DeleteDataLogger
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteDatalogger")]
        [Authorize]
        // //[Authorize(Policy = "Permissions.Operations.Estimate List.ReadOnly,Permissions.Operations.Estimate List.AddUpdateDelete")]
        public async Task<IActionResult> DeleteAllDataLogger()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                _framesService.DeleteAlldataLog();
                return Ok();
            }
            catch (Exception ex)
            {   _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("filedownload")]
        [Authorize]
        // //[Authorize(Policy = "Permissions.Operations.Estimate List.ReadOnly,Permissions.Operations.Estimate List.AddUpdateDelete")]
        public async Task<IActionResult> FileDownload(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                MultiDataLogger fileAttachment = await _mainDBContext.MultiDataLogger.Where(x => x.Id == id).FirstOrDefaultAsync();
                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Multi_DataLogger", DbManager.SiteName, fileAttachment.Message);

                DriveInfo cDrive = new DriveInfo(System.Environment.CurrentDirectory);

                //_logger.LogInformation(System.AppDomain.CurrentDomain.BaseDirectory);
                if (fileAttachment != null)
                {
                    // _logger.LogInformation(filePath);
                    string filenm = Path.GetFileName(fileAttachment.Message);
                    // get content type of the attachment
                    var fileProvider = new FileExtensionContentTypeProvider();
                    if (!fileProvider.TryGetContentType(filePath, out string contentType))
                    {
                        throw new ArgumentOutOfRangeException($"Unable to find Content Type for file name {filenm}.");
                    }
                    return PhysicalFile(filePath, contentType, filenm);
                }
                else
                {
                    ModelState.AddModelError("Error", "File not found");
                    return BadRequest(new CustomBadRequest(ModelState));
                }
            }
            catch (Exception ex)
            {
                string error = $@"[{nameof(FrameController)}.{nameof(FileDownload)}] 
                    Exception = {ex}
                    loggedin user = {User.Identity.Name}
                    Http Request Details:
                    id = {id}";
                _logger.LogError(error);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("filedownloadall")]
        [Authorize]
        // //[Authorize(Policy = "Permissions.Operations.Estimate List.ReadOnly,Permissions.Operations.Estimate List.AddUpdateDelete")]
        public async Task<IActionResult> FileDownloadAll()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                string pathFolder = "DataLoggerOutput";
                string ATTACHMENTS_FOLDER_PATH = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder);
                string filename = "Datalog_Op.txt";
                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder, filename);

                //_logger.LogInformation(System.AppDomain.CurrentDomain.BaseDirectory);
                if (filePath != null)
                {
                    // _logger.LogInformation(filePath);
                    string filenm = Path.GetFileName(filename);
                    // get content type of the attachment
                    var fileProvider = new FileExtensionContentTypeProvider();
                    if (!fileProvider.TryGetContentType(filePath, out string contentType))
                    {
                        throw new ArgumentOutOfRangeException($"Unable to find Content Type for file name {filenm}.");
                    }
                    return PhysicalFile(filePath, contentType, filenm);
                }
                else
                {
                    ModelState.AddModelError("Error", "File not found");
                    return BadRequest(new CustomBadRequest(ModelState));
                }
            }
            catch (Exception ex)
            {
                string error = $@"[{nameof(FrameController)}.{nameof(FileDownload)}] 
                    Exception = {ex}
                    loggedin user = {User.Identity.Name}
                    Http Request Details:
                    ";
                _logger.LogError(error);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Data logger by Date
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("GetDataloggerbyDate")]
        public async Task<IActionResult> GetDataloggerbyDate([FromBody] PostEventsDataLlogger model) //TODO: get list from UI
        {
            try
            {    //Set Site Name
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //Check date and return
                if (model.StartDateTime > model.EndDateTime)
                {
                    return BadRequest("To Date should be smaller than start date");
                }
                //Return response
                var multiValveEvents = await _framesService.GetDataLoggerByDate(model);
                // var gwLists = await _framesService.GetGatewayListForDataLogger();
                var outputModel = new DtoMultiDataLogger
                {
                    Paging = multiValveEvents.GetHeader(),
                    Items = multiValveEvents.ToList(),
                    //  GwItems = gwLists.ToList()
                };
                return Ok(outputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveController)}.{nameof(GetDataloggerbyDate)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Datalogger
        /// </summary>
        /// <returns></returns>
        [HttpPost("Datalogger")]
        public async Task<ActionResult> DataLoggger()
        {
            try
            {
                //DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //string projectName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //Read Posted Data
                string content = await new StreamReader(Request.Body).ReadToEndAsync();
                //Split frame by DL
                var item = content.Split("DL");
                // content = "ST0001000027110000000100010100C8DLfnjn32546j84ghfuhb hguvh5; gm5";
                //Get ProjectId
                int ProjectId = HexToDecimal(item[0].Substring(6, 8));
                //Get project gateway mapping
                ProjectGatewayMapping projectGatewayMapping = _globalDBContext.ProjectGatewayMapping.Where(x => x.ProjectId == ProjectId).FirstOrDefault();
                //Set Sitename
                DbManager.SiteName = projectGatewayMapping.ProjectName;
                //Save Data logger
                string result = await _framesService.SaveDataLogger(content);
                GWToSWNotificationVIewModel body = new GWToSWNotificationVIewModel();
                // _logger.LogInformation("==" + JsonConvert.SerializeObject(heartBeatFrame));
                //Get Notification response and return
                var notificationResponse = await _framesService.GetNotificationResponse(body, content.Split("DL")[0], 0, (int)projectGatewayMapping.ProjectId);
                _logger.LogInformation("Server Response==" + JsonConvert.SerializeObject((object)notificationResponse));
                return Ok(notificationResponse);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //int HexToDecimal(string sHexValue)
        //{
        //    int iNumber = int.Parse(sHexValue, System.Globalization.NumberStyles.HexNumber);
        //    return iNumber;
        //}
        /// <summary>
        /// Notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Notification([FromBody] GWToSWNotificationVIewModel model)
        {
            /*       {
                        "head": ["AN", 2, 4, 1, 1211, 11, 2, 100],
                        "body": {
                                    "APP_UPID": [],
                                    "NODE_UPID": ["0401, 2,2, 2, 2","0402, 2, 2, 2, 2"],
                                    "SQ_UPID": []
                                 }
                       }
            ---------------------------------------------------------------------------------------------------------------------------------------------------------
             {
                        "head": ["AN", 2, 4, 1, 1211, 11, 2, 100],
                        "body": {
                                    "Data": ["X190003000614FF758501010000000C3B79060000000000X"]                                   
                                 }
                       }

            //Multi node node nw data 
              {
                        "head": ["FS", 2, 4, 1, 1211, 11, 2, 100],
                        "body": {
                                    "Data": ["X27000400050002020103BEA13D0000000000C8A13D00010000904200000000000000000000X"]                                   
                                 }
                       }
            ==========================================================================================================================================================

            {
	"head": ["AN", 1, 4, 1, 1211, 11, 2, 35000],
	"body": {
		"APP_UPID": [],
		"NODE_UPID": ["0801,00,00,00,00", "0801,00,00,00,00", "0803,00,00,00,00", "0804,00,00,00,00", "0805,00,00,00,00", "0806,00,00,00,00", "0807,00,00,00,00", "0808,00,00,00,00", "0809,00,00,00,00", "080A,00,00,00,00", "080B,00,00,00,00", "080C,00,00,00,00", "080D,00,00,00,00", "080E,00,00,00,00", "080F,00,00,00,00", "0810,00,00,00,00", "0811,00,00,00,00", "0812,00,00,00,00", "0813,00,00,00,00", "0814,00,00,00,00",  "0415,00,00,00,00","0816,00,00,00,00", "0817,00,00,00,00", "0818,00,00,00,00", "0819,00,00,00,00", "081A,00,00,00,00", "081B,00,00,00,00", "081C,00,00,00,00","081D,00,00,00,00","081E,00,00,00,00","081F,00,00,00,00","0820,00,00,00,00","0821,00,00,00,00","0822,00,00,00,00","0823,00,00,00,00","0824,00,00,00,00","0825,00,00,00,00","0826,00,00,00,00","0827,00,00,00,00","0828,00,00,00,00","0829,00,00,00,00","082A,00,00,00,00","082B,00,00,00,00","082C,00,00,00,00","082D,00,00,00,00","082E,00,00,00,00","082F,00,00,00,00","0830,00,00,00,00","0831,00,00,00,00","0832,00,00,00,00"],
		"SQ_UPID": [],
		"MAIN_SCH_UPID":1
		
	}
}//["01040101290010200c17b40f3301010108000000000102000000000101010500000200f01E002800","02040102290010201010b40f330101010800000000010200000000010101050f000200f01E00280028000a013c"],"VRT":
            //["JN",1,2,1,0,200],"BODY":{"APP_UPID":[],"NODE_UPID":[],"SQ_UPID":[],"MAIN_SCH_UPID":0,"DATA":[]}}
            */
            try
            {
                //  DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                _logger.LogInformation("Step 0--" + DateTime.Now);
                //Gateway ID
                int GWID = 0;
                //Project ID
                int PROJECT_ID = 4;
                //DB Id
                int DB_ID = 1;
                //CMD No
                int CMD_NO = 0;
                //AtiveMIN
                int ACTIVEMIN = 0;
                //NetworkUPID
                int NWT_UPID = 0;
                _logger.LogInformation("Requested Model==" + JsonConvert.SerializeObject(model));
                //Frame Type
                string frameType = model.HEAD[0].ToString();
                if (frameType == "JN")
                {   //Joining Frame
                    //Gatway serial No
                    int GwSrnNo = Convert.ToInt32(model.HEAD[1]);
                    //Project Gateway Mapping
                    ProjectGatewayMapping projectGatewayMapping = _globalDBContext.ProjectGatewayMapping.Where(x => x.GatewaySrNo == GwSrnNo).FirstOrDefault();
                    //Site Name
                    DbManager.SiteName = projectGatewayMapping.ProjectName;
                    //Project Id
                    PROJECT_ID = (int)projectGatewayMapping.ProjectId;
                    //Get Gateway information by serial no
                    Gateway gw = await _framesService.GetGatewayBySrNo(GwSrnNo);
                    //Gateway id
                    GWID = Convert.ToInt32(gw.GatewayNo);
                    //Command no
                    CMD_NO = Convert.ToInt32(model.HEAD[2]);
                    //AtiveMIN
                    ACTIVEMIN = Convert.ToInt32(model.HEAD[3]);
                    //Get Upids project
                    UpdateIdsProject proj = await _framesService.GetUpdateIdsProject();
                    //Network Upid
                    NWT_UPID = Convert.ToInt32(proj.ProjectUpId);
                }
                else
                {   //Gateway id
                    GWID = Convert.ToInt32(model.HEAD[1]);
                    //Project Id
                    PROJECT_ID = Convert.ToInt32(model.HEAD[2]);
                    //Project Gateway Mapping
                    ProjectGatewayMapping projectGatewayMapping = _globalDBContext.ProjectGatewayMapping.Where(x => x.ProjectId == PROJECT_ID).FirstOrDefault();
                    //Site Name
                    DbManager.SiteName = projectGatewayMapping.ProjectName;
                    //DBId
                    DB_ID = Convert.ToInt32(model.HEAD[3]);
                    //CMD No
                    CMD_NO = Convert.ToInt32(model.HEAD[4]);
                    //Active Min
                    ACTIVEMIN = Convert.ToInt32(model.HEAD[5]);
                    //Netwrok upid
                    NWT_UPID = Convert.ToInt32(model.HEAD[6]);
                }
                //Frame Type HB
                string FrameType = "HB";
                //TOTAL_NODE_IN_PROJECT 
                int TOTAL_NODE_IN_PROJECT = 0;
                //PROGRAM_Day_END_MOD
                int PROGRAM_Day_END_MOD = 0;
                int YEAR = Convert.ToInt32(DateTime.Now.ToString("yy"));
                int MONTH = DateTime.Now.Month;
                int DATE = DateTime.Now.Day;
                int HH = DateTime.Now.Hour;
                int MM = DateTime.Now.Minute;
                int SS = DateTime.Now.Second;
                int WEEK_DA = (int)DateTime.Now.DayOfWeek;
                TOTAL_NODE_IN_PROJECT = await _mainDBContext.ProjectConfiguration.Select(x => (int)x.MaxNodeInProject).FirstOrDefaultAsync();
                //Create Heartbeat headers
                HeartBeatFrame heartBeatFrame = new HeartBeatFrame();
                List<dynamic> HEAD = new List<dynamic>();
                BODYBlank bODYBlank = new BODYBlank();
                HEAD.Add(FrameType);
                HEAD.Add(GWID);
                HEAD.Add(PROJECT_ID);
                HEAD.Add(DB_ID);
                HEAD.Add(CMD_NO);
                HEAD.Add(NWT_UPID);
                HEAD.Add(TOTAL_NODE_IN_PROJECT);
                HEAD.Add(PROGRAM_Day_END_MOD);
                HEAD.Add(YEAR);
                HEAD.Add(MONTH);
                HEAD.Add(DATE);
                HEAD.Add(HH);
                HEAD.Add(MM);
                HEAD.Add(SS);
                HEAD.Add(WEEK_DA);
                heartBeatFrame.HEAD = HEAD;
                heartBeatFrame.BODY = bODYBlank;
                //Frame Type FS
                if (frameType == "FS")
                {
                    //IF ther is Data in Body 
                    if (model.BODY.DATA.Count > 0)
                    {   //Save events
                        string success = await _framesService.SaveEvents(model.BODY.DATA, GWID);

                    }
                    // _logger.LogInformation("==" + JsonConvert.SerializeObject(heartBeatFrame));
                    //Generate notification
                    var notificationResponse = await _framesService.GetNotificationResponse(model, "", 1, PROJECT_ID);
                    _logger.LogInformation("Server Response==" + JsonConvert.SerializeObject((object)notificationResponse));
                    //return response
                    return Ok(notificationResponse);

                }
                else if (frameType == "JN")
                {
                    //Frmae type JN
                    // heartBeatFrame.BODY = bODYBlank;
                    //_logger.LogInformation("==" + JsonConvert.SerializeObject(heartBeatFrame));
                    //return Ok(heartBeatFrame);
                    //Generate Notification
                    var notificationResponse = await _framesService.GetNotificationResponse(model, "", 1, PROJECT_ID);
                    _logger.LogInformation("Server Response ==" + JsonConvert.SerializeObject((object)notificationResponse));
                    //return response
                    return Ok(notificationResponse);
                }
                else
                {
                    //if (model.BODY.NODE_UPID.Count > 0)
                    //{
                    //Generate Notification
                    var notificationResponse = await _framesService.GetNotificationResponse(model, "", 1, PROJECT_ID);
                    _logger.LogInformation("Server Response==" + JsonConvert.SerializeObject((object)notificationResponse));
                    //return response
                    return Ok(notificationResponse);
                    //}
                    //else
                    //{
                    //    _logger.LogInformation("==" + JsonConvert.SerializeObject(heartBeatFrame));
                    //    return Ok(heartBeatFrame);
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FrameController)}.{nameof(Notification)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Error! GatewayId might not be configured or there is error in configuration.", null, 1));
            }
        }
        /// <summary>
        /// HexToDecimal
        /// </summary>
        /// <param name="sHexValue"></param>
        /// <returns></returns>
        private int HexToDecimal(string sHexValue)
        {
            int iNumber = int.Parse(sHexValue, System.Globalization.NumberStyles.HexNumber);
            return iNumber;
        }
        /// <summary>
        /// DecimalTo2Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo2Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(2, '0'); ;
        }
        /// <summary>
        /// DecimalTo4Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo4Hex(int sDecValue)
        {
            return sDecValue.ToString("X").PadLeft(4, '0');
        }
    }
}
