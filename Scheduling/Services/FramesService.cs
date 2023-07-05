using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using Scheduling.ViewModels.Lib;
using Scheduling.ViewModels.ResourceParamaters;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IFramesService
    {
        bool DeletedataLog(int id);

        bool DeleteAlldataLog();
        //Get Notification response 
        Task<dynamic> GetNotificationResponse(GWToSWNotificationVIewModel model, string datalogger, int flag, int projectId);
        //Get Project updateids
        Task<UpdateIdsProject> GetUpdateIdsProject();
        //Get Gateway serial No
        Task<Gateway> GetGatewayBySrNo(int serialo);
        //Get Multi Datalogger
        Task<PagedList<MultiDataLogger>> GetMultiDataLogger(ResourceParameter resourceParameter);
        //Save Events
        Task<string> SaveEvents(List<dynamic> model, int GatewayId);
        //Save Data Logger
        Task<string> SaveDataLogger(string model);
        //GetNodeNetworkDataFrameByDate
        Task<PagedList<MultiDataLogger>> GetDataLoggerByDate(PostEventsDataLlogger model);
        Task<IQueryable<GatewayDDLViewModel>> GetGatewayListForDataLogger();
    }
    public class FramesService : IFramesService
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<FramesService> _logger;
        private IZoneTimeService _zoneTimeService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FramesService(ILogger<FramesService> logger,
               MainDBContext mainDBContext,
               IMapper mapper,
               IZoneTimeService zoneTimeService, IConfiguration config, IWebHostEnvironment webHostEnvironment
           )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
            _zoneTimeService = zoneTimeService;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<UpdateIdsProject> GetUpdateIdsProject()
        {
            try
            {
                var projUpdis = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                return projUpdis;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FramesService)}.{nameof(GetUpdateIdsProject)}]{ex}");
                throw ex;
            }

        }

        /// <summary>
        /// GetRechargableNodeString
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ConfigUid"></param>
        /// <param name="nodeNo"></param>
        /// <param name="nwno"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> GetRechargableNodeString(int nodeId, int ConfigUid, int nodeNo, int nwno, List<RechableNode> model)
        {
            try
            {
                List<dynamic> csarray = new List<dynamic>();
                string joined = "";
                RechableNode rechableNode = model.Where(x => x.NodeId == nodeId && x.GwSrn == nwno).OrderBy(x => x.GwSrn).ThenBy(x => x.NodeId).FirstOrDefault(); /// await _mainDBContext.RechableNode
                if (rechableNode != null)
                {
                    csarray = new List<dynamic>();
                    //Node ID
                    csarray.Add(LittleEndian4X(nodeNo));
                    //UPID
                    csarray.Add(DecimalTo2Hex(ConfigUid));
                    //Product Type
                    csarray.Add(DecimalTo2Hex(2));
                    csarray.Add(LittleEndian4X((int)rechableNode.ThresholdforOcsc));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MinConnInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MaxConnInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MinAdvInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MaxAdvInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MtusizeValue));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.HandshkInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.Pulsedelayvalue));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.OperAttempt));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.AttemptForWaterFlow));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.LongSleepHndshkIntervalMf));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.Bttxpower));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.FixLoraSf));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.FixLoraPower));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.FixLoraFreq));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.FixLoraCr));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.PreferredGw1id));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.PreferredGw2id));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.PreferredGw3id));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.PreferredGw4id));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.AwfdetectEndis));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.FixLoraSetting));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.AutoSendStatusEnableBit));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.PowerLoopLatchEnable));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.GlobalAlarmEnDs));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.MaxLoRaCommAtt));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.SensorAlarmEndis));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.LoRaRxWindowMasking));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.DummyByte3Lsb));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.DummyByte3Msb));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.SaftetyTimeoutMin));
                    csarray.Add(LittleEndian4X((int)rechableNode.SlowDownCommDurMin));
                    csarray.Add(LittleEndian4X((int)rechableNode.ForceDeepSleepDurMin));
                    csarray.Add(LittleEndian4X((int)rechableNode.AutoSensorStatusSendInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.SamplingTimeInterval));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.ApplicationEnbits));
                    csarray.Add(DecimalTo2Hex((int)rechableNode.TimeForNocommWithGw));
                    joined = string.Join("", new List<dynamic>(csarray).ToArray());
                }

                return joined;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FramesService)}.{nameof(GetRechargableNodeString)}]{ex}");
                return "";
            }

        }
        /// <summary>
        /// GetNonRechargableString
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ConfigUid"></param>
        /// <param name="nodeNo"></param>
        /// <param name="nwno"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> GetNonRechargableString(int nodeId, int ConfigUid, int nodeNo, int nwno, List<NonRechableNode> model)
        {
            try
            {
                List<dynamic> csarray = new List<dynamic>();
                string joined = "";
                //Get Setting
                NonRechableNode nonRechableNode1 = model.Where(x => x.NodeId == nodeId && x.GwSrn == nwno).OrderBy(x => x.GwSrn).ThenBy(x => x.NodeId).FirstOrDefault(); // await _mainDBContext.NonRechableNode.Where(x => x.NodeId == nodeId && x.GwSrn == nwno).OrderBy(x => x.GwSrn).ThenBy(x => x.NodeId).FirstOrDefaultAsync();
                if (nonRechableNode1 != null)
                {
                    NonRechableNode nonRechableNode = nonRechableNode1;
                    csarray = new List<dynamic>();
                    //Node ID
                    csarray.Add(LittleEndian4X(nodeNo));
                    //UPID
                    csarray.Add(DecimalTo2Hex(ConfigUid));
                    //Product Type
                    csarray.Add(DecimalTo2Hex(1));
                    csarray.Add(LittleEndian4X((int)nonRechableNode.ThresholdforOcsc));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MinConnInterval));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MaxConnInterval));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MinAdvInterval));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MaxAdvInterval));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MtusizeValue));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.HandshkInterval));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.Pulsedelayvalue));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.OperAttempt));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.AttemptForWaterFlow));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.LongSleepHndshkIntervalMf));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.Bttxpower));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.FixLoraSf));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.FixLoraPower));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.FixLoraFreq));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.FixLoraCr));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.PreferredGw1id));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.PreferredGw2id));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.PreferredGw3id));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.PreferredGw4id));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.AwfdetectEndis));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.FixLoraSetting));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.AutoSendStatusEnableBit));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.PowerLoopLatchEnable));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.GlobalAlarmEnDs));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.MaxLoRaCommAtt));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.SensorAlarmEndis));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.LoRaRxWindowMasking));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.DummyByte3Lsb));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.DummyByte3Msb));
                    csarray.Add(DecimalTo2Hex((int)nonRechableNode.SaftetyTimeoutMin));
                    csarray.Add(LittleEndian4X((int)nonRechableNode.SlowDownCommDurMin));
                    csarray.Add(LittleEndian4X((int)nonRechableNode.ForceDeepSleepDurMin));
                    joined = string.Join("", new List<dynamic>(csarray).ToArray());

                }
                return joined;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FramesService)}.{nameof(GetNonRechargableString)}]{ex}");
                return "";
            }

        }
        /// <summary>
        /// GetGatewayNodeString
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="ConfigUid"></param>
        /// <param name="nodeNo"></param>
        /// <param name="nwno"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> GetGatewayNodeString(int nodeId, int ConfigUid, int nodeNo, int nwno, List<GatewayNode> model)
        {
            try
            {
                List<dynamic> csarray = new List<dynamic>();
                string joined = "";
                GatewayNode gNode = model.Where(x => x.NodeId == nodeId && x.GwSrn == nwno).OrderBy(x => x.GwSrn).ThenBy(x => x.NodeId).FirstOrDefault(); //await _mainDBContext.GatewayNode.
                csarray = new List<dynamic>();
                if (gNode != null)
                {
                    //Node ID
                    csarray.Add(LittleEndian4X((int)nodeNo));
                    //UPID
                    csarray.Add(DecimalTo2Hex(ConfigUid));
                    //Product Type -- Check for porduct type 4/2
                    csarray.Add(DecimalTo2Hex((int)gNode.ProductId));
                    csarray.Add(MobileCorrecton(gNode.Operator1MobNo));
                    csarray.Add(MobileCorrecton(gNode.Operator2MobNo));
                    csarray.Add(MobileCorrecton(gNode.Operator3MobNo));
                    csarray.Add(MobileCorrecton(gNode.Operator4MobNo));
                    csarray.Add(MobileCorrecton(gNode.Operator5MobNo));

                    csarray.Add(ToLSBHexString((float)gNode.TempSensorHighTh)); //Float Conversion
                    csarray.Add(ToLSBHexString((float)gNode.TempSensorLowTh));  //Float Conversion
                    csarray.Add(ToLSBHexString((float)gNode.VbatlowThvoltage)); //Float Conversion

                    csarray.Add(DecimalTo2Hex((int)gNode.AlarmInterval));
                    csarray.Add(DecimalTo2Hex((int)gNode.FuaskInterval));
                    csarray.Add(ToLSBHexString((float)gNode.Maxiocurrent));
                    csarray.Add(DecimalTo2Hex(gNode.LogAutoTxEndis == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.Debug == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.Warning == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.Error == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.Info == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex((int)gNode.Direction));
                    csarray.Add(LittleEndian4X((int)gNode.StatusStoreDuration));
                    csarray.Add(DecimalTo2Hex(gNode.GsmportEn == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.SchtransferedEnableDis == true ? 1 : 0));
                    csarray.Add(DecimalTo2Hex(gNode.ForceAlarmDisable == true ? 1 : 0));
                    csarray.Add(LittleEndian4X((int)gNode.Comdelay));
                    csarray.Add(DecimalTo2Hex(gNode.LoRaConcentratorEn == true ? 1 : 0));
                    csarray.Add(LittleEndian32X((int)gNode.SettingRfport));

                    string card1 = DecimalTo2Hex((int)gNode.CardNo1);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1Type);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1Debug);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1Warning);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1Error);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1Info);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1SleepEn);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1AuoStatusEn);
                    card1 = card1 + LittleEndian4X((int)gNode.C1AutoStatusInt);
                    card1 = card1 + LittleEndian4X((int)gNode.C1LogIntSec);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1FirmareVer);
                    card1 = card1 + DecimalTo2Hex((int)gNode.C1SafteyTimeoutMin);
                    if (gNode.C1Type == 4)
                    {
                        List<string> setting = gNode.C1Settings.Split(",").ToList();
                        if (setting.Count > 2)
                        {
                            card1 = card1 + LittleEndian4X(Convert.ToInt32(setting[0]));
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                        }
                        else
                        {
                            card1 = card1 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C1Type == 6)
                    {
                        List<string> setting = gNode.C1Settings.Split(",").ToList();
                        if (setting.Count > 3)
                        {
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[0]));
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                            card1 = card1 + DecimalTo2Hex(Convert.ToInt32(setting[3]));
                        }
                        else
                        {
                            card1 = card1 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C1Type == 3)
                    {
                        card1 = card1 + ToLSBHexString(float.Parse(gNode.C1Settings));
                    }
                    else
                    {
                        card1 = card1 + LittleEndian8X(Convert.ToInt32(gNode.C1Settings));
                    }
                    csarray.Add(card1);

                    string card2 = DecimalTo2Hex((int)gNode.CardNo2);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2Type);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2Debug);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2Warning);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2Error);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2Info);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2SleepEn);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2AuoStatusEn);
                    card2 = card2 + LittleEndian4X((int)gNode.C2AutoStatusInt);
                    card2 = card2 + LittleEndian4X((int)gNode.C2LogIntSec);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2FirmareVer);
                    card2 = card2 + DecimalTo2Hex((int)gNode.C2SafteyTimeoutMin);
                    if (gNode.C2Type == 4)
                    {
                        List<string> setting = gNode.C2Settings.Split(",").ToList();
                        if (setting.Count > 2)
                        {
                            card2 = card2 + LittleEndian4X(Convert.ToInt32(setting[0]));
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                        }
                        else
                        {
                            card2 = card2 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C2Type == 6)
                    {
                        List<string> setting = gNode.C2Settings.Split(",").ToList();
                        if (setting.Count > 3)
                        {
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[0]));
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                            card2 = card2 + DecimalTo2Hex(Convert.ToInt32(setting[3]));
                        }
                        else
                        {
                            card2 = card2 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C2Type == 3)
                    {
                        card2 = card2 + ToLSBHexString(float.Parse(gNode.C2Settings));
                    }
                    else
                    {
                        card2 = card2 + LittleEndian8X(Convert.ToInt32(gNode.C2Settings));
                    }
                    csarray.Add(card2);

                    string card3 = DecimalTo2Hex((int)gNode.CardNo3);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3Type);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3Debug);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3Warning);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3Error);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3Info);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3SleepEn);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3AuoStatusEn);
                    card3 = card3 + LittleEndian4X((int)gNode.C3AutoStatusInt);
                    card3 = card3 + LittleEndian4X((int)gNode.C3LogIntSec);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3FirmareVer);
                    card3 = card3 + DecimalTo2Hex((int)gNode.C3SafteyTimeoutMin);
                    if (gNode.C3Type == 4)
                    {
                        List<string> setting = gNode.C3Settings.Split(",").ToList();
                        if (setting.Count > 2)
                        {
                            card3 = card3 + LittleEndian4X(Convert.ToInt32(setting[0]));
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                        }
                        else
                        {
                            card3 = card3 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C3Type == 6)
                    {
                        List<string> setting = gNode.C3Settings.Split(",").ToList();
                        if (setting.Count > 3)
                        {
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[0]));
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[1]));
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[2]));
                            card3 = card3 + DecimalTo2Hex(Convert.ToInt32(setting[3]));
                        }
                        else
                        {
                            card3 = card3 + LittleEndian8X(0);
                        }
                    }
                    else if (gNode.C3Type == 3)
                    {
                        card3 = card3 + ToLSBHexString(float.Parse(gNode.C3Settings));
                    }
                    else
                    {
                        card3 = card3 + LittleEndian8X(Convert.ToInt32(gNode.C3Settings));
                    }
                    csarray.Add(card3);

                    csarray.Add(LittleEndian4X((int)gNode.ProgramEndDayMode));
                    joined = string.Join("", new List<dynamic>(csarray).ToArray());
                }

                return joined;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FramesService)}.{nameof(GetGatewayNodeString)}]{ex}");
                return "";
            }
        }
        /// <summary>
        /// GetNotificationResponse
        /// </summary>
        /// <param name="model"></param>
        /// <param name="datalogger"></param>
        /// <param name="flag"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public async Task<dynamic> GetNotificationResponse(GWToSWNotificationVIewModel model, string datalogger, int flag, int projectID)
        { /*       {
                        "head": ["AN", 2, 4, 1, 1211, 11, 2, 100],
                        "body": {
                                    "APP_UPID": [],
                                    "NODE_UPID": ["0401, 1, 1, 1, 1","0402, 1, 1, 1, 1"],
                                    "SQ_UPID": []
                                 }
                       }
            */
            try
            {
                _logger.LogInformation("Step 1--" + DateTime.Now);

                bool noChange = true;
                int totalLengthAdded = 0;
                int totalLengthRunning = 0;
                string runningString = "";
                List<dynamic> csarray = new List<dynamic>();
                List<dynamic> SSensor = new List<dynamic>();
                List<dynamic> VRT = new List<dynamic>();
                List<dynamic> SEQ = new List<dynamic>();
                string joined = string.Empty;
                string FrameType = "AN";
                int GWID = 0;
                int PROJECT_ID = projectID;
                int DB_ID = 1;
                int ACTIVE_MIN = 0;
                int NWT_UPID = 0;
                int MAX_LENGTH_IN_KB = 100;
                int CMD_NO = 0;
                int ACTIVEMIN = 0;
                int TOTAL_NODE_IN_PROJECT = 0;
                int PROGRAM_Day_END_MOD = 0;
                int YEAR = Convert.ToInt32(DateTime.Now.ToString("yy"));
                int MONTH = DateTime.Now.Month;
                int DATE = DateTime.Now.Day;
                int HH = DateTime.Now.Hour;
                int MM = DateTime.Now.Minute;
                int SS = DateTime.Now.Second;
                int WEEK_DA = (int)DateTime.Now.DayOfWeek;

                if (flag == 1)
                {
                    FrameType = model.HEAD[0].ToString();
                    if (FrameType == "JN")
                    {
                        int GwSrnNo = Convert.ToInt32(model.HEAD[1]);
                        Gateway gw = await GetGatewayBySrNo(GwSrnNo);
                        GWID = Convert.ToInt32(gw.GatewayNo);
                        // GWID = Convert.ToInt32(model.HEAD[1]);
                        CMD_NO = Convert.ToInt32(model.HEAD[2]);
                        ACTIVEMIN = Convert.ToInt32(model.HEAD[3]);
                        // UpdateIdsProject proj = await GetUpdateIdsProject();
                        NWT_UPID = Convert.ToInt32(model.HEAD[4]);
                        MAX_LENGTH_IN_KB = Convert.ToInt32(model.HEAD[5]) * 1024;
                        if (model.HEAD[0].ToString() == "JN")
                        {
                            //var updatedata = _mainDBContext.UpdateIds.Where(x => x.Gwid == GWID).AsQueryable();
                            //foreach (var item in updatedata)
                            //{
                            //    item.ConfigUid = 0;
                            //    item.VrtUid = 0;
                            //    item.ScheduleNodeUid = 0;
                            //    item.SensorUid = 0;
                            //}
                            //_mainDBContext.UpdateIds.UpdateRange(updatedata);
                            //_mainDBContext.SaveChanges();
                            var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
                            // var dbConnectionString = DbManager.GetDbConnectionString(_config["SiteName"], "Main");
                            using (var sqlConnection = new SqlConnection(dbConnectionString))
                            {
                                await sqlConnection.OpenAsync();
                                var parameters = new DynamicParameters();
                                parameters.Add("@GwId", GWID);
                                var resultSP = await sqlConnection.QueryMultipleAsync("MultiUpdateUpids", parameters, null, null, CommandType.StoredProcedure);
                            }
                            // bool flag = await ResetUpids(GWID);
                        }
                    }
                    else
                    {
                        GWID = Convert.ToInt32(model.HEAD[1]);
                        PROJECT_ID = Convert.ToInt32(model.HEAD[2]);
                        DB_ID = Convert.ToInt32(model.HEAD[3]);
                        CMD_NO = Convert.ToInt32(model.HEAD[4]);
                        ACTIVEMIN = Convert.ToInt32(model.HEAD[5]);
                        NWT_UPID = Convert.ToInt32(model.HEAD[6]);
                        MAX_LENGTH_IN_KB = Convert.ToInt32(model.HEAD[7]) * 1024;
                    }
                }
                else
                {
                    FrameType = "DL";
                    GWID = HexToDecimal(datalogger.Substring(2, 4));
                    PROJECT_ID = HexToDecimal(datalogger.Substring(6, 8));
                    DB_ID = HexToDecimal(datalogger.Substring(14, 8));
                    CMD_NO = HexToDecimal(datalogger.Substring(22, 4));
                    ACTIVEMIN = 0;
                    NWT_UPID = HexToDecimal(datalogger.Substring(26, 2));
                    MAX_LENGTH_IN_KB = HexToDecimal(datalogger.Substring(28, 4)) * 1024;
                }

                //GWID = Convert.ToInt32(model.HEAD[1]);
                //PROJECT_ID = Convert.ToInt32(model.HEAD[2]);
                //DB_ID = Convert.ToInt32(model.HEAD[3]);
                //CMD_NO = Convert.ToInt32(model.HEAD[4]);
                //ACTIVE_MIN = Convert.ToInt32(model.HEAD[5]);
                //NWT_UPID = Convert.ToInt32(model.HEAD[6]);


                //Total Node in project
                TOTAL_NODE_IN_PROJECT = await _mainDBContext.ProjectConfiguration.Select(x => (int)x.MaxNodeInProject).FirstOrDefaultAsync();
                //Get project Updateids
                var projUpdis = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                NotificationResponse notiResponse = new NotificationResponse();
                List<dynamic> CS = new List<dynamic>();
                List<dynamic> HEAD = new List<dynamic>();
                HEAD.Add("AN");
                HEAD.Add(GWID);
                HEAD.Add(PROJECT_ID);
                HEAD.Add(DB_ID);
                HEAD.Add(CMD_NO);
                HEAD.Add(projUpdis.ProjectUpId);
                HEAD.Add(TOTAL_NODE_IN_PROJECT);
                HEAD.Add(PROGRAM_Day_END_MOD);
                HEAD.Add(YEAR);
                HEAD.Add(MONTH);
                HEAD.Add(DATE);
                HEAD.Add(HH);
                HEAD.Add(MM);
                HEAD.Add(SS);
                HEAD.Add(WEEK_DA);
                notiResponse.HEAD = HEAD;

                _logger.LogInformation("Step 2--" + DateTime.Now);

                //Body of Notification frame
                NotificationGWTOSWBODY BODY = model.BODY;
                //Get All nodes with Upids from Gateway
                List<object> NODE_UPID = BODY.NODE_UPID;
                List<int> lstOfNodesFromGw = new List<int>();

                if (FrameType == "JN")
                {
                    if (NODE_UPID.Count == 0)
                    {
                        var upidsnodes = _mainDBContext.UpdateIds.Where(x => x.Gwid == GWID).OrderBy(x => x.Gwid).ThenBy(x => x.NodeId).AsQueryable();
                        foreach (var item in upidsnodes)
                        {
                            NODE_UPID.Add(LittleEndian4X((int)item.NodeId).ToString() + "0000000000");
                        }




                    }
                }
                else if (FrameType == "HB" || FrameType == "FS" || FrameType == "DL")
                {
                    if (NODE_UPID.Count == 0)
                    {
                        var upidsnodes = await _mainDBContext.UpdateIds.Where(x => x.Gwid == GWID).OrderBy(x => x.Gwid).ThenBy(x => x.NodeId).ToListAsync();
                        foreach (var item in upidsnodes)
                        {
                            //Node No
                            string lNode = LittleEndian4X((int)item.NodeId).ToString();
                            int nodeNo = HexToDecimal(ChangeHexOrder(lNode));
                            lstOfNodesFromGw.Add(nodeNo);
                            NODE_UPID.Add(LittleEndian4X((int)item.NodeId).ToString() + DecimalTo2Hex((int)item.ConfigUid) + DecimalTo2Hex((int)item.VrtUid) + DecimalTo2Hex((int)item.SensorUid) + DecimalTo2Hex((int)item.ScheduleNodeUid));
                        }
                    }
                }
                //Get All SeqUpIds
                List<int> SQ_UPID = BODY.SQ_UPID;

                _logger.LogInformation("Step 2--" + DateTime.Now);

                //If NW upid /projectUpid missmatched
                if (projUpdis.ProjectUpId != NWT_UPID)
                {
                    //LIST OF NODES FROM GATEWAY AND UPDATE TABLE
                    #region Update Nodeid 
                    List<UpdateIds> ListToUpdateIds = new List<UpdateIds>();
                    //IF Frame type equal to AN update the up Ids
                    if (FrameType == "AN" || FrameType == "JN")
                    {
                        if (NODE_UPID.Count > 0)
                        {   //Update the UPIds commming form gateway
                            foreach (string itemnode in NODE_UPID)
                            {
                                int placeNo = 0;
                                // string[] nodesToProcess = itemnode.Split(',').ToArray();
                                //Node No
                                int nodeNo = HexToDecimal(ChangeHexOrder(itemnode.Substring(placeNo, 4)));
                                //CS Upid
                                int csUPid = HexToDecimal(itemnode.Substring(4, 2));
                                //VRT Upid
                                int vrtUPid = HexToDecimal(itemnode.Substring(6, 2));
                                //SS Upid
                                int ssUPid = HexToDecimal(itemnode.Substring(8, 2));
                                //Sch Upid
                                int schUPid = HexToDecimal(itemnode.Substring(10, 2));
                                // Update updateids in db for this node //Required Table
                                //TODO: check gateway for updating
                                if (nodeNo > 0 && GWID >= 0)
                                {
                                    if (FrameType == "AN")
                                    {
                                        var nodeidToUpdate = await _mainDBContext.UpdateIds.Where(x => x.NodeId == nodeNo && x.Gwid == GWID).FirstOrDefaultAsync();
                                        if (nodeidToUpdate != null)
                                        {
                                            nodeidToUpdate.ConfigUid = csUPid;
                                            nodeidToUpdate.VrtUid = vrtUPid;
                                            nodeidToUpdate.SensorUid = ssUPid;
                                            nodeidToUpdate.ScheduleNodeUid = schUPid;
                                            // ListToUpdateIds.Add(nodeidToUpdate);
                                            //_mainDBContext.UpdateIds.Update(nodeidToUpdate);
                                            //_mainDBContext.SaveChanges();
                                        }
                                    }

                                }


                                //Node No
                                int nodenoForDBupid = nodeNo & 1023;
                                lstOfNodesFromGw.Add(nodeNo);
                            }

                            _mainDBContext.UpdateIds.UpdateRange(ListToUpdateIds);
                            _mainDBContext.SaveChanges();
                        }
                    }
                    #endregion
                    _logger.LogInformation("Step 3--" + DateTime.Now);

                    //UPADTE MAXSCH ID FROM GATEWAY
                    if (FrameType == "AN")
                    {
                        var gwSchToUpdate = await _mainDBContext.GatewayMaxSch.Where(x => x.GatewayNo == GWID).FirstOrDefaultAsync();
                        gwSchToUpdate.MaxSchUpId = model.BODY.MAIN_SCH_UPID;
                        _mainDBContext.GatewayMaxSch.Update(gwSchToUpdate);
                        await _mainDBContext.SaveChangesAsync();
                    }
                    _logger.LogInformation("Step 4--" + DateTime.Now);

                    //Get Gateway node table for scanning as  per gateway
                    var gatewayNodeidLst = await _mainDBContext.UpdateIds.Where(x => x.Gwid == GWID).OrderBy(x => x.Gwid).ThenBy(x => x.NodeId).ToListAsync();

                    // Get List of nodes in Server with Upids 
                    List<UpdateIdsRequired> lstOfNodesToCheckRequired = await _mainDBContext.UpdateIdsRequired.OrderBy(x => x.NetworkNo).ThenBy(x => x.NodeId).ToListAsync();

                    //List<int> lstOfNodesNotInGw = new List<int>();
                    //lstOfNodesNotInGw = lstOfNodesToCheckRequired.Where(x => !lstOfNodesFromGw.Contains((int)x.NodeId)).Select(x => (int)x.NodeId).ToList();
                    _logger.LogInformation("Step 5--" + DateTime.Now);

                    //CS
                    //For each nodeupid array from gateway, save updateids from server
                    #region CS Setting
                    var RechargableNodeList = _mainDBContext.RechableNode.AsEnumerable();
                    List<RechableNode> rechableNodes = RechargableNodeList.ToList();

                    var nonRechargableNodeList = _mainDBContext.NonRechableNode.AsEnumerable();
                    List<NonRechableNode> nonRechableNodes = nonRechargableNodeList.ToList();

                    var GateWayNodeList = _mainDBContext.GatewayNode.AsEnumerable();
                    List<GatewayNode> gatewayNode = GateWayNodeList.ToList();


                    foreach (int nodeNo in lstOfNodesFromGw) //foreach (var item in gatewayNodeidLst)
                    {
                        totalLengthRunning = 0;
                        //Node No
                        //Get actual node no
                        int nodenoForDB = nodeNo & 1023;
                        int shifts = nodeNo >> 10;

                        if (lstOfNodesToCheckRequired.Count > 0)
                        {
                            var upids = gatewayNodeidLst.Where(x => x.NodeId == nodeNo).FirstOrDefault();
                            //CS Upid
                            int csUPid = (int)upids.ConfigUid;

                            if (nodenoForDB != 0)
                            {
                                //Check node updateids in required table
                                var nodeToCheckModel = lstOfNodesToCheckRequired.Where(x => x.NodeId == nodenoForDB && x.NetworkNo == shifts).FirstOrDefault();
                                //If config id missmatched
                                if (nodeToCheckModel.ConfigUid != csUPid)
                                {
                                    noChange = false;
                                    if (nonRechableNodes.Any(x => x.NodeId == nodenoForDB))
                                    {
                                        //Get NonRechargable String
                                        joined = await GetNonRechargableString(nodenoForDB, (int)nodeToCheckModel.ConfigUid, nodeNo, shifts, nonRechableNodes);
                                        totalLengthRunning = joined.Length;
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {
                                            if (joined != "")
                                                CS.Add(joined);
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                        }
                                        else
                                        {
                                            break;
                                        }

                                    }


                                    if (rechableNodes.Any(x => x.NodeId == nodenoForDB))
                                    {
                                        //Get Rechargable Node String
                                        joined = await GetRechargableNodeString(nodenoForDB, (int)nodeToCheckModel.ConfigUid, nodeNo, shifts, rechableNodes);
                                        totalLengthRunning = joined.Length;
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {
                                            if (joined != "")
                                                CS.Add(joined);
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;

                                            //Node node = new Node();
                                            //node.NodeNo = nodenoForDB;
                                            //node.ProductTypeId = 2;
                                            //node.IsAddonCard = true;
                                            //await _mainDBContext.Node.AddAsync(node);
                                            //await _mainDBContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            break;
                                        }


                                    }
                                    if (gatewayNode.Any(x => x.NodeId == nodenoForDB))
                                    {
                                        //Get Gateway Node String
                                        joined = await GetGatewayNodeString(nodenoForDB, (int)nodeToCheckModel.ConfigUid, nodeNo, shifts, gatewayNode);
                                        totalLengthRunning = joined.Length;
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {
                                            if (joined != "")
                                                CS.Add(joined);
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;

                                            //Node node = new Node();
                                            //node.NodeNo = nodenoForDB;
                                            //node.ProductTypeId = 3;
                                            //node.IsAddonCard = true;
                                            //await _mainDBContext.Node.AddAsync(node);
                                            //await _mainDBContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    #endregion
                    _logger.LogInformation("Step 6--" + DateTime.Now);

                    //VRT SETTING
                    //For each nodeupid array from gateway, save updateids from server
                    #region VRT SETTING
                    var vrtset = _mainDBContext.Vrtsetting.AsEnumerable();
                    List<Vrtsetting> vrtsettings = vrtset.ToList();
                    foreach (int nodeNo in lstOfNodesFromGw)
                    {
                        //Node No
                        //int nodeNo = (int)item.NodeId;
                        ////SS Upid
                        //int vrtUPid = (int)item.VrtUid;
                        //Get actual node no
                        int nodenoForDBSS = nodeNo & 1023;
                        int nwno = nodeNo >> 10;
                        if (lstOfNodesToCheckRequired.Count > 0)
                        {
                            var upids = gatewayNodeidLst.Where(x => x.NodeId == nodeNo).FirstOrDefault();
                            //CS Upid
                            int vrtUPid = (int)upids.VrtUid;
                            //Get actual node no
                            int nodenoForDBVrt = nodeNo & 1023;
                            if (nodenoForDBVrt != 0)
                            {
                                var nodeToCheckModel = lstOfNodesToCheckRequired.Where(x => x.NodeId == nodenoForDBVrt && x.NetworkNo == nwno).FirstOrDefault();
                                //If config id missmatched
                                if (nodeToCheckModel.VrtUid != vrtUPid)
                                {
                                    noChange = false;
                                    //Get VRT Setting
                                    List<Vrtsetting> vrtsetting = vrtsettings.Where(x => x.NodeId == nodenoForDBVrt && x.GwSrn == nwno).OrderBy(x => x.ValveNo).ToList();
                                    if (vrtsetting != null)
                                    {
                                        int i = 0; string vrtjoined = "";
                                        int OldNodeId = 0;
                                        List<dynamic> vrtarray = new List<dynamic>();
                                        foreach (Vrtsetting vsetting in vrtsetting)
                                        {
                                            if (i != 0)
                                            {
                                                vrtarray.Add("X");
                                            }
                                            else
                                            {
                                                vrtarray.Add(LittleEndian4X(nodeNo));
                                                vrtarray.Add(DecimalTo2Hex((int)nodeToCheckModel.VrtUid));
                                                vrtarray.Add(DecimalTo2Hex((int)vsetting.ProductType));
                                            }
                                            vrtarray.Add(DecimalTo2Hex((int)vsetting.ValveNo));
                                            vrtarray.Add(DecimalTo2Hex((int)vsetting.ValveType));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.MasterNodeId));
                                            vrtarray.Add(DecimalTo2Hex((int)vsetting.MasterValveNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.BlockNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.FertGrpNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.FilterGrpNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.LinkedSensor1NodeId));
                                            vrtarray.Add(DecimalTo2Hex((int)vsetting.LinkedSensor1SensorNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.LinkedSensor2NodeId));
                                            vrtarray.Add(DecimalTo2Hex((int)vsetting.LinkedSensor2SensorNo));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.ValveGrpNo1));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.ValveGrpNo2));
                                            vrtarray.Add(LittleEndian4X((int)vsetting.HeadPumpGrNo));
                                            i++;
                                        }

                                        runningString = string.Join("", new List<dynamic>(vrtarray).ToArray());
                                        totalLengthRunning = runningString.Length;
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {
                                            vrtjoined = vrtjoined + runningString;
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        if (vrtjoined != "")
                                        {
                                            vrtjoined = vrtjoined + "X";
                                            VRT.Add(vrtjoined);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    _logger.LogInformation("Step 7--" + DateTime.Now);

                    //SENSOR SETTING
                    //For each nodeupid array from gateway, save updateids from server
                    #region SENSOR SETTING

                    var analog420V = _mainDBContext.Analog420mAsensor.AsEnumerable();
                    List<Analog420mAsensor> analog420VsensorLst = analog420V.ToList();

                    //Type  - 3
                    var Analog05 = _mainDBContext.Analog05vsensor.AsEnumerable();
                    List<Analog05vsensor> analog05VsensorLst = Analog05.ToList();
                    //Type  - 4
                    var digitalNoNctype = _mainDBContext.DigitalNoNctypeSensor.AsEnumerable();
                    List<DigitalNoNctypeSensor> digitalNoNctypeSensorLst = digitalNoNctype.ToList();
                    //Type  - 5
                    var digitalCounterType = _mainDBContext.DigitalCounterTypeSensor.AsEnumerable();
                    List<DigitalCounterTypeSensor> digitalCounterTypeSensorLst = digitalCounterType.ToList();

                    //Type  - 37
                    var waterMeterSensor = _mainDBContext.WaterMeterSensorSetting.AsEnumerable();
                    List<WaterMeterSensorSetting> waterMeterSensorSettingLst = waterMeterSensor.ToList();


                    foreach (var nodeNo in lstOfNodesFromGw)
                    {
                        string ssjoined = "";
                        bool isHeaderAdded = false;
                        ////Node No
                        //int nodeNo = (int)item.NodeId;
                        ////SS Upid
                        //int ssUPid = (int)item.SensorUid;
                        //Get actual node no
                        int nodenoForDBSS = nodeNo & 1023;
                        int nwno = nodeNo >> 10;
                        if (lstOfNodesToCheckRequired.Count > 0)
                        {
                            var upids = gatewayNodeidLst.Where(x => x.NodeId == nodeNo).FirstOrDefault();
                            //CS Upid
                            int ssUPid = (int)upids.SensorUid;
                            if (nodenoForDBSS != 0)
                            {
                                var nodeToCheckModel = lstOfNodesToCheckRequired.Where(x => x.NodeId == nodenoForDBSS && x.NetworkNo == nwno).FirstOrDefault();
                                //If config id missmatched
                                if (nodeToCheckModel.SensorUid != ssUPid)
                                {
                                    noChange = false;
                                    //GET Sensor Setting
                                    //Type  - 2
                                    List<Analog420mAsensor> analog420Vsensor = analog420VsensorLst.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).ToList();
                                    //Type  - 3
                                    List<Analog05vsensor> analog05Vsensor = analog05VsensorLst.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).ToList();
                                    //Type  - 4
                                    List<DigitalNoNctypeSensor> digitalNoNctypeSensor = digitalNoNctypeSensorLst.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).ToList();
                                    //Type  - 5
                                    List<DigitalCounterTypeSensor> digitalCounterTypeSensor = digitalCounterTypeSensorLst.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).ToList();
                                    //Type  - 37
                                    List<WaterMeterSensorSetting> waterMeterSensorSetting = waterMeterSensorSettingLst.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).ToList();
                                    List<dynamic> ssarray = new List<dynamic>();
                                    List<dynamic> ssarrayRunning = new List<dynamic>();
                                    if (analog420Vsensor.Count > 0 || analog05Vsensor.Count > 0 || digitalNoNctypeSensor.Count > 0 || digitalCounterTypeSensor.Count > 0 || waterMeterSensorSetting.Count > 0)
                                    {
                                        //--------------------------
                                        //HEADERS
                                        //--------------------------
                                        //Node ID
                                        ssarrayRunning.Add(LittleEndian4X(nodeNo));
                                        //UPID
                                        ssarrayRunning.Add(DecimalTo2Hex((int)nodeToCheckModel.SensorUid));
                                        bool shownProductTypeId = false;
                                        //Get Node
                                        if (analog420Vsensor != null && analog420Vsensor.Count > 0)
                                        {
                                            if (!shownProductTypeId)
                                            {
                                                var node = analog420Vsensor.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).FirstOrDefault();
                                                if (node != null)
                                                {
                                                    shownProductTypeId = true;
                                                    ssarrayRunning.Add(DecimalTo2Hex((int)node.NodePorductId));
                                                }
                                                else
                                                {
                                                    ssarrayRunning.Add(DecimalTo2Hex(0));
                                                }
                                            }


                                        }
                                        else if (analog05Vsensor != null && analog05Vsensor.Count > 0)
                                        {
                                            if (!shownProductTypeId)
                                            {
                                                var node = analog05Vsensor.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).FirstOrDefault();
                                                if (node != null)
                                                {
                                                    shownProductTypeId = true;
                                                    ssarrayRunning.Add(DecimalTo2Hex((int)node.NodePorductId));
                                                }
                                                else
                                                {
                                                    ssarrayRunning.Add(DecimalTo2Hex(0));
                                                }
                                            }

                                        }
                                        else if (digitalNoNctypeSensor != null && digitalNoNctypeSensor.Count > 0)
                                        {
                                            if (!shownProductTypeId)
                                            {
                                                var node = digitalNoNctypeSensor.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).FirstOrDefault();
                                                if (node != null)
                                                {
                                                    shownProductTypeId = true;
                                                    ssarrayRunning.Add(DecimalTo2Hex((int)node.NodePorductId));
                                                }
                                                else
                                                {
                                                    ssarrayRunning.Add(DecimalTo2Hex(0));
                                                }
                                            }

                                        }
                                        else if (digitalCounterTypeSensor != null && digitalCounterTypeSensor.Count > 0)
                                        {
                                            if (!shownProductTypeId)
                                            {
                                                var node = digitalCounterTypeSensor.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).FirstOrDefault();
                                                if (node != null)
                                                {
                                                    shownProductTypeId = true;
                                                    ssarrayRunning.Add(DecimalTo2Hex((int)node.NodePorductId));
                                                }
                                                else
                                                {
                                                    ssarrayRunning.Add(DecimalTo2Hex(0));
                                                }
                                            }

                                        }
                                        else if (waterMeterSensorSetting != null && waterMeterSensorSetting.Count > 0)
                                        {
                                            if (!shownProductTypeId)
                                            {
                                                var node = waterMeterSensorSetting.Where(x => x.NodeId == nodenoForDBSS && x.GwSrn == nwno).FirstOrDefault();
                                                if (node != null)
                                                {
                                                    shownProductTypeId = true;
                                                    ssarrayRunning.Add(DecimalTo2Hex((int)node.NodePorductId));
                                                }
                                                else
                                                {
                                                    ssarrayRunning.Add(DecimalTo2Hex(0));
                                                }
                                            }

                                        }
                                        else
                                        {
                                            ssarrayRunning.Add(DecimalTo2Hex(0));
                                        }

                                        //if(nwno == 0)
                                        //{
                                        //    ssarrayRunning.Add(DecimalTo2Hex(4));
                                        //}
                                        //else
                                        //{
                                        //    var node = _mainDBContext.Node.Where(x => x.NodeNo == nodenoForDBSS).FirstOrDefault();
                                        //    if (node != null)
                                        //    {
                                        //        if (node.ProductTypeId != null)
                                        //        {
                                        //            //Product Type
                                        //            ssarrayRunning.Add(DecimalTo2Hex((int)node.ProductTypeId));
                                        //        }
                                        //        else
                                        //        {
                                        //            ssarrayRunning.Add(DecimalTo2Hex(0));
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        ssarrayRunning.Add(DecimalTo2Hex(0));
                                        //    }

                                        //}


                                        //runningString = string.Join("", new List<dynamic>(ssarray).ToArray());
                                        //totalLengthRunning = runningString.Length;
                                        //if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        //{

                                        //    ssjoined = ssjoined + runningString;
                                        //    totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                        //}
                                        //else
                                        //{
                                        //    break;
                                        //}
                                    }
                                    //Analog 420V
                                    if (analog420Vsensor != null)
                                    {
                                        if (analog420Vsensor.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var itemss in analog420Vsensor)
                                            {
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SsNo));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.ProductType));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.Sspriority));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SamplingRate));
                                                //SS DATA
                                                string ssData = "";
                                                ssData = ssData + ToLSBHexString((float)itemss.CtrHghThrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.HghThrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.LwTthrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.CrtLwrThrs);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyCfmThrs);
                                                ssData = ssData + DecimalTo16Hex((int)itemss.Rsved);
                                                //ADD SS Data
                                                ssarrayRunning.Add(ssData);
                                                ssarrayRunning.Add("X");
                                                //runningString = string.Join("", new List<dynamic>(ssarrayRunning).ToArray());
                                                //totalLengthRunning = runningString.Length;
                                                //if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                                //{

                                                //    ssjoined = ssjoined + runningString;
                                                //    ssjoined = ssjoined + "X";
                                                //    totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                                //}
                                                //else
                                                //{
                                                //    break;
                                                //}
                                                // SSensor.Add(ssjoined);
                                                i++;
                                            }

                                        }

                                    }
                                    //Analog 05V
                                    if (analog05Vsensor != null)
                                    {
                                        if (analog05Vsensor.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var itemss in analog05Vsensor)
                                            {

                                                //if (!isHeaderAdded)
                                                //{
                                                //    ssarrayRunning.Add(string.Join("", new List<dynamic>(ssarray).ToArray()));
                                                //    isHeaderAdded = true;
                                                //}

                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SsNo));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.ProductType));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.Sspriority));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SamplingRate));

                                                //SS DATA
                                                string ssData = "";
                                                ssData = ssData + ToLSBHexString((float)itemss.CtrHghThrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.HghThrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.LwTthrs);
                                                ssData = ssData + ToLSBHexString((float)itemss.CrtLwrThrs);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyCfmThrs);
                                                ssData = ssData + DecimalTo16Hex((int)itemss.Rsved);
                                                //ADD SS Data
                                                ssarrayRunning.Add(ssData);
                                                ssarrayRunning.Add("X");

                                                //runningString = string.Join("", new List<dynamic>(ssarrayRunning).ToArray());
                                                //totalLengthRunning = runningString.Length;
                                                //if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                                //{

                                                //    ssjoined = ssjoined + runningString;
                                                //    ssjoined = ssjoined + "X";
                                                //    totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                                //}
                                                //else
                                                //{
                                                //    break;
                                                //}
                                                //ssjoined = ssjoined + string.Join("", new List<dynamic>(ssarrayRunning).ToArray());                                       
                                                //  SSensor.Add(ssjoined);
                                                i++;
                                            }

                                        }

                                    }
                                    //Analog NONC
                                    if (digitalNoNctypeSensor != null)
                                    {
                                        if (digitalNoNctypeSensor.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var itemss in digitalNoNctypeSensor)
                                            {
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SsNo));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.ProductType));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.Sspriority));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SamplingRate));

                                                //SS DATA
                                                string ssData = "";
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DfltStat);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.StatRevDly);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.AlrmLvl);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyTmeIfRevr);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyCfmThrs);
                                                ssData = ssData + DecimalTo40Hex((int)itemss.Rsved);
                                                //ADD SS Data
                                                ssarrayRunning.Add(ssData);
                                                ssarrayRunning.Add("X");
                                                //runningString = string.Join("", new List<dynamic>(ssarrayRunning).ToArray());
                                                //totalLengthRunning = runningString.Length;
                                                //if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                                //{

                                                //    ssjoined = ssjoined + runningString;
                                                //    ssjoined = ssjoined + "X";
                                                //    totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                                //}
                                                //else
                                                //{
                                                //    break;
                                                //}
                                                i++;
                                            }

                                        }

                                    }
                                    //DIgital COunter
                                    if (digitalCounterTypeSensor != null)
                                    {
                                        if (digitalCounterTypeSensor.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var itemss in digitalCounterTypeSensor)
                                            {
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SsNo));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.ProductType));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.Sspriority));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SamplingRate));

                                                //SS DATA
                                                string ssData = "";
                                                ssData = ssData + ToLSBHexString((float)itemss.HghFlwRtFrq);
                                                ssData = ssData + ToLSBHexString((float)itemss.LowFlwRtFrq);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.FilTmIniWtTm);
                                                ssData = ssData + LittleEndian4X((int)itemss.PlsDivFct);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyCfmThrs);
                                                ssData = ssData + DecimalTo26Hex((int)itemss.Rsved);
                                                //ADD SS Data
                                                ssarrayRunning.Add(ssData);
                                                ssarrayRunning.Add("X");
                                                //runningString = string.Join("", new List<dynamic>(ssarrayRunning).ToArray());
                                                //totalLengthRunning = runningString.Length;
                                                //if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                                //{

                                                //    ssjoined = ssjoined + runningString;
                                                //    ssjoined = ssjoined + "X";
                                                //    totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                                //}
                                                //else
                                                //{
                                                //    break;
                                                //}
                                                i++;
                                            }

                                        }

                                    }
                                    //Wattermetter
                                    if (waterMeterSensorSetting != null)
                                    {
                                        if (waterMeterSensorSetting.Count > 0)
                                        {
                                            int i = 0;
                                            foreach (var itemss in waterMeterSensorSetting)
                                            {
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SsNo));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.ProductType));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.Sspriority));
                                                ssarrayRunning.Add(DecimalTo2Hex((int)itemss.SamplingRate));

                                                //SS DATA
                                                string ssData = "";
                                                ssData = ssData + ToLSBHexString((float)itemss.HghFlwRtFrq);
                                                ssData = ssData + ToLSBHexString((float)itemss.LowFlwRtFrq);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.TmeCfrmNoFlwSec);
                                                ssData = ssData + DecimalTo2Hex((int)itemss.DlyCfmThrs);
                                                ssData = ssData + DecimalTo30Hex((int)itemss.Rsved);
                                                //ADD SS Data
                                                ssarrayRunning.Add(ssData);
                                                ssarrayRunning.Add("X");
                                                // SSensor.Add(ssjoined);
                                                i++;
                                            }

                                        }

                                    }

                                    runningString = string.Join("", new List<dynamic>(ssarrayRunning).ToArray());
                                    totalLengthRunning = runningString.Length;
                                    if (runningString.Length > 0)
                                    {
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {

                                            ssjoined = ssjoined + runningString;
                                            // ssjoined = ssjoined + "X";
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        if (ssjoined != "")
                                        {
                                            SSensor.Add(ssjoined);
                                        }
                                        totalLengthRunning = runningString.Length;
                                    }


                                }
                            }
                        }
                    }
                    #endregion
                    _logger.LogInformation("Step 8--" + DateTime.Now);

                    //SEQUENCE
                    //For each nodeupid array from gateway, save updateids from server
                    #region SEQUENCE
                    var sequpload = _mainDBContext.MultiSequenceUploading.FirstOrDefault();
                    if (sequpload.SeqUploadingFlag == false)
                    {
                        MschID mschID = new MschID();
                        mschID.MAIN_SCH_UPID = await _mainDBContext.UpdateIdsMainSch.Select(x => (int)x.SeqMaxUpid).FirstOrDefaultAsync();
                        //Check saved SCH UID FROM GATEWAY
                        var gwSch = await _mainDBContext.GatewayMaxSch.Where(x => x.GatewayNo == GWID).FirstOrDefaultAsync();

                        //int MAINSCHUPIDFROMFGATEWAY = model.BODY.MAIN_SCH_UPID;
                        if (gwSch != null)
                        {
                            if (mschID.MAIN_SCH_UPID != gwSch.MaxSchUpId)
                            {
                                int nodenoForDBSS = 0;
                                string ssjoined = "";
                                noChange = false;
                                //GET Sensor Setting
                                //Type  - 2
                                //  List<NewSequence> seqData = await _mainDBContext.NewSequence.Where(x => x.NodeId == nodenoForDBSS).ToListAsync();
                                //Type  - 3
                                List<dynamic> ssarray = new List<dynamic>();
                                List<NewSequence> newSequences = new List<NewSequence>();
                                List<NewSequenceValveConfig> newSequenceValveConfig = new List<NewSequenceValveConfig>();
                                List<Nrvchannels> nrvchannels = await _mainDBContext.Nrvchannels.ToListAsync();
                                var newseq = _mainDBContext.NewSequence.AsQueryable();
                                newSequences = await newseq.OrderBy(x => x.SeqNo).ToListAsync();

                                var valconfig = _mainDBContext.NewSequenceValveConfig.AsQueryable();
                                newSequenceValveConfig = await valconfig.OrderBy(x => x.ChannelId).ToListAsync();
                                List<NewSequenceWeeklySchedule> weeklySchedules = new List<NewSequenceWeeklySchedule>();
                                List<dynamic> seqComma = new List<dynamic>();
                                if (newSequences.Count > 0)
                                {
                                    string singleSeq = "";
                                    foreach (var itemseq in newSequences)
                                    {
                                        weeklySchedules = await _mainDBContext.NewSequenceWeeklySchedule.Where(x => x.SeqId == itemseq.SeqId).ToListAsync();

                                        ssarray = new List<dynamic>();
                                        //--------------------------
                                        //    //HEADERS
                                        //--------------------------

                                        ssarray.Add(DecimalTo2Hex((int)itemseq.SeqNo));
                                        ssarray.Add(DecimalTo2Hex((int)itemseq.Upid));
                                        ssarray.Add(DecimalTo2Hex(itemseq.SeqStartDate.Value.Day));
                                        ssarray.Add(DecimalTo2Hex(itemseq.SeqStartDate.Value.Month));
                                        ssarray.Add(DecimalTo2Hex(Convert.ToInt32(itemseq.SeqStartDate.Value.ToString("yy"))));
                                        ssarray.Add(DecimalTo2Hex(itemseq.SeqEndDate.Value.Day));
                                        ssarray.Add(DecimalTo2Hex(itemseq.SeqEndDate.Value.Month));
                                        ssarray.Add(DecimalTo2Hex(Convert.ToInt32(itemseq.SeqEndDate.Value.ToString("yy"))));
                                        if (weeklySchedules == null)
                                        {
                                            ssarray.Add(DecimalTo2Hex(1));
                                        }
                                        else
                                        {
                                            // Type
                                            if (weeklySchedules.Count > 0)
                                            {
                                                ssarray.Add(DecimalTo2Hex(2));
                                            }
                                            else
                                            {
                                                ssarray.Add(DecimalTo2Hex(1));
                                            }
                                        }

                                        //  int basop = itemseq.BasisOfOp == "Interval" ? 1 : 2);
                                        //Week Days
                                        ssarray.Add(GetIntervalByteOrWeekByte(weeklySchedules, itemseq.SeqId, 1, (int)itemseq.IntervalDays));
                                        //Head/ Pump Gr. No.
                                        ssarray.Add(LittleEndian4X(0));
                                        //Start Time
                                        // ssarray.Add(LittleEndian4X(HexTimeForLE(itemseq.StartTime)));
                                        ssarray.Add(GetTimeForSchedule(itemseq.StartTime));

                                        //Start End Time
                                        //string lastValveTime = "00:00";
                                        //var seqHorizontalValves = newSequenceValveConfig.Where(x => x.SeqId == itemseq.SeqId).OrderByDescending(x => x.HorizGrId).FirstOrDefault();
                                        //if (seqHorizontalValves != null)
                                        //{
                                        //    lastValveTime = (TimeSpan.Parse(seqHorizontalValves.ValveStartTime) + TimeSpan.Parse(seqHorizontalValves.ValveStartDuration)).ToString();
                                        //}
                                        //  ssarray.Add(LittleEndian4X(HexTimeForLE(lastValveTime)));
                                        //  ssarray.Add(GetTimeForSchedule(lastValveTime));
                                        // ssarray.Add("Y");
                                        var seqValves = newSequenceValveConfig.Where(x => x.SeqId == itemseq.SeqId).OrderBy(x => x.ScheduleNo).ThenBy(x => x.HorizGrId).ThenByDescending(x => x.IsHorizontal).ToList();
                                        foreach (var itemVal in seqValves)
                                        {
                                            int nodId = (int)nrvchannels.Where(x => x.Id == itemVal.ChannelId).Select(x => x.RtuId).First();

                                            if (itemVal.IsHorizontal == true)
                                            {
                                                ssarray.Add("Y");
                                                ssarray.Add(LittleEndian4X(HexTimeForLE(itemVal.ValveStartDuration)));
                                                // ssarray.Add(GetTimeForSchedule(itemVal.ValveStartDuration));
                                            }
                                            //Node ID                                           
                                            ssarray.Add(LittleEndian4X(nodId));
                                            //Valve No
                                            int valNo = (int)nrvchannels.Where(x => x.Id == itemVal.ChannelId).Select(x => x.ValveNo).First();
                                            ssarray.Add(DecimalTo2Hex(valNo));
                                            ssarray.Add(DecimalTo2Hex(itemVal.IsFertilizerRelated == true ? 1 : 0));
                                            ssarray.Add(DecimalTo2Hex(itemVal.IsFlushRelated == true ? 1 : 0));
                                        }

                                        ssarray.Add("X");

                                        runningString = string.Join("", new List<dynamic>(ssarray).ToArray());
                                        totalLengthRunning = runningString.Length;
                                        if ((totalLengthAdded + totalLengthRunning) <= MAX_LENGTH_IN_KB)
                                        {
                                            ssjoined = ssjoined + runningString;
                                            // ssjoined = ssjoined + "X";
                                            totalLengthAdded = totalLengthAdded + totalLengthRunning;
                                        }
                                        else
                                        {
                                            ssjoined = "";
                                            SEQ = new List<dynamic>();
                                            break;
                                        }
                                        if (ssjoined != "")
                                        {
                                            SEQ.Add(ssjoined);
                                            ssjoined = "";
                                        }
                                    }

                                }

                                #region SEQUENCE UPIDS
                                //if (SEQ.Count == 0)
                                //{
                                //    noChange = true;
                                //}
                                //else
                                //{
                                //    noChange = false;
                                //}
                                //notiResponse.BODY.SCH_UPID = null;
                                List<NrseqUpids> lstUpids = await _mainDBContext.NrseqUpids.ToListAsync();
                                //List<int> gatewayList = await _mainDBContext.Gateway.Select(x => (int)x.GatewayNo).OrderBy(x => x).ToListAsync();

                                notiResponse.BODY.SCH_UPID.Add(mschID);
                                List<int> networkIds = lstOfNodesToCheckRequired.Select(x => (int)x.NetworkNo).Distinct().OrderBy(x => x).ToList(); //await _mainDBContext.UpdateIdsRequired.Select(x => (int)x.NetworkNo).Distinct().OrderBy(x => x).ToListAsync();
                                List<NWRTU> nwrtuLst = new List<NWRTU>();
                                var nrSch = _mainDBContext.NrseqUpids.AsEnumerable();
                                List<NrseqUpids> NrSchLst = nrSch.ToList();
                                foreach (var itemNw in networkIds)
                                {
                                    if (itemNw != 0)
                                    {
                                        List<int> nodeList = lstOfNodesToCheckRequired.Where(x => x.NetworkNo == itemNw).Select(x => (int)x.NodeId).OrderBy(x => x).ToList(); ///await _mainDBContext.UpdateIdsRequired.Where(x => x.NetworkNo == itemNw).Select(x => (int)x.NodeId).OrderBy(x => x).ToListAsync();
                                        bool toShow = false;
                                        if (rechableNodes.Any(x => x.GwSrn == itemNw))
                                        {
                                            toShow = true;
                                        }
                                        if (nonRechableNodes.Any(x => x.GwSrn == itemNw))
                                        {
                                            toShow = true;
                                        }

                                        if (gatewayNode.Any(x => x.GwSrn == itemNw))
                                        {
                                            toShow = true;
                                        }
                                        if (toShow)
                                        {
                                            NWRTU nWRTU = new NWRTU();
                                            nWRTU.NWTNO = itemNw;
                                            foreach (var item in nodeList)
                                            {
                                                var nrSchLst = NrSchLst.Where(x => x.RtuNo == item && x.NetworkNo == itemNw).FirstOrDefault();
                                                if (nrSchLst != null)
                                                {
                                                    nWRTU.UPID.Add((int)nrSchLst.UpId);
                                                }
                                                else
                                                {
                                                    int lsit = lstOfNodesToCheckRequired.Where(x => x.NodeId == item && x.NetworkNo == itemNw).Select(x => (int)x.ConfigUid).FirstOrDefault(); //await _mainDBContext.UpdateIdsRequired.Where(x => x.NodeId == item && x.NetworkNo == itemNw).Select(x => (int)x.ConfigUid).FirstOrDefaultAsync();
                                                    if (lsit > 0)
                                                    {
                                                        nWRTU.UPID.Add(0);
                                                    }

                                                    //nWRTU.UPID.Add(0);
                                                    //if (nWRTU.UPID.Count == 0)
                                                    //{
                                                    //    nWRTU.UPID = new List<int>();
                                                    //}
                                                }
                                            }
                                            //   nwrtuLst.Add(nWRTU);
                                            notiResponse.BODY.SCH_UPID.Add(nWRTU);

                                        }
                                    }

                                }

                                //notiResponse.BODY.SCH_UPID.Add(nwrtuLst);
                                //}
                                //else
                                //{
                                //    notiResponse.BODY.SCH_UPID = null;
                                //}



                                #endregion
                            }
                            else
                            {
                                notiResponse.BODY.SCH_UPID = null;
                            }


                        }


                        #endregion
                        _logger.LogInformation("Step 9--" + DateTime.Now);

                        if (CS.Count > 0 || SSensor.Count > 0 || VRT.Count > 0 || SEQ.Count > 0)
                        {
                            //ADD All Settings
                            if (CS.Count > 0)
                            {
                                notiResponse.BODY.CS = CS;
                            }
                            else
                            {
                                notiResponse.BODY.CS = null;
                            }

                            if (SSensor.Count > 0)
                            {
                                notiResponse.BODY.SS = SSensor;
                            }
                            else
                            {
                                notiResponse.BODY.SS = null;
                            }

                            if (VRT.Count > 0)
                            {
                                notiResponse.BODY.VRT = VRT;
                            }
                            else
                            {
                                notiResponse.BODY.VRT = null;
                            }
                            if (SEQ.Count > 0)
                            {
                                notiResponse.BODY.SQ_SET = SEQ;
                            }
                            else
                            {
                                notiResponse.BODY.SQ_SET = null;
                            }
                        }
                        else
                        {
                            if (noChange)
                            {
                                NotificationResponseBlankBody notificationResponseBlankBody = new NotificationResponseBlankBody();
                                HEAD = new List<dynamic>();
                                HEAD.Add("HB");
                                HEAD.Add(GWID);
                                HEAD.Add(PROJECT_ID);
                                HEAD.Add(DB_ID);
                                HEAD.Add(CMD_NO);
                                HEAD.Add(projUpdis.ProjectUpId);
                                HEAD.Add(TOTAL_NODE_IN_PROJECT);
                                HEAD.Add(PROGRAM_Day_END_MOD);
                                HEAD.Add(YEAR);
                                HEAD.Add(MONTH);
                                HEAD.Add(DATE);
                                HEAD.Add(HH);
                                HEAD.Add(MM);
                                HEAD.Add(SS);
                                HEAD.Add(WEEK_DA);
                                notiResponse.HEAD = HEAD;
                                notificationResponseBlankBody.HEAD = notiResponse.HEAD;
                                return notificationResponseBlankBody;
                            }
                            else
                            {
                                if (CS.Count > 0)
                                {
                                    notiResponse.BODY.CS = CS;
                                }
                                else
                                {
                                    notiResponse.BODY.CS = null;
                                }

                                if (SSensor.Count > 0)
                                {
                                    notiResponse.BODY.SS = SSensor;
                                }
                                else
                                {
                                    notiResponse.BODY.SS = null;
                                }

                                if (VRT.Count > 0)
                                {
                                    notiResponse.BODY.VRT = VRT;
                                }
                                else
                                {
                                    notiResponse.BODY.VRT = null;
                                }
                                if (SEQ.Count > 0)
                                {
                                    notiResponse.BODY.SQ_SET = SEQ;
                                }
                                else
                                {
                                    notiResponse.BODY.SQ_SET = null;
                                }
                                NotificationResponseBlankBody notificationResponseBlankBody = new NotificationResponseBlankBody();
                                HEAD = new List<dynamic>();
                                HEAD.Add("AN");
                                HEAD.Add(GWID);
                                HEAD.Add(PROJECT_ID);
                                HEAD.Add(DB_ID);
                                HEAD.Add(CMD_NO);
                                HEAD.Add(projUpdis.ProjectUpId);
                                HEAD.Add(TOTAL_NODE_IN_PROJECT);
                                HEAD.Add(PROGRAM_Day_END_MOD);
                                HEAD.Add(YEAR);
                                HEAD.Add(MONTH);
                                HEAD.Add(DATE);
                                HEAD.Add(HH);
                                HEAD.Add(MM);
                                HEAD.Add(SS);
                                HEAD.Add(WEEK_DA);
                                notiResponse.HEAD = HEAD;
                                notificationResponseBlankBody.HEAD = notiResponse.HEAD;
                                return notiResponse;
                            }

                        }
                    }



                }
                else
                {
                    NotificationResponseBlankBody notificationResponseBlankBody = new NotificationResponseBlankBody();
                    HEAD = new List<dynamic>();
                    HEAD.Add("HB");
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
                    notiResponse.HEAD = HEAD;
                    notificationResponseBlankBody.HEAD = notiResponse.HEAD;
                    return notificationResponseBlankBody;
                }
                _logger.LogInformation("Step 10--" + DateTime.Now);

                return notiResponse;
                // return Ok(CustomResponse.CreateResponse(true, "", "", 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(FramesService)}.{nameof(GetNotificationResponse)}]{ex}");
                throw ex;
            }
        }
        /// <summary>
        /// ResetUpids
        /// </summary>
        /// <param name="Gwid"></param>
        /// <returns></returns>
        public async Task<bool> ResetUpids(int Gwid)
        {
            try
            {
                List<UpdateIds> upids = new List<UpdateIds>();
                List<GatewayMaxSch> gatewayMaxSches = new List<GatewayMaxSch>();

                upids = _mainDBContext.UpdateIds.Where(w => w.Gwid == Gwid).ToList();
                gatewayMaxSches = _mainDBContext.GatewayMaxSch.Where(w => w.GatewayNo == Gwid).ToList();

                upids = upids.Where(w => w.Gwid == Gwid).Select(w => { w.NodeUid = 0; w.ConfigUid = 0; w.VrtUid = 0; w.SensorUid = 0; return w; }).ToList();
                gatewayMaxSches = gatewayMaxSches.Where(w => w.GatewayNo == Gwid).Select(w => { w.MaxSchUpId = 0; return w; }).ToList();

                _mainDBContext.UpdateIds.UpdateRange(upids);
                _mainDBContext.SaveChanges();

                _mainDBContext.GatewayMaxSch.UpdateRange(gatewayMaxSches);
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// ChangeHexOrder
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        static string ChangeHexOrder(string s)
        {
            char[] arr = s.ToCharArray();
            char tmp;
            //change positions of [i, i + 1 , , , , , ,length - i - 2, length - i - 1]
            for (int i = 0; i < arr.Length / 2; i += 2)
            {
                tmp = arr[i];
                arr[i] = arr[arr.Length - i - 2];
                arr[arr.Length - i - 2] = tmp;

                tmp = arr[i + 1];
                arr[i + 1] = arr[arr.Length - i - 1];
                arr[arr.Length - i - 1] = tmp;
            }
            return new String(arr);
        }
        /// <summary>
        /// Get Interval bytes or week byte for which the sequence is running
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="seqId"></param>
        /// <param name="basisOfOP"></param>
        /// <param name="intervalDays"></param>
        /// <returns></returns>
        #region Get Interval bytes or week byte for which the sequence is running
        private string GetIntervalByteOrWeekByte(List<NewSequenceWeeklySchedule> dt, int seqId, int basisOfOP, int intervalDays)
        {
            if (dt.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                string[] arr_week = new string[7];
                for (int i = 0; i < arr_week.Length; i++)
                {
                    arr_week[i] = "0";
                }

                for (int i = 7; i >= 1; i--)
                {
                    //IEnumerable<DataRow> rows = dt.AsEnumerable()
                    //.Where(r => r.Field<int>("seqid") == seqId && r.Field<int>("weekdayid") == i);
                    if (dt.Any(x => x.SeqId == seqId && x.WeekDayId == i))
                    {
                        arr_week[7 - i] = "1";
                    }
                }
                sb.Append("0");
                for (int arrCount = 0; arrCount < arr_week.Length; arrCount++)
                {
                    sb.Append(arr_week[arrCount]);
                }
                return ConvertNotesToDecHex(sb.ToString()).ToString();
            }
            else
            {
                return intervalDays.ToString("X").PadLeft(2, '0');
            }
        }
        #endregion
        /// <summary>
        /// convert binary string to decimal & then to hex
        /// </summary>
        /// <param name="strBinary"></param>
        /// <returns></returns>
        #region convert binary string to decimal & then to hex
        private string ConvertNotesToDecHex(string strBinary)
        {
            StringBuilder sb_hex = new StringBuilder();
            for (int i = 0; i < strBinary.Length; i = i + 4)
            {
                string dec = Convert.ToInt32(strBinary.Substring(i, 4), 2).ToString();
                sb_hex.Append(int.Parse(dec).ToString("X"));
            }
            return sb_hex.ToString();
        }
        #endregion
        /// <summary>
        /// Get minues from : string
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        #region Get minues from : string
        private int GetMinFromTimeString(string strTime)
        {
            string[] arr_Time = strTime.Split(':');
            int min = (int.Parse(arr_Time[0]) * 60) + int.Parse(arr_Time[1]);
            return min;
        }

        #endregion
        /// <summary>
        /// ConvertFromIpAddressToInteger
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static decimal ConvertFromIpAddressToInteger(string ipAddress)
        {
            var address = IPAddress.Parse(ipAddress);
            byte[] bytes = address.GetAddressBytes();

            // flip big-endian(network order) to little-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToUInt32(bytes, 0);
        }
        /// <summary>
        /// ConvertFromIntegerToIpAddress
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static string ConvertFromIntegerToIpAddress(uint ipAddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipAddress);

            // flip little-endian to big-endian(network order)
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return new IPAddress(bytes).ToString();
        }
        /// <summary>
        /// HexTime
        /// </summary>
        /// <param name="str_time"></param>
        /// <returns></returns>
        public string HexTime(string str_time)
        {
            StringBuilder convertedTime = new StringBuilder();
            string[] arr_Time = str_time.Split(':');

            int min = (int.Parse(arr_Time[0]) * 60) + int.Parse(arr_Time[1]);
            convertedTime.Append(min.ToString("X").PadLeft(4, '0'));
            return convertedTime.ToString();
        }
        /// <summary>
        /// HexTimeForLE
        /// </summary>
        /// <param name="str_time"></param>
        /// <returns></returns>
        public int HexTimeForLE(string str_time)
        {
            StringBuilder convertedTime = new StringBuilder();
            string[] arr_Time = str_time.Split(':');

            int min = (int.Parse(arr_Time[0]) * 60) + int.Parse(arr_Time[1]);
            return min;
        }
        /// <summary>
        /// GetTimeForSchedule
        /// </summary>
        /// <param name="StartTime"></param>
        /// <returns></returns>
        public string GetTimeForSchedule(string StartTime)
        {
            StringBuilder convertedTime = new StringBuilder();
            string[] arr_Time = StartTime.Split(':');
            string Hr = DecimalTo2Hex(Convert.ToInt32(arr_Time[0]));
            string min = DecimalTo2Hex(Convert.ToInt32(arr_Time[1]));
            return Hr + min;
        }
        /// <summary>
        /// DecimalToHex
        /// </summary>
        /// <param name="sHexValue"></param>
        /// <returns></returns>
        private string DecimalToHex(int sHexValue)
        {
            return sHexValue.ToString("x");
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
        /// HexToFloatDecimal
        /// </summary>
        /// <param name="sHexValue"></param>
        /// <returns></returns>
        private decimal HexToFloatDecimal(string sHexValue)
        {
            decimal iNumber = decimal.Parse(sHexValue, System.Globalization.NumberStyles.HexNumber);
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
        /// DecimalTo15Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo15Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(15, '0'); ;
        }
        /// <summary>
        /// DecimalTo16Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo16Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(16, '0'); ;
        }
        /// <summary>
        /// DecimalTo30Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo30Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(30, '0'); ;
        }
        /// <summary>
        /// DecimalTo24Hex
        /// </summary>
        /// <param name="sDecValue"></param>
        /// <returns></returns>
        private string DecimalTo24Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(24, '0'); ;
        }
        private string DecimalTo26Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(26, '0'); ;
        }

        private string DecimalTo40Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(40, '0'); ;
        }
        private string DecimalTo13Hex(int sDecValue)
        {
            return sDecValue.ToString("x").PadLeft(13, '0'); ;
        }


        //private string DecimalTo4Hex(int sDecValue)
        //{
        //    return sDecValue.ToString("X").PadLeft(4, '0');
        //}

        private string DecimalTo20Hex(long sDecValue)
        {
            return sDecValue.ToString("X").PadLeft(20, '0');
        }

        private string DecimalTo8Hex(int sDecValue)
        {
            return sDecValue.ToString("X").PadLeft(8, '0');
        }

        private string DecimalTo32Hex(int sDecValue)
        {
            return sDecValue.ToString("X").PadLeft(32, '0');
        }

        private string DecimalTo36Hex(string sDecValue)
        {
            return sDecValue.ToString().PadLeft(36, '0');
        }

        private string DecimalTo34Hex(string sDecValue)
        {
            return sDecValue.ToString().PadLeft(34, '0');
        }

        private string DecimalTo50Hex(string sDecValue)
        {
            return sDecValue.ToString().PadLeft(50, '0');
        }

        string MobileCorrecton(string mobile)
        {
            string mob = "00000000000000000000";
            if (mobile == "0")
            {
                return mob = "00000000000000000000";
            }
            char[] charArr = mobile.ToCharArray();
            if (charArr.Length > 0)
            {
                mob = "";
            }
            if (charArr.Length < 10)
            {
                return mob = "00000000000000000000";
            }
            foreach (var item in charArr)
            {
                mob = mob + DecimalTo2Hex(Convert.ToInt32(item.ToString()));
            }

            return mob;
        }
        string ToLSBHexString(float f)
        {
            //var bytes = BitConverter.GetBytes(f);
            //var i = BitConverter.ToInt32(bytes, 0);
            //return "0x" + i.ToString("X8");         
            byte[] bytes = BitConverter.GetBytes(f);
            Array.Reverse(bytes);
            int i = BitConverter.ToInt32(bytes, 0);
            var ss = DecimalTo8Hex(i);
            return ss;
            //return i.ToString("X4");
        }
        static string LittleEndian2X(int num)
        {
            int target = num; // 0x00FA
            // swap the bytes of target
            target = IPAddress.HostToNetworkOrder((short)target) & 0xFFFF;
            // target now is 0xFA00
            string hexString = target.ToString("X2");
            return hexString;
        }
        static string LittleEndian4X(int num)
        {
            int target = num; // 0x00FA
            // swap the bytes of target
            target = IPAddress.HostToNetworkOrder((short)target) & 0xFFFF;
            // target now is 0xFA00
            string hexString = target.ToString("X4");
            return hexString;
        }

        static string LittleEndian8X(int num)
        {
            int target = num; // 0x00FA
            // swap the bytes of target
            target = IPAddress.HostToNetworkOrder((short)target) & 0xFFFF;
            // target now is 0xFA00
            string hexString = target.ToString("X8");
            return hexString;
        }

        static string LittleEndian20X(long num)
        {
            long target = num; // 0x00FA
            // swap the bytes of target
            target = IPAddress.HostToNetworkOrder((long)target) & 0xFFFFFFFFFF;
            // target now is 0xFA00
            string hexString = target.ToString("X20");
            return hexString;
        }

        static string LittleEndian32X(long num)
        {
            long target = num; // 0x00FA
            // swap the bytes of target
            target = IPAddress.HostToNetworkOrder((short)target) & 0xFFFF;
            // target now is 0xFA00
            string hexString = target.ToString("X32");
            return hexString;
        }
        float FromLSBHexString(string s)
        {
            var i = Convert.ToInt32(s, 16);
            var bytes = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(bytes, 0);
        }

        public int littleEndianHexStringToDecimal(string hex)
        {
            int ret = 0;
            var len = hex.Length;
            var bigEndianHexString = "0x";
            if (hex.Length % 2 != 0) return ret;
            for (int i = hex.Length - 2; i >= 0; i -= 2)
            {
                bigEndianHexString += hex.Substring(i, i + 2);
            }
            ret = Convert.ToInt32(bigEndianHexString);
            return ret;
        }

        public async Task<PagedList<MultiDataLogger>> GetMultiDataLogger(ResourceParameter resourceParameter)
        {
            try
            {
                var datalog = _mainDBContext.MultiDataLogger.AsQueryable();
                return PagedList<MultiDataLogger>.Create(datalog, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// GetNodeNetworkDataFrameByDate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PagedList<MultiDataLogger>> GetDataLoggerByDate(PostEventsDataLlogger model)
        {
            string pathFolder = "DataLoggerOutput";
            string filePath = "";
            List<MultiDataLogger> list = new List<MultiDataLogger>();
            try
            {

                var lists = _mainDBContext.MultiDataLogger.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime && x.GwId == model.GwidNo).OrderBy(x => x.AddedDateTime).AsQueryable();
                //Merged files here
                //Server Path
                string filename = "Datalog_Op.txt";

                filePath = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder, filename);
                string ServerUrl = _config["DataLoggerServerPath"];
                string pathServerFolder = ServerUrl + DbManager.SiteName;
                _logger.LogInformation("Server Path: " + pathServerFolder);
                //Local path

                string ATTACHMENTS_FOLDER_PATH = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder);
                _logger.LogInformation("Local Path: " + ATTACHMENTS_FOLDER_PATH);
                if (!Directory.Exists(ATTACHMENTS_FOLDER_PATH))
                {
                    Directory.CreateDirectory(ATTACHMENTS_FOLDER_PATH);
                }

                _logger.LogInformation("OP Local Path: " + filePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                using (TextWriter txtWrt = new StreamWriter(filePath))
                {
                    foreach (var item in lists)
                    {
                        _logger.LogInformation("Server Files Path: " + Path.Combine(pathServerFolder, item.Message));
                        string[] s1 = File.ReadLines(Path.Combine(pathServerFolder, item.Message)).ToArray<string>();
                        for (int i = 0; i < s1.Length; i++)
                        {
                            txtWrt.WriteLine(s1[i]);
                        }

                    }
                    txtWrt.Close();
                }

                return PagedList<MultiDataLogger>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {
                File.Create(filePath).Dispose();
                throw;
            }
        }

        public async Task<IQueryable<GatewayDDLViewModel>> GetGatewayListForDataLogger()
        {
            List<GatewayDDLViewModel> list = new List<GatewayDDLViewModel>();
            try
            {
                var lists = _mainDBContext.MultiDataLogger.Select(x => new GatewayDDLViewModel { Id = (int)x.GwId, Name = x.GwId.ToString() }).AsQueryable();
                return lists;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// GetGatewayBySrNo
        /// </summary>
        /// <param name="serialo"></param>
        /// <returns></returns>
        public async Task<Gateway> GetGatewayBySrNo(int serialo)
        {
            try
            {
                Gateway gateway = await _mainDBContext.Gateway.Where(x => x.SerialNo == serialo).FirstOrDefaultAsync();
                if (gateway != null)
                {
                    return gateway;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// SaveDataLogger
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> SaveDataLogger(string model)
        {

            try
            {


                // model = "ST0001000027110000000100010100C8DLfnjn32546j84ghfuhb hguvh5;gm5";
                MultiDataLogger multiDataLogger = new MultiDataLogger();
                var item = model.Split("DL");

                int placeNo = 2;
                multiDataLogger.GwId = HexToDecimal(item[0].Substring(placeNo, 4));
                multiDataLogger.ProjectId = HexToDecimal(item[0].Substring(6, 8));
                multiDataLogger.Dbid = HexToDecimal(item[0].Substring(14, 8));
                multiDataLogger.Cmdno = HexToDecimal(item[0].Substring(22, 4));
                multiDataLogger.NwUpid = HexToDecimal(item[0].Substring(26, 2));
                multiDataLogger.MaxLengthKb = HexToDecimal(item[0].Substring(28, 4));
                string filename = "Datalog_" + DbManager.SiteName + "_" + DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + DateTime.Now.Millisecond + ".txt";
                multiDataLogger.Message = filename;
                string MessageData = "";
                if (item.Length > 1)
                {
                    for (int i = 0; i < item.Length - 1; i++)
                    {
                        MessageData = MessageData + "" + item[i + 1];
                    }
                }
                multiDataLogger.AddedDateTime = DateTime.Now;
                await _mainDBContext.MultiDataLogger.AddAsync(multiDataLogger);
                await _mainDBContext.SaveChangesAsync();

                string pathFolder = "Multi_DataLogger\\" + DbManager.SiteName;
                string ATTACHMENTS_FOLDER_PATH = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder);
                if (!Directory.Exists(ATTACHMENTS_FOLDER_PATH))
                {
                    Directory.CreateDirectory(ATTACHMENTS_FOLDER_PATH);
                }



                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, pathFolder, filename);
                if (File.Exists(filePath))
                {
                    using (StreamWriter writer = System.IO.File.AppendText(filePath))
                    {
                        writer.WriteLine(multiDataLogger.AddedDateTime);
                        writer.WriteLine("GatewayId : " + multiDataLogger.GwId + " " + "Project ID : " + multiDataLogger.ProjectId);
                        writer.WriteLine("Message : " + MessageData);
                        writer.WriteLine("__________________________________________________________________________________________________");
                        writer.Close();
                    }
                }
                else
                {
                    using (StreamWriter writer = System.IO.File.CreateText(filePath))
                    {
                        writer.WriteLine(multiDataLogger.AddedDateTime);
                        writer.WriteLine("GatewayId : " + multiDataLogger.GwId + " " + "Project ID : " + multiDataLogger.ProjectId);
                        writer.WriteLine("Message : " + MessageData);
                        writer.WriteLine("__________________________________________________________________________________________________");
                        writer.Close();
                    }

                }

                return "success";
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool DeleteAlldataLog()
        {
            try
            {
                List<MultiDataLogger> fileAttachments = _mainDBContext.MultiDataLogger.Where(x => x.AddedDateTime < DateTime.Now.AddDays(-30)).ToList();
                string ServerUrl = _config["DataLoggerServerPath"];
                string pathServerFolder = ServerUrl + DbManager.SiteName;
                foreach (var itemAttachment in fileAttachments)
                {
                    string filePath = Path.Combine(pathServerFolder, itemAttachment.Message);

                    if (itemAttachment != null)
                    {

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                            }
                            catch (Exception ex)
                            {
                                //Do something
                            }
                        }
                    }
                }

                _mainDBContext.MultiDataLogger.RemoveRange(fileAttachments);
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeletedataLog(int id)
        {
            try
            {
                MultiDataLogger fileAttachment = _mainDBContext.MultiDataLogger.Where(x => x.Id == id).FirstOrDefault();
                string ServerUrl = _config["DataLoggerServerPath"];
                string pathServerFolder = ServerUrl + DbManager.SiteName;

                string filePath = Path.Combine(pathServerFolder, fileAttachment.Message);

                if (fileAttachment != null)
                {

                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            //Do something
                        }
                    }
                }
                _mainDBContext.MultiDataLogger.Remove(fileAttachment);
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// SaveEvents
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> SaveEvents(List<dynamic> model, int GatewayId)
        {
            try
            {
                List<MultiRtuAnalysis> multiRtuAnalysis = new List<MultiRtuAnalysis>();
                List<MultiValveEvent> MultiValveEventLst = new List<MultiValveEvent>();
                List<MultiSensorEvent> MultiSensorEventLst = new List<MultiSensorEvent>();
                List<MultiNodeNwDataFrame> MultiNodeNwDataFrameLst = new List<MultiNodeNwDataFrame>();
                List<MultiNodeJoinDataFrame> MultiNodeJoinDataFrameLst = new List<MultiNodeJoinDataFrame>();
                List<GwstatusData> GwstatusDataLst = new List<GwstatusData>();
                List<MultiValveAlarmData> MultiValveAlarmDataLst = new List<MultiValveAlarmData>();
                List<MultiSensorAlarmData> MultiSensorAlarmDataLst = new List<MultiSensorAlarmData>();
                List<MultiNodeAlarm> MultiNodeAlarmDataLst = new List<MultiNodeAlarm>();
                List<MultiHandShakeNonReach> MultiHandShakeNonReach = new List<MultiHandShakeNonReach>();
                List<MultiHandShakeReach> MultiHandShakeReach = new List<MultiHandShakeReach>();

                foreach (string item in model)
                {
                    int placeNo = 1;
                    //Total Bytes
                    int totalBytes = HexToDecimal(item.Substring(placeNo, 4));
                    //Frame Type
                    int frameType = HexToDecimal(ChangeHexOrder(item.Substring(5, 4)));
                    if (frameType == 3) //MultiValveEvent
                    {
                        //MultiValveEventRawProcessing multiValveEventRawProcessing = new MultiValveEventRawProcessing();
                        //multiValveEventRawProcessing.Frame = item;
                        //multiValveEventRawProcessing.IsProcessed = false;
                        //multiValveEventRawProcessing.AddedDateTime = DateTime.Now;
                        //_mainDBContext.MultiValveEventRawProcessing.Add(multiValveEventRawProcessing);
                        //_mainDBContext.SaveChanges();


                        MultiValveEvent multiValveEvent = new MultiValveEvent();
                        multiValveEvent.FrameType = frameType;
                        multiValveEvent.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        multiValveEvent.UpdataeTimeSow = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));// HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                        multiValveEvent.ValveNo = HexToDecimal(ChangeHexOrder(item.Substring(21, 2)));
                        multiValveEvent.ValveType = HexToDecimal(ChangeHexOrder(item.Substring(23, 2)));
                        multiValveEvent.RequiredState = HexToDecimal(ChangeHexOrder(item.Substring(25, 2)));
                        multiValveEvent.CurrentState = HexToDecimal(ChangeHexOrder(item.Substring(27, 2)));
                        multiValveEvent.CurrentStateReason = HexToDecimal(ChangeHexOrder(item.Substring(29, 2)));
                        multiValveEvent.OperationTimeMoy = ConvertMOYToDateTime(HexToDecimal(ChangeHexOrder(item.Substring(31, 8))));
                        multiValveEvent.GwoperationTimeMoy = HexToDecimal(ChangeHexOrder(item.Substring(31, 8)));
                        multiValveEvent.ActiveCurrent = HexToDecimal(ChangeHexOrder(item.Substring(39, 8)));
                        multiValveEvent.AddedDateTime = DateTime.Now;
                        multiValveEvent.NetworkNo = multiValveEvent.NodeId >> 10;
                        multiValveEvent.NodeNo = multiValveEvent.NodeId & 1023;
                        multiValveEvent.GatewayId = GatewayId;
                        MultiValveEventLst.Add(multiValveEvent);
                        //await _mainDBContext.MultiValveEvent.AddAsync(multiValveEvent);
                        //await _mainDBContext.SaveChangesAsync();
                        _logger.LogInformation("Valve event Frame Saved -multiValveEvent" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiValveEvent.NodeId, "Valve Event", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                    }
                    else if (frameType == 4)//MultiSensorEvent
                    {
                        MultiSensorEvent multiSensorEvent = new MultiSensorEvent();
                        multiSensorEvent.FrameType = frameType;
                        multiSensorEvent.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        multiSensorEvent.Ssno = HexToDecimal(ChangeHexOrder(item.Substring(13, 2)));
                        multiSensorEvent.Sstype = HexToDecimal(ChangeHexOrder(item.Substring(15, 2)));
                        multiSensorEvent.Sspriority = HexToDecimal(ChangeHexOrder(item.Substring(17, 2)));
                        multiSensorEvent.Samprate = HexToDecimal(ChangeHexOrder(item.Substring(19, 2)));
                        multiSensorEvent.LastAtmSoy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(21, 8))));  //HexToDecimal(ChangeHexOrder(item.Substring(21, 8))).ToString();
                        multiSensorEvent.Utsoy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(29, 8)))); //HexToDecimal(ChangeHexOrder(item.Substring(29, 8))).ToString();
                        multiSensorEvent.StoreSoy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(37, 8)))); //HexToDecimal(ChangeHexOrder(item.Substring(37, 8))).ToString();

                        multiSensorEvent.GwlastAtmSoy = HexToDecimal(ChangeHexOrder(item.Substring(21, 8)));
                        multiSensorEvent.Gwutsoy = HexToDecimal(ChangeHexOrder(item.Substring(29, 8)));
                        multiSensorEvent.GwstoreSoy = HexToDecimal(ChangeHexOrder(item.Substring(37, 8)));
                        //multiSensorEvent.LastAtmSoy = ChangeHexOrder(item.Substring(46, 8)).ToString();
                        string sensorData = item.Substring(45, 30);
                        int placeSensorNo = 0;
                        if (multiSensorEvent.Sstype == 2)
                        {
                            multiSensorEvent.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                            multiSensorEvent.SensorValue = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(2, 8))));
                        }
                        else if (multiSensorEvent.Sstype == 3)
                        {
                            multiSensorEvent.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                            multiSensorEvent.SensorValue = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(2, 8))));
                        }
                        else if (multiSensorEvent.Sstype == 4)
                        {   //Digital NO_NC Type Sensor Data		
                            multiSensorEvent.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                            multiSensorEvent.Cstate = HexToDecimal(sensorData.Substring(2, 2));
                            multiSensorEvent.DefaultState = HexToDecimal(sensorData.Substring(4, 2));
                        }
                        else if (multiSensorEvent.Sstype == 5)
                        {
                            multiSensorEvent.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                            multiSensorEvent.CummulativeCount = Convert.ToDecimal(HexToDecimal(ChangeHexOrder((sensorData.Substring(2, 8)))));
                            multiSensorEvent.Frequency = Convert.ToDecimal(ChangeHexOrder(sensorData.Substring(10, 8)));
                        }
                        else if (multiSensorEvent.Sstype == 37)
                        {
                            multiSensorEvent.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                            multiSensorEvent.CummulativeCount = Convert.ToDecimal(HexToDecimal(ChangeHexOrder((sensorData.Substring(2, 8)))));
                            multiSensorEvent.Frequency = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(10, 8))));

                        }
                        multiSensorEvent.AddedDateTime = DateTime.Now;
                        multiSensorEvent.NetworkNo = multiSensorEvent.NodeId >> 10;
                        multiSensorEvent.NodeNo = multiSensorEvent.NodeId & 1023;
                        multiSensorEvent.GatewayId = GatewayId;
                        MultiSensorEventLst.Add(multiSensorEvent);
                        //await _mainDBContext.MultiSensorEvent.AddAsync(multiSensorEvent);
                        //await _mainDBContext.SaveChangesAsync();
                        _logger.LogInformation("Sensor event Frame Saved -multiSensorEvent" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiSensorEvent.NodeId, "Sensor Event", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                    }
                    else if (frameType == 774) //Multi Node Nw Data Frame
                    {
                        MultiNodeNwDataFrame multiNodeNwDataFrame = new MultiNodeNwDataFrame();
                        multiNodeNwDataFrame.FrameType = frameType;
                        multiNodeNwDataFrame.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        multiNodeNwDataFrame.GwLastCommTime = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                        multiNodeNwDataFrame.LastCommTime = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        multiNodeNwDataFrame.GwLastCommTime = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                        multiNodeNwDataFrame.RxFrametype = HexToDecimal(ChangeHexOrder(item.Substring(21, 2)));
                        multiNodeNwDataFrame.NgcurrentRssi = HexToDecimal(ChangeHexOrder(item.Substring(23, 4)));
                        multiNodeNwDataFrame.NgcurrentSnr = HexToDecimal(ChangeHexOrder(item.Substring(27, 2)));
                        multiNodeNwDataFrame.NgcurrentSf = HexToDecimal(ChangeHexOrder(item.Substring(29, 2)));
                        multiNodeNwDataFrame.Ngfreq = HexToDecimal(ChangeHexOrder(item.Substring(31, 2)));
                        multiNodeNwDataFrame.NgcurrentCr = HexToDecimal(ChangeHexOrder(item.Substring(33, 2)));
                        multiNodeNwDataFrame.Gwid6b = HexToDecimal(ChangeHexOrder(item.Substring(35, 2)));
                        multiNodeNwDataFrame.Ngattempt3b = HexToDecimal(ChangeHexOrder(item.Substring(37, 2)));
                        multiNodeNwDataFrame.Power2b = HexToDecimal(ChangeHexOrder(item.Substring(39, 2)));
                        multiNodeNwDataFrame.Sfgw2b = HexToDecimal(ChangeHexOrder(item.Substring(41, 2)));
                        multiNodeNwDataFrame.GnsnrPrevious3b = HexToDecimal(ChangeHexOrder(item.Substring(43, 2)));
                        multiNodeNwDataFrame.GnrssiPrevious3b = HexToDecimal(ChangeHexOrder(item.Substring(45, 2)));
                        multiNodeNwDataFrame.ProductType4b = HexToDecimal(ChangeHexOrder(item.Substring(47, 2)));
                        multiNodeNwDataFrame.AddedDatetime = DateTime.Now;
                        multiNodeNwDataFrame.NetworkNo = multiNodeNwDataFrame.NodeId >> 10;
                        multiNodeNwDataFrame.NodeNo = multiNodeNwDataFrame.NodeId & 1023;
                        multiNodeNwDataFrame.GatewayId = GatewayId;

                        MultiNodeNwDataFrameLst.Add(multiNodeNwDataFrame);
                        //await _mainDBContext.MultiNodeNwDataFrame.AddAsync(multiNodeNwDataFrame);
                        //await _mainDBContext.SaveChangesAsync();
                        _logger.LogInformation("Sensor event Frame Saved multiNodeNwDataFrame-" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiNodeNwDataFrame.NodeId, "Node Network Data", 0, GatewayId, multiNodeNwDataFrame.NgcurrentSf, multiNodeNwDataFrame.NgcurrentSnr, multiNodeNwDataFrame.NgcurrentRssi, multiNodeNwDataFrame.Sfgw2b, multiNodeNwDataFrame.GnsnrPrevious3b, multiNodeNwDataFrame.GnrssiPrevious3b, 0));
                    }
                    else if (frameType == 775) // Multi Node Join Data Frame
                    {
                        MultiNodeJoinDataFrame multiNodeJoinDataFrame = new MultiNodeJoinDataFrame();
                        multiNodeJoinDataFrame.FrameType = frameType;
                        multiNodeJoinDataFrame.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        multiNodeJoinDataFrame.LastCommTime = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        multiNodeJoinDataFrame.GwLastCommTime = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                        multiNodeJoinDataFrame.DeviceResetCause = HexToDecimal(ChangeHexOrder(item.Substring(21, 2)));
                        multiNodeJoinDataFrame.Attempt = HexToDecimal(ChangeHexOrder(item.Substring(23, 2)));
                        multiNodeJoinDataFrame.ProjectId = HexToDecimal(ChangeHexOrder(item.Substring(25, 4)));
                        multiNodeJoinDataFrame.TechId = HexToDecimal(ChangeHexOrder(item.Substring(29, 4)));
                        multiNodeJoinDataFrame.NodeNo = HexToDecimal(ChangeHexOrder(item.Substring(33, 4)));
                        multiNodeJoinDataFrame.Gwno1 = HexToDecimal(ChangeHexOrder(item.Substring(37, 2)));
                        multiNodeJoinDataFrame.Gwno2 = HexToDecimal(ChangeHexOrder(item.Substring(39, 2)));
                        multiNodeJoinDataFrame.Gwno3 = HexToDecimal(ChangeHexOrder(item.Substring(41, 2)));
                        multiNodeJoinDataFrame.Gwno4 = HexToDecimal(ChangeHexOrder(item.Substring(43, 2)));
                        multiNodeJoinDataFrame.Latitude = Convert.ToDecimal(HextoFloat(ChangeHexOrder(item.Substring(45, 8))));
                        multiNodeJoinDataFrame.Longitude = Convert.ToDecimal(HextoFloat(ChangeHexOrder(item.Substring(53, 8))));//  HexToDecimal(ChangeHexOrder(item.Substring(53, 8)));
                        multiNodeJoinDataFrame.Moy = ConvertMOYToDateTime(HexToDecimal(ChangeHexOrder(item.Substring(61, 8))));
                        multiNodeJoinDataFrame.Gwmoy = HexToDecimal(ChangeHexOrder(item.Substring(61, 8)));
                        multiNodeJoinDataFrame.Seconds = HexToDecimal(ChangeHexOrder(item.Substring(69, 2)));
                        multiNodeJoinDataFrame.Time = HexToDecimal(ChangeHexOrder(item.Substring(71, 2)));
                        multiNodeJoinDataFrame.Year = HexToDecimal(ChangeHexOrder(item.Substring(73, 2)));
                        multiNodeJoinDataFrame.LatLongAccuracy = HexToDecimal(ChangeHexOrder(item.Substring(75, 8)));
                        multiNodeJoinDataFrame.DeviceSrno = HexToDecimal(ChangeHexOrder(item.Substring(83, 8)));
                        multiNodeJoinDataFrame.AddedDateTime = DateTime.Now;
                        multiNodeJoinDataFrame.NetworkNo = multiNodeJoinDataFrame.NodeId >> 10;
                        multiNodeJoinDataFrame.NodeNo = multiNodeJoinDataFrame.NodeId & 1023;
                        multiNodeJoinDataFrame.GatewayId = GatewayId;
                        MultiNodeJoinDataFrameLst.Add(multiNodeJoinDataFrame);

                        int nodenoForDB = (int)multiNodeJoinDataFrame.NodeId & 1023;
                        int shifts = (int)multiNodeJoinDataFrame.NodeId >> 10;
                        //Check lat long and add in table or update
                        if (_mainDBContext.MultiNodeLatLong.Any(x => x.NodeId == (int)multiNodeJoinDataFrame.NodeId))
                        {
                            var multinodedata = await _mainDBContext.MultiNodeLatLong.Where(x => x.NodeId == multiNodeJoinDataFrame.NodeId).FirstOrDefaultAsync();
                            if (multinodedata.Latitude != multiNodeJoinDataFrame.Latitude || multinodedata.Longitude != multiNodeJoinDataFrame.Longitude)
                            {
                                multinodedata.Latitude = multiNodeJoinDataFrame.Latitude;
                                multinodedata.Longitude = multiNodeJoinDataFrame.Longitude;
                                _mainDBContext.MultiNodeLatLong.Update(multinodedata);
                                await _mainDBContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            MultiNodeLatLong multiNodeLatLong = new MultiNodeLatLong();
                            multiNodeLatLong.Latitude = multiNodeJoinDataFrame.Latitude;
                            multiNodeLatLong.Longitude = multiNodeJoinDataFrame.Longitude;
                            multiNodeLatLong.ManualLatitude = multiNodeJoinDataFrame.Latitude;
                            multiNodeLatLong.ManualLongitude = multiNodeJoinDataFrame.Longitude;
                            multiNodeLatLong.NodeId = multiNodeJoinDataFrame.NodeId;
                            multiNodeLatLong.NetworkNo = shifts;
                            await _mainDBContext.MultiNodeLatLong.AddAsync(multiNodeLatLong);
                            await _mainDBContext.SaveChangesAsync();
                        }
                        _logger.LogInformation("Sensor event Frame Saved multiNodeJoinDataFrame-" + item);
                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiNodeJoinDataFrame.NodeId, "Node Join Data", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                    }
                    else if (frameType == 776) // Hand Shake non reach
                    {
                        MultiHandShakeNonReach modelToAdd = new MultiHandShakeNonReach();
                        modelToAdd.FrameType = frameType;
                        modelToAdd.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        modelToAdd.Soy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        modelToAdd.VerNumber = HexToDecimal(ChangeHexOrder(item.Substring(21, 4)));
                        modelToAdd.ConfigVerNumber = HexToDecimal(ChangeHexOrder(item.Substring(25, 2)));
                        modelToAdd.ScheduleUpno = HexToDecimal(ChangeHexOrder(item.Substring(27, 2)));
                        modelToAdd.SensorUpno = HexToDecimal(ChangeHexOrder(item.Substring(29, 2)));
                        modelToAdd.TempReading = HexToDecimal(ChangeHexOrder(item.Substring(31, 2)));
                        modelToAdd.BatteryVoltReading = HexToDecimal(ChangeHexOrder(item.Substring(33, 2)));
                        modelToAdd.LvsStartTime = HexToDecimal(ChangeHexOrder(item.Substring(35, 8)));
                        modelToAdd.LvsDuration = HexToDecimal(ChangeHexOrder(item.Substring(43, 2)));
                        modelToAdd.LvsOperation = HexToDecimal(ChangeHexOrder(item.Substring(45, 2)));
                        modelToAdd.LvsValveNo = HexToDecimal(ChangeHexOrder(item.Substring(47, 2)));
                        modelToAdd.LvsRop = HexToDecimal(ChangeHexOrder(item.Substring(49, 2)));
                        modelToAdd.MoUpno = HexToDecimal(ChangeHexOrder(item.Substring(51, 2)));
                        modelToAdd.ValveStatus = HexToDecimal(ChangeHexOrder(item.Substring(53, 2)));
                        modelToAdd.Distatus = HexToDecimal(ChangeHexOrder(item.Substring(55, 2)));
                        modelToAdd.AddedDatetime = DateTime.Now;
                        modelToAdd.NetworkNo = modelToAdd.NodeId >> 10;
                        modelToAdd.NodeNo = modelToAdd.NodeId & 1023;
                        modelToAdd.GatewayId = GatewayId;
                        MultiHandShakeNonReach.Add(modelToAdd);
                        _logger.LogInformation("Sensor event Frame Saved MultiHandShakeNonReach-" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)modelToAdd.NodeId, "Handshake Non Reach", Convert.ToDecimal(modelToAdd.TempReading), GatewayId, 0, 0, 0, 0, 0, 0, Convert.ToDecimal(modelToAdd.BatteryVoltReading)));
                    }
                    else if (frameType == 777) // Hand shake reach
                    {
                        MultiHandShakeReach modelToAdd = new MultiHandShakeReach();
                        modelToAdd.FrameType = frameType;
                        modelToAdd.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        modelToAdd.Soy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        modelToAdd.VerNumber = HexToDecimal(ChangeHexOrder(item.Substring(21, 4)));
                        modelToAdd.ConfigVerNumber = HexToDecimal(ChangeHexOrder(item.Substring(25, 2)));
                        modelToAdd.ScheduleUpno = HexToDecimal(ChangeHexOrder(item.Substring(27, 2)));
                        modelToAdd.SensorUpno = HexToDecimal(ChangeHexOrder(item.Substring(29, 2)));
                        modelToAdd.FbwConfigUpno = HexToDecimal(ChangeHexOrder(item.Substring(31, 2)));
                        modelToAdd.TempReading = HexToDecimal(ChangeHexOrder(item.Substring(33, 2)));
                        modelToAdd.BatteryVoltReading = HexToDecimal(ChangeHexOrder(item.Substring(35, 2)));
                        modelToAdd.LvsStartTime = HexToDecimal(ChangeHexOrder(item.Substring(37, 8)));
                        modelToAdd.LvsDuration = HexToDecimal(ChangeHexOrder(item.Substring(45, 4)));
                        modelToAdd.LvsOperation = HexToDecimal(ChangeHexOrder(item.Substring(49, 2)));
                        modelToAdd.LvsValveNo = HexToDecimal(ChangeHexOrder(item.Substring(51, 2)));
                        modelToAdd.LvsRop = HexToDecimal(ChangeHexOrder(item.Substring(53, 2)));
                        modelToAdd.ChargingStatus = HexToDecimal(ChangeHexOrder(item.Substring(55, 2)));
                        modelToAdd.ValveStatus = HexToDecimal(ChangeHexOrder(item.Substring(57, 2)));
                        modelToAdd.Distatus = HexToDecimal(ChangeHexOrder(item.Substring(59, 2)));
                        modelToAdd.AddedDatetime = DateTime.Now;
                        modelToAdd.NetworkNo = modelToAdd.NodeId >> 10;
                        modelToAdd.NodeNo = modelToAdd.NodeId & 1023;
                        modelToAdd.GatewayId = GatewayId;
                        MultiHandShakeReach.Add(modelToAdd);
                        _logger.LogInformation("Sensor event Frame Saved MultiHandShakeReach-" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)modelToAdd.NodeId, "Handshake Reach", Convert.ToDecimal(modelToAdd.TempReading), GatewayId, 0, 0, 0, 0, 0, 0, Convert.ToDecimal(modelToAdd.BatteryVoltReading)));
                    }
                    else if (frameType == 262)//GwstatusData
                    {
                        GwstatusData gwstatusData = new GwstatusData();
                        gwstatusData.FrameType = frameType;
                        gwstatusData.NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        gwstatusData.CtimeSoy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        gwstatusData.GwctimeSoy = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                        gwstatusData.ActiveSec = HexToDecimal(ChangeHexOrder(item.Substring(21, 8)));
                        //To ask regarding decimal points
                        gwstatusData.Gwvbat = Convert.ToDecimal(HextoFloat(ChangeHexOrder(item.Substring(29, 8))));// HexToDecimal(ChangeHexOrder(item.Substring(29, 8)));
                        gwstatusData.Gwtemp = Convert.ToDecimal(HextoFloat(ChangeHexOrder(item.Substring(37, 8))));//HexToDecimal(ChangeHexOrder(item.Substring(37, 8)));
                        gwstatusData.Spvvolt = Convert.ToDecimal(HextoFloat(ChangeHexOrder(item.Substring(45, 8))));//HexToDecimal(ChangeHexOrder(item.Substring(45, 8)));
                        gwstatusData.GwchargeS = HexToDecimal(ChangeHexOrder(item.Substring(53, 2)));
                        gwstatusData.GsmSig = HexToDecimal(ChangeHexOrder(item.Substring(55, 2)));
                        gwstatusData.ConnectedCards = HexToDecimal(ChangeHexOrder(item.Substring(57, 2)));
                        gwstatusData.Rfsig = HexToDecimal(ChangeHexOrder(item.Substring(59, 4)));
                        gwstatusData.Rfnwledstate = HexToDecimal(ChangeHexOrder(item.Substring(63, 2)));
                        gwstatusData.C1actmin = HexToDecimal(ChangeHexOrder(item.Substring(65, 8)));
                        gwstatusData.C2actmin = HexToDecimal(ChangeHexOrder(item.Substring(73, 8)));
                        gwstatusData.C3actmin = HexToDecimal(ChangeHexOrder(item.Substring(81, 8)));
                        gwstatusData.AddedDateTime = DateTime.Now;
                        gwstatusData.GatewayId = GatewayId;
                        GwstatusDataLst.Add(gwstatusData);

                        //await _mainDBContext.GwstatusData.AddAsync(gwstatusData);
                        //await _mainDBContext.SaveChangesAsync();
                        _logger.LogInformation("Sensor event Frame Saved GwstatusData-" + item);

                        //Add RTU Analysis Data
                        multiRtuAnalysis.Add(setRtuAnalysisModel((int)gwstatusData.NodeId, "Gateway status Data", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                    }
                    else if (frameType == 5)
                    {
                        //Alarm  Frame
                        int NodeId = HexToDecimal(ChangeHexOrder(item.Substring(9, 4)));
                        string CtimeSoy = ConvertToSoy(HexToDecimal(ChangeHexOrder(item.Substring(13, 8))));
                        int AlarmType = HexToDecimal(ChangeHexOrder(item.Substring(21, 2)));
                        if (AlarmType == 10) //MultiValveAlarmData
                        {
                            MultiValveAlarmData multiValveAlarmData = new MultiValveAlarmData();
                            multiValveAlarmData.FrameType = frameType;
                            multiValveAlarmData.NodeId = NodeId;
                            multiValveAlarmData.NodeId = NodeId;
                            multiValveAlarmData.Soy = CtimeSoy;
                            multiValveAlarmData.Gwsoy = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                            multiValveAlarmData.AlarmType = AlarmType;
                            multiValveAlarmData.ValveNo = HexToDecimal(ChangeHexOrder(item.Substring(23, 2)));
                            multiValveAlarmData.ValveType = HexToDecimal(ChangeHexOrder(item.Substring(25, 2)));
                            multiValveAlarmData.ReqState = HexToDecimal(ChangeHexOrder(item.Substring(27, 2)));
                            multiValveAlarmData.ReqStateReason = HexToDecimal(ChangeHexOrder(item.Substring(29, 2)));
                            multiValveAlarmData.CurrentState = HexToDecimal(ChangeHexOrder(item.Substring(31, 2)));
                            multiValveAlarmData.CstateReason = HexToDecimal(ChangeHexOrder(item.Substring(33, 2)));
                            multiValveAlarmData.AlarmReason = HexToDecimal(ChangeHexOrder(item.Substring(35, 2)));
                            multiValveAlarmData.UpdateTime = ConvertMOYToDateTime(HexToDecimal(ChangeHexOrder(item.Substring(37, 8))));
                            multiValveAlarmData.Gwmoy = HexToDecimal(ChangeHexOrder(item.Substring(37, 8)));
                            multiValveAlarmData.AcvcurrentConsimption = HexToDecimal(ChangeHexOrder(item.Substring(45, 8)));
                            multiValveAlarmData.AddedDateTime = DateTime.Now;
                            multiValveAlarmData.NetworkNo = multiValveAlarmData.NodeId >> 10;
                            multiValveAlarmData.NodeNo = multiValveAlarmData.NodeId & 1023;
                            multiValveAlarmData.GatewayId = GatewayId;
                            MultiValveAlarmDataLst.Add(multiValveAlarmData);

                            _logger.LogInformation("Sensor event Frame Saved multiValveAlarmData-" + item);

                            //Add RTU Analysis Data
                            multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiValveAlarmData.NodeId, "Valve Alarm", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                        }
                        else if (AlarmType == 11) //MultiSensorAlarmData
                        {
                            MultiSensorAlarmData multiSensorAlarmData = new MultiSensorAlarmData();
                            multiSensorAlarmData.FrameType = frameType;
                            multiSensorAlarmData.NodeId = NodeId;
                            multiSensorAlarmData.Soy = CtimeSoy;
                            multiSensorAlarmData.Gwsoy = HexToDecimal(ChangeHexOrder(item.Substring(13, 8)));
                            multiSensorAlarmData.AlarmType = AlarmType;
                            multiSensorAlarmData.SensorNo = HexToDecimal(ChangeHexOrder(item.Substring(23, 2)));
                            multiSensorAlarmData.SensorType = HexToDecimal(ChangeHexOrder(item.Substring(25, 2)));

                            string sensorData = item.Substring(27, 30);
                            int placeSensorNo = 0;
                            if (multiSensorAlarmData.SensorType == 2)
                            {
                                multiSensorAlarmData.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                                multiSensorAlarmData.SensorValue = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(2, 8))));
                            }
                            else if (multiSensorAlarmData.SensorType == 3)
                            {
                                multiSensorAlarmData.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                                multiSensorAlarmData.SensorValue = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(2, 8))));
                            }
                            else if (multiSensorAlarmData.SensorType == 4)
                            {   //Digital NO_NC Type Sensor Data		
                                multiSensorAlarmData.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                                multiSensorAlarmData.Cstate = HexToDecimal(sensorData.Substring(2, 2));
                                multiSensorAlarmData.DefaultState = HexToDecimal(sensorData.Substring(4, 2));
                            }
                            else if (multiSensorAlarmData.SensorType == 5)
                            {
                                multiSensorAlarmData.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                                multiSensorAlarmData.CummulativeCount = Convert.ToDecimal(HexToDecimal(ChangeHexOrder((sensorData.Substring(2, 8)))));
                                multiSensorAlarmData.Frequency = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(10, 8))));
                            }
                            else if (multiSensorAlarmData.SensorType == 37)
                            {
                                multiSensorAlarmData.AlarmReason = HexToDecimal(sensorData.Substring(placeSensorNo, 2));
                                multiSensorAlarmData.CummulativeCount = Convert.ToDecimal(HexToDecimal(ChangeHexOrder((sensorData.Substring(2, 8)))));
                                multiSensorAlarmData.Frequency = Convert.ToDecimal(HextoFloat(ChangeHexOrder(sensorData.Substring(10, 8))));

                            }
                            multiSensorAlarmData.AddedDateTime = DateTime.Now;
                            multiSensorAlarmData.NetworkNo = multiSensorAlarmData.NodeId >> 10;
                            multiSensorAlarmData.NodeNo = multiSensorAlarmData.NodeId & 1023;
                            multiSensorAlarmData.GatewayId = GatewayId;
                            MultiSensorAlarmDataLst.Add(multiSensorAlarmData);
                            _logger.LogInformation("Sensor event Frame Saved -multiSensorAlarmData" + item);
                            //Add RTU Analysis Data
                            multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiSensorAlarmData.NodeId, "Sensor Alarm", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));
                        }
                        else if (AlarmType >= 50 && AlarmType <= 56) //Node Alarm
                        {
                            MultiNodeAlarm multiSensorAlarmData = new MultiNodeAlarm();
                            multiSensorAlarmData.FrameType = frameType;
                            multiSensorAlarmData.NodeId = NodeId;
                            multiSensorAlarmData.Soy = CtimeSoy;
                            multiSensorAlarmData.AlarmType = AlarmType;
                            multiSensorAlarmData.AddedDateTime = DateTime.Now;
                            MultiNodeAlarmDataLst.Add(multiSensorAlarmData);
                            multiSensorAlarmData.NetworkNo = multiSensorAlarmData.NodeId >> 10;
                            multiSensorAlarmData.NodeNo = multiSensorAlarmData.NodeId & 1023;
                            multiSensorAlarmData.GatewayId = GatewayId;
                            _logger.LogInformation("MultiNodeAlarmDataLst Frame Saved -MultiNodeAlarmDataLst" + item);

                            //Add RTU Analysis Data
                            multiRtuAnalysis.Add(setRtuAnalysisModel((int)multiSensorAlarmData.NodeId, "Node Alarm", 0, GatewayId, 0, 0, 0, 0, 0, 0, 0));

                        }
                    }

                }

                if (MultiValveEventLst.Count > 0)
                {
                    await _mainDBContext.MultiValveEvent.AddRangeAsync(MultiValveEventLst);
                    await _mainDBContext.SaveChangesAsync();

                }

                if (MultiSensorEventLst.Count > 0)
                {
                    await _mainDBContext.MultiSensorEvent.AddRangeAsync(MultiSensorEventLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiNodeNwDataFrameLst.Count > 0)
                {
                    await _mainDBContext.MultiNodeNwDataFrame.AddRangeAsync(MultiNodeNwDataFrameLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiNodeJoinDataFrameLst.Count > 0)
                {
                    await _mainDBContext.MultiNodeJoinDataFrame.AddRangeAsync(MultiNodeJoinDataFrameLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (GwstatusDataLst.Count > 0)
                {
                    await _mainDBContext.GwstatusData.AddRangeAsync(GwstatusDataLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiValveAlarmDataLst.Count > 0)
                {
                    await _mainDBContext.MultiValveAlarmData.AddRangeAsync(MultiValveAlarmDataLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiSensorAlarmDataLst.Count > 0)
                {
                    await _mainDBContext.MultiSensorAlarmData.AddRangeAsync(MultiSensorAlarmDataLst);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (MultiNodeAlarmDataLst.Count > 0)
                {
                    await _mainDBContext.MultiNodeAlarm.AddRangeAsync(MultiNodeAlarmDataLst);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiHandShakeNonReach.Count > 0)
                {
                    await _mainDBContext.MultiHandShakeNonReach.AddRangeAsync(MultiHandShakeNonReach);
                    await _mainDBContext.SaveChangesAsync();
                }
                if (MultiHandShakeReach.Count > 0)
                {
                    await _mainDBContext.MultiHandShakeReach.AddRangeAsync(MultiHandShakeReach);
                    await _mainDBContext.SaveChangesAsync();
                }

                if (multiRtuAnalysis.Count > 0)
                {
                    await _mainDBContext.MultiRtuAnalysis.AddRangeAsync(multiRtuAnalysis);
                    await _mainDBContext.SaveChangesAsync();
                }


                return "success";
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        public MultiRtuAnalysis setRtuAnalysisModel(int nodeId, string eventType, decimal minTemp, int gatewayId, int? nodesf, int? nodesnr, int? noderssi, int? gwsf, int? gwsnr, int? gwrssi, decimal battery)
        {
            MultiRtuAnalysis analysismodel = new MultiRtuAnalysis();
            analysismodel.NetworkNo = GetNetworkNoFromNodeId(nodeId);
            analysismodel.NodeNo = GetNodeNoFromNodeId(nodeId);
            analysismodel.EventType = eventType;
            analysismodel.AddedDateTime = DateTime.Now;

            analysismodel.GatewayId = gatewayId;
            analysismodel.MinTemp = minTemp;
            analysismodel.NodeSf = nodesf;
            analysismodel.NodeSnr = nodesnr;
            analysismodel.NodeRssi = noderssi;
            analysismodel.GatewaySf = gwsf;
            analysismodel.GatewaySnr = gwsnr;
            analysismodel.GatewayRssi = gwrssi;
            analysismodel.Battery = battery;

            return analysismodel;
        }
        public int GetNodeNoFromNodeId(int nodeNo)
        {
            int nodenoForDB = nodeNo & 1023;
            return nodenoForDB;

        }

        public int GetNetworkNoFromNodeId(int nodeNo)
        {
            int shifts = nodeNo >> 10;
            return shifts;
        }
        /// <summary>
        /// ConvertToSoy
        /// </summary>
        /// <param name="soy"></param>
        /// <returns></returns>
        public string ConvertToSoy(int soy)
        {
            int sMoy = soy / 60;
            var dateTime = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateWithOffset = new DateTimeOffset(dateTime).ToUniversalTime();
            long timestamp = dateWithOffset.ToUnixTimeSeconds();

            int totalMoy = (int)(((timestamp / 60) + sMoy) * 60);

            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(totalMoy);

            DateTime dTime = dateTimeOffset.DateTime;

            return dTime.ToString();
        }
        /// <summary>
        /// UnixTimeStampToDateTime
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// ConvertMOYToDateTime
        /// </summary>
        /// <param name="MOY"></param>
        /// <returns></returns>
        public string ConvertMOYToDateTime(int MOY)
        {
            string datetime = "";
            //subtract the epoch start time from current time

            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)now.TotalSeconds;

            //TimeSpan current = new DateTime(DateTime.Now.Year, 1, 1, 12, 0, 0);
            //int secondsSincecurrentYear = (int)current.TotalSeconds;

            Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(DateTime.Now.Year, 1, 1)).TotalSeconds;

            int epocSec = (((secondsSinceEpoch - unixTimestamp) / 60) + MOY) * 60;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epocSec);

            DateTime dTime = dateTimeOffset.DateTime;

            return dTime.ToString();
        }
        /// <summary>
        /// HextoFloat
        /// </summary>
        /// <param name="HexRep"></param>
        /// <returns></returns>
        public float HextoFloat(string HexRep)
        {
            // Hexadecimal Representation of 0.0500
            // string HexRep = "3D4CCCCD";
            // Converting to integer
            Int32 IntRep = Int32.Parse(HexRep, NumberStyles.AllowHexSpecifier);
            // Integer to Byte[] and presenting it for float conversion
            float f = BitConverter.ToSingle(BitConverter.GetBytes(IntRep), 0);
            return f;
        }
    }
}
