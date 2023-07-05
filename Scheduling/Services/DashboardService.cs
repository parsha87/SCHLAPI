using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public interface IDashboardService
    {
        Task<ZBSList> GetZoneBlockSubblock(string priviledges, string userId);
        Task<DashboardZoneMatrixModel> GetZoneMatrix(string priviledges, string userId, int zoneId);
        Task<DashboardBlockModelList> GetBlockMatrix(string priviledges, string userId, int blockId);

    }
    public class DashboardService : IDashboardService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private EventDBContext _eventDBContext;
        private IZoneTimeService _zoneTimeService;

        private readonly ILogger<DashboardService> _logger;
        public DashboardService(ILogger<DashboardService> logger,
                MainDBContext mainDBContext, EventDBContext eventDBContext,
                IMapper mapper, IZoneTimeService zoneTimeService
            )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _eventDBContext = eventDBContext;
            _zoneTimeService = zoneTimeService;
            _logger = logger;
        }
        public async Task<ZBSList> GetZoneBlockSubblock(string priviledges, string userId)
        {
            try
            {
                ZBSList zBSLists = new ZBSList();
                List<KeyValueZB> keyValueZBs = new List<KeyValueZB>();

                List<KeyValueViewModel> zoneName = await _mainDBContext.Zone.Select(r => new KeyValueViewModel { Text = r.Name, Value = r.ZoneId, TagName = "" }).ToListAsync();
                if (priviledges != "All")
                {
                    List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();
                    List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
                    zoneName = zoneName.Where(s => zoneObj.Contains(s.Value)).ToList();
                }

                foreach (var item in zoneName)
                {
                    KeyValueZB keyValueZB = new KeyValueZB();
                    keyValueZB.ZoneName = item.Text;
                    keyValueZB.ZoneId = item.Value;
                    List<KeyValueBS> Blocks = await _mainDBContext.Block.Where(x => x.ZoneId == item.Value).Select(r => new KeyValueBS { Text = r.Name, Value = r.BlockId }).ToListAsync();

                    foreach (KeyValueBS itemBlock in Blocks)
                    {
                        List<KeyValueViewModel> BlocksSubblock = await _mainDBContext.SubBlock.Where(x => x.BlockId == itemBlock.Value).Select(r => new KeyValueViewModel { Text = r.Name, Value = r.SubblockId }).ToListAsync();
                        itemBlock.SubBlocks = BlocksSubblock;
                    }
                    keyValueZB.Blocks = Blocks;
                    keyValueZBs.Add(keyValueZB);
                }
                zBSLists.ZoneBlockSubblock = keyValueZBs;
                return zBSLists;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<DashboardZoneMatrixModel> GetZoneMatrix(string priviledges, string userId, int zoneId)
        {
            try
            {
                List<KeyValueBlockSensorViewModel> BlockNameList = new List<KeyValueBlockSensorViewModel>();
                BlockNameList = _mainDBContext.Block.Where(r => r.ZoneId == zoneId).OrderBy(z => z.BlockNo).Select(r => new KeyValueBlockSensorViewModel { Text = r.Name, Value = r.BlockId, TagName = r.TagName }).ToList();
                int blockCount = BlockNameList.Count;
                string CharId = "Z";
                DashboardZoneMatrixModel ds = await GetMatrixViewStatusZone(CharId, zoneId, userId);
                foreach (var item in BlockNameList)
                {

                    item.MatrixAssignedSensor = await GetMatrixAssignedSensor(userId, item.Text);

                }
                ds.blocks = BlockNameList;

                return ds;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<DashboardBlockModelList> GetBlockMatrix(string priviledges, string userId, int blockId)
        {
            try
            {

                List<SubBlock> SubBlockNameList = new List<SubBlock>();
                List<SubBlockViewModel> subBlockViewModels = new List<SubBlockViewModel>();
                DashboardBlockModelList dashboardBlockModelList = new DashboardBlockModelList();
                SubBlockNameList = await _mainDBContext.SubBlock.Where(r => r.BlockId == blockId).OrderBy(z => z.SubBlockNo).ToListAsync();
                subBlockViewModels = _mapper.Map<List<SubBlockViewModel>>(SubBlockNameList);
                int blockCount = SubBlockNameList.Count;
                if (blockCount == 0)
                {
                    return null;
                }
                else
                {
                    string CharId = "B";
                    //DashboardZoneMatrixModel ds = await GetMatrixViewStatusBlock(CharId, blockId, userId);
                    foreach (SubBlockViewModel item in subBlockViewModels)
                    {
                        DashboardBlockModel dashboardBlockModel = new DashboardBlockModel();

                        string ElementStatus = string.Empty;
                        string ElementReason = string.Empty;
                        string ValveReceivedTime = string.Empty;
                        bool tosetCroporNot = true;
                        bool faultyvalve = false;
                        bool isHavingStartTime = true;
                        string attributeicon = string.Empty;

                        dashboardBlockModel.BlockId = item.BlockId;
                        dashboardBlockModel.SubblockId = item.SubblockId;
                        dashboardBlockModel.ZoneId = item.SubblockId;
                        dashboardBlockModel.Name = item.Name;
                        dashboardBlockModel.ChannelId = item.ChannelId;
                        dashboardBlockModel.ChannelName = await _mainDBContext.Channel.Where(x => x.ChannelId == item.ChannelId).Select(x => x.ChannelName).FirstOrDefaultAsync();
                        dashboardBlockModel.TagName = item.TagName;

                        dashboardBlockModel.isHavingStartTime = false;
                        dashboardBlockModel.faultyvalve = false;

                        BlockMatrixSPData dsforSBicon = new BlockMatrixSPData();
                        dsforSBicon = await GetSubblockStatusIcon(Convert.ToInt32(item.ChannelId), await _zoneTimeService.TimeZoneDateTime());
                        //Get Start time
                        if (dsforSBicon.startEndTime != null)
                        {
                            string stTime = dsforSBicon.startEndTime.StartTime == null ? "00" : dsforSBicon.startEndTime.StartTime;
                            string etTime = dsforSBicon.startEndTime.EndTime == null ? "00" : dsforSBicon.startEndTime.EndTime;
                            dashboardBlockModel.StartEndTime = stTime + ":" + etTime;
                        }
                        else
                        {
                            dashboardBlockModel.StartEndTime = "-";

                        }

                        List<DashboardIcons> dtSubBlockStatusnxthrsch = dsforSBicon.nextSchedule; // Valve next one hour schedule
                        StartEndTIme dtSubBlockStatusnxthrAllTimesch = dsforSBicon.startEndTime;
                        if (dtSubBlockStatusnxthrsch != null)
                        {
                            if (dtSubBlockStatusnxthrsch.Count > 0)
                            {
                                var query = dtSubBlockStatusnxthrsch.AsEnumerable().Select(myRow => new { startTime = myRow.startTime, endTime = myRow.EndTime }).FirstOrDefault();

                                if (string.IsNullOrEmpty(query.startTime) == true)
                                {
                                    if (string.IsNullOrEmpty(dtSubBlockStatusnxthrAllTimesch.StartTime.ToString()) == true)
                                    {
                                        dashboardBlockModel.TimeTextColor = "white";
                                        dashboardBlockModel.TimeText = "(00:00-00:00)";
                                        dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                        dashboardBlockModel.isHavingStartTime = false;
                                    }
                                    else
                                    {
                                        DateTime dt = Convert.ToDateTime(dtSubBlockStatusnxthrAllTimesch.StartTime.ToString());
                                        DateTime tt = await _zoneTimeService.TimeZoneDateTime();
                                        if (dt < tt)
                                        {
                                            dashboardBlockModel.TimeTextColor = "white";
                                            dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrsch[0].startTime.ToString() + "-" + dtSubBlockStatusnxthrsch[0].EndTime.ToString() + ")";
                                            dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                        }
                                        else
                                        {
                                            if (dt < tt.AddHours(1))
                                            {
                                                dashboardBlockModel.TimeTextColor = "blue";
                                                dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrsch[0].startTime.ToString() + "-" + dtSubBlockStatusnxthrsch[0].EndTime.ToString() + ")";
                                                dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                            }
                                            else
                                            {
                                                dashboardBlockModel.TimeTextColor = "darkorchid";
                                                dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrsch[0].startTime.ToString() + "-" + dtSubBlockStatusnxthrsch[0].EndTime.ToString() + ")";
                                                dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DateTime dt = Convert.ToDateTime(query.startTime);
                                    DateTime tt = await _zoneTimeService.TimeZoneDateTime();

                                    if (dt < tt)
                                    {
                                        dashboardBlockModel.TimeTextColor = "black";
                                        dashboardBlockModel.TimeText = "(" + query.startTime + "-" + query.endTime + ")";
                                        dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                    }
                                    else
                                    {
                                        if (dt < tt.AddHours(1))
                                        {
                                            dashboardBlockModel.TimeTextColor = "blue";
                                            dashboardBlockModel.TimeText = "(" + query.startTime + "-" + query.endTime + ")";
                                            dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                        }
                                        else
                                        {
                                            dashboardBlockModel.TimeTextColor = "darkorchid";
                                            dashboardBlockModel.TimeText = "(" + query.startTime + "-" + query.endTime + ")";
                                            dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(dtSubBlockStatusnxthrAllTimesch.StartTime) == true)
                                {
                                    dashboardBlockModel.TimeTextColor = "white";
                                    dashboardBlockModel.TimeText = "(00:00-00:00)";
                                    dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                    dashboardBlockModel.isHavingStartTime = false;
                                }
                                else
                                {
                                    DateTime tt = await _zoneTimeService.TimeZoneDateTime();
                                    DateTime dt = Convert.ToDateTime(dtSubBlockStatusnxthrAllTimesch.StartTime.ToString());
                                    if (dt < tt)
                                    {
                                        dashboardBlockModel.TimeTextColor = "black";
                                        dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrAllTimesch.StartTime.ToString() + "-" + dtSubBlockStatusnxthrAllTimesch.EndTime.ToString() + ")";
                                        dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                        dashboardBlockModel.isHavingStartTime = true;
                                    }
                                    else
                                    {
                                        if (dt < tt.AddHours(1))
                                        {
                                            dashboardBlockModel.TimeTextColor = "blue";
                                            dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrAllTimesch.StartTime.ToString() + "-" + dtSubBlockStatusnxthrAllTimesch.EndTime.ToString() + ")";
                                            dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                            dashboardBlockModel.isHavingStartTime = true;
                                        }
                                        else
                                        {
                                            dashboardBlockModel.TimeTextColor = "darkorchid";
                                            dashboardBlockModel.TimeText = "(" + dtSubBlockStatusnxthrAllTimesch.StartTime.ToString() + "-" + dtSubBlockStatusnxthrAllTimesch.EndTime.ToString() + ")";
                                            dashboardBlockModel.Attribute = "SetImageforDashboardvalceschedule";
                                            dashboardBlockModel.isHavingStartTime = true;
                                        }
                                    }
                                }
                            }
                        }

                        List<BstEvents> dtSubBlockStatus = dsforSBicon.dashboardIcons;
                        #region Show Subblock valve colors
                        var valvetime = (dynamic)null;
                        DateTime dtstrt;
                        if (dtSubBlockStatusnxthrsch != null)
                        {
                            if (dtSubBlockStatusnxthrsch.Count > 0)
                            {
                                valvetime = dtSubBlockStatusnxthrsch.AsEnumerable().Select(myRow => new { startTime = myRow.startTime }).FirstOrDefault();
                                dtstrt = Convert.ToDateTime(valvetime.startTime);
                            }
                        }
                        if (dtSubBlockStatus.Count > 0)
                        {

                            ValveReceivedTime = dtSubBlockStatus[0].elestatus.ToString().Trim();
                            ElementStatus = dtSubBlockStatus[0].elestatus.ToString().Trim();
                            ElementReason = dtSubBlockStatus[0].reason.ToString().Trim();
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            //Mannual override and ON
                            if (ElementReason == "Manual Override" && ElementStatus == "ON")
                            {
                                dashboardBlockModel.BackgroundImage = "blueMOon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMOonTimer";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMOonNormal";
                                }

                            }
                            else if (ElementReason == "Manual Override" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {   // Mannual Override Pause/Stop
                                dashboardBlockModel.BackgroundImage = "yellowMOoff.png";
                            }
                            else if (ElementReason == "Manual Override" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {   // Mannual Override short/open
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            else if (ElementReason == "Manual Override" && ElementStatus == "Resume")
                            {   // Mannual Override Resume
                                dashboardBlockModel.BackgroundImage = "blueMOon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMOonTimer";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMOonNormal";
                                }

                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "schedule" && ElementStatus == "ON")
                            {  //Operated bySchedule ON // Irrigation Running
                                dashboardBlockModel.BackgroundImage = "bluevalve.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropon";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDrop";
                                }
                            }
                            else if (ElementReason == "schedule" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {
                                //Schedule Pause/Stop //Irrigation completed
                                dashboardBlockModel.BackgroundImage = "greenvalve.png";
                                dashboardBlockModel.ImageForDropKanis = "Jwariche-Kanis.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropwithSchedule";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardschtimer";
                                }
                            }
                            else if (ElementReason == "schedule" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {
                                //Schedule open/short
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            else if (ElementReason == "schedule" && ElementStatus == "Resume")
                            {
                                //Schedule Resume
                                dashboardBlockModel.BackgroundImage = "bluevalve.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                dashboardBlockModel.AttributeIcon = "SetImageforDashboardDrop";
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Rule" && ElementStatus == "ON")
                            {   //Operated by Rule ON
                                dashboardBlockModel.BackgroundImage = "blueruleon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";

                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleTimmer";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }

                            }
                            else if (ElementReason == "Rule" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {    //Rule Pause/Stop
                                dashboardBlockModel.BackgroundImage = "yelloruleoff.png";

                            }
                            else if (ElementReason == "Rule" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {    //Rule open short
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            else if (ElementReason == "Rule" && ElementStatus == "Resume")
                            {   //Rule Resume
                                dashboardBlockModel.BackgroundImage = " blueruleon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleTimmer";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }

                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Handeled" && ElementStatus == "ON")
                            {  //Operated by Rule ON
                                dashboardBlockModel.BackgroundImage = "bluehandheldon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMO";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }

                            }
                            else if (ElementReason == "Handeled" && ElementStatus == "Resume")
                            {  //Operated by Rule Resume
                                dashboardBlockModel.BackgroundImage = "bluehandheldon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";

                            }
                            else if (ElementReason == "Handeled" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {    //Handeled Pause/Stop
                                dashboardBlockModel.BackgroundImage = "yellohandleldoff.png";

                            }
                            else if (ElementReason == "Handeled" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Handeled Device" && ElementStatus == "ON")
                            {  //Operated by Rule ON
                                dashboardBlockModel.BackgroundImage = "bluehandheldon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";

                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMO";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }

                            }
                            else if (ElementReason == "Handeled Device" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {  //Operated by  ON
                                dashboardBlockModel.BackgroundImage = "yellohandleldoff.png";
                            }
                            else if (ElementReason == "Handeled Device" && ElementStatus == "Resume")
                            {  //Operated by  Resume
                                dashboardBlockModel.BackgroundImage = "bluehandheldon.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMO";
                                }
                                else
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }

                            }
                            else if (ElementReason == "Handeled Device" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Resume by Server" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Resume by Server
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            else if (ElementReason == "Resume by Server" && (ElementStatus == "Stop" || ElementStatus == "Pause" || ElementStatus == "OFF"))
                            {  //Operated by Resume by Server
                                dashboardBlockModel.BackgroundImage = "yellovalve.png";
                            }
                            else if (ElementReason == "Resume by Server" && ElementStatus == "ON")
                            {  //Operated by Resume by Server
                                dashboardBlockModel.BackgroundImage = "bluevalve.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropResumeServernormal";
                                }
                                else
                                {//
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropResumeServer";
                                }
                            }
                            else if (ElementReason == "PC" && ElementStatus == "ON")
                            {  //Operated by PC ON added on 01-06-2018
                                dashboardBlockModel.BackgroundImage = "bluePCon.png";

                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";
                                if (tosetCroporNot)
                                {
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropMO";
                                }
                                else
                                {//
                                    dashboardBlockModel.AttributeIcon = "SetImageforDashboardDropRuleHDon";
                                }
                            }
                            else if (ElementReason == "PC" && ElementStatus == "OFF")
                            {  //Operated by PC ON added on 01-06-2018

                                dashboardBlockModel.BackgroundImage = "yelloPCoff.png";

                            }
                            else if (ElementReason == "Resume by Server" && ElementStatus == "Resume")
                            {  //Operated by Resume
                                dashboardBlockModel.BackgroundImage = "bluevalve.png";
                                dashboardBlockModel.ImageForDropKanis = "Drop.gif";

                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "WDT Reset" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by WDT Reset
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Day Change" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Day Change
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            else if (ElementReason == "Schedule Notification" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Schedule Notification
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Resume by Hand Held" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Resume by Hand Held
                                dashboardBlockModel.BackgroundImage = "redvalve.png";

                                dashboardBlockModel.faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Test Mode" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Test Mode
                                dashboardBlockModel.BackgroundImage = "redvalve.png";
                                dashboardBlockModel.faultyvalve = true;
                            }
                            else if (ElementReason == "Test Mode")
                            {  //Operated by Resume by Start Up Sequence
                                if (valvetime != null)
                                {
                                    if (string.IsNullOrEmpty(valvetime.startTime) == true) //if valve is not in schedule
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                    }
                                    else
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";
                                    }
                                }
                                else
                                {
                                    if (dashboardBlockModel.isHavingStartTime)
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";

                                    }
                                    else
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";

                                    }
                                }
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Software Reset")
                            {  //Operated by Software Reset
                                dashboardBlockModel.BackgroundImage = "greyvalve.png";

                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Start Up Sequence" && (ElementStatus == "Open (for valves)" || ElementStatus == "Short (for valves)"))
                            {  //Operated by Resume by Start Up Sequence
                                dashboardBlockModel.BackgroundImage = "redvalve.png";

                                dashboardBlockModel.faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Start Up Sequence")
                            {  //Operated by Resume by Start Up Sequence

                                if (valvetime != null)
                                {
                                    if (string.IsNullOrEmpty(valvetime.startTime) == true) //if valve is not in schedule
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                    }
                                    else
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";

                                    }
                                }
                                else
                                {
                                    if (dashboardBlockModel.isHavingStartTime)
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";


                                    }
                                    else
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                    }
                                }
                                //faultyvalve = true;
                            }
                            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            else if (ElementReason == "Schedule Notification")
                            {  //Operated by Resume by Start Up Sequence

                                if (valvetime != null)
                                {
                                    if (string.IsNullOrEmpty(valvetime.startTime) == true) //if valve is not in schedule
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                    }
                                    else
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";
                                    }
                                }
                                else
                                {
                                    if (dashboardBlockModel.isHavingStartTime)
                                    {
                                        //if valve is in schedule and thre are no events
                                        dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";

                                    }
                                    else
                                    {
                                        dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                    }
                                }
                            }
                            else
                            {
                                dashboardBlockModel.BackgroundImage = "greyvalve.png";


                            }

                        }
                        else
                        {
                            if (valvetime != null)
                            {
                                if (string.IsNullOrEmpty(valvetime.startTime) == true) //if valve is not in schedule
                                {
                                    dashboardBlockModel.BackgroundImage = "greyvalve.png";

                                }
                                else
                                {
                                    //if valve is in schedule and thre are no events
                                    dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";
                                    
                                }
                            }
                            else
                            {
                                if (dashboardBlockModel.isHavingStartTime)
                                {
                                    //if valve is in schedule and thre are no events
                                    dashboardBlockModel.BackgroundImage = "Darkgreyvalve.png";
                                }
                                else
                                {
                                    dashboardBlockModel.BackgroundImage = "greyvalve.png";
                                }

                            }


                        }
                        #endregion


                       // dsforSBicon.subblocks = _mapper.Map<SubBlockViewModel>(item);

                        dashboardBlockModelList.dashboardBlockModelList.Add(dashboardBlockModel);
                    }
                    return dashboardBlockModelList;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #region Private Methods


        async Task<BlockMatrixSPData> GetSubblockStatusIcon(int ChannelId, DateTime TimeZoneDateTime)
        {
            try
            {
                BlockMatrixSPData lstdata = new BlockMatrixSPData();

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {

                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ChannelId", ChannelId);
                    parameters.Add("@TimeZoneDateTime", TimeZoneDateTime);
                    using (var gridReader = sqlConnection.QueryMultiple("GetSubblockStatusInfo", parameters, null, null, CommandType.StoredProcedure))
                    {
                        while (!gridReader.IsConsumed)
                        { //<-- query multiple until consumed
                            lstdata.startEndTime = gridReader.Read<StartEndTIme>().FirstOrDefault();
                            lstdata.progressbar = gridReader.Read<BstEvents>().ToList();
                            lstdata.nextSchedule = gridReader.Read<DashboardIcons>().ToList();
                            lstdata.dashboardIcons = gridReader.Read<BstEvents>().ToList();
                            lstdata.channelNames = gridReader.Read<string>().ToList();
                        }
                    }
                    //await sqlConnection.OpenAsync();
                    //var parameters = new DynamicParameters();
                    //parameters.Add("@ChannelId", ChannelId);
                    //parameters.Add("@TimeZoneDateTime", TimeZoneDateTime);
                    //var result = await sqlConnection.QueryMultipleAsync("GetSubblockStatusInfo", parameters, null, null, CommandType.StoredProcedure);
                    //var resultIntervals = await result.ReadAsync();

                    //lstdata.startEndTime = result.Read<dynamic>().ToList();
                    //lstdata.progressBar = result.Read<dynamic>().ToList();
                    //lstdata.nextSchedule = result.Read<dynamic>().ToList();
                    //lstdata.dashboardIcons = result.ReadSingle<dynamic>().Single();
                    //lstdata.channelNames = result.ReadSingle<dynamic>().Single();
                    await sqlConnection.CloseAsync();


                    return lstdata;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<DashboardZoneMatrixModel> GetMatrixViewStatusZone(string CharId, int zoneId, string userId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", zoneId);
                    parameters.Add("@userId", userId);
                    parameters.Add("@CharId", CharId);
                    var result = await sqlConnection.QueryMultipleAsync("GetMatrixViewStatusZone", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    DashboardZoneMatrixModel lstdata = new DashboardZoneMatrixModel();

                    lstdata.fertStatus = result.Read<dynamic>().AsList();
                    lstdata.filterStatus = result.Read<dynamic>().AsList();
                    lstdata.masterValveStatus = result.Read<dynamic>().AsList();
                    lstdata.masterPumpStatus = result.Read<dynamic>().AsList();
                    lstdata.ruleStaus = result.Read<dynamic>().AsList();
                    lstdata.moStatus = result.Read<dynamic>().AsList();
                    lstdata.sensorThresholdStatus = result.Read<dynamic>().AsList();
                    lstdata.sensorStatus = result.Read<dynamic>().AsList();
                    lstdata.blockLevelSensors = result.Read<dynamic>().AsList();
                    await sqlConnection.CloseAsync();

                    return lstdata;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DashboardZoneMatrixModel> GetMatrixViewStatusBlock(string CharId, int blockId, string userId)
        {
            try
            {

                using (SqlCommand cmd = new SqlCommand("GetMatrixViewStatusBlock",
            new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main"))))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CharId", SqlDbType.NVarChar).Value = CharId;
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = blockId;
                    cmd.Parameters.Add("@userId", SqlDbType.NVarChar).Value = userId;

                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            //string a = (string)reader["ATTRIBUTE_A"];
                            //string b = (string)reader["ATTRIBUTE_B"];
                        }
                    }
                }

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {

                    //var modelList = await _mainDBContext.Set<dynamic>().FromSqlRaw("GetMatrixViewStatusBlock @CharId = {0},c@Id = {1},v@userId = {2}", CharId, blockId, @userId).ToListAsync();


                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CharId", CharId);
                    parameters.Add("@Id", blockId);
                    parameters.Add("@userId", userId);
                    var result = await sqlConnection.QueryMultipleAsync("GetMatrixViewStatusBlock", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    DashboardZoneMatrixModel lstdata = new DashboardZoneMatrixModel();

                    lstdata.fertStatus = result.Read<dynamic>().AsList();
                    lstdata.filterStatus = result.Read<dynamic>().AsList();
                    lstdata.masterValveStatus = result.Read<dynamic>().AsList();
                    lstdata.masterPumpStatus = result.Read<dynamic>().AsList();
                    lstdata.ruleStaus = result.Read<dynamic>().AsList();
                    lstdata.moStatus = result.Read<dynamic>().AsList();
                    await sqlConnection.CloseAsync();

                    return lstdata;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<dynamic>> GetMatrixAssignedSensor(string userId, string blockName)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId);
                    parameters.Add("@ElementName", blockName);
                    var result = await sqlConnection.QueryMultipleAsync("GetMatrixAssignedSensors", parameters, null, null, CommandType.StoredProcedure);
                    List<dynamic> lstdata = result.Read<dynamic>().AsList();
                    return lstdata;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion
    }
}
