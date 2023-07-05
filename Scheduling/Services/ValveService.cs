using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.EventEntities;
using Scheduling.Data.TimestampEntities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using Scheduling.ViewModels.Lib;
using Scheduling.ViewModels.ResourceParamaters;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Scheduling.Services
{
    public interface IValveService
    {
        Task<PagedList<MultiSensorAlarmData>> GetSensorAlarm(ResourceParameter resourceParameter);
        Task<PagedList<MultiSensorAlarmData>> GetSensorAlarmByDate(PostEvents model);
        Task<IQueryable<MultiSensorAlarmData>> DownloadSensorAlarmByDate(PostEvents model);
        Task<PagedList<MultiNodeAlarm>> GetNodeAlarm(ResourceParameter resourceParameter);
        Task<PagedList<MultiNodeAlarm>> GetNodeAlarmByDate(PostEvents model);
        Task<IQueryable<MultiNodeAlarm>> DownloadNodeAlarmByDate(PostEvents model);
        Task<PagedList<MultiValveAlarmData>> GetValvesAlarmData(ResourceParameter resourceParameter);
        Task<PagedList<MultiValveAlarmData>> GetValvesAlarmDataByDate(PostEvents model);
        Task<IQueryable<MultiValveAlarmData>> DownloadValvesAlarmDataByDate(PostEvents model);
        Task<PagedList<MultiValveEvent>> GetAllValvesEventsMulti(ResourceParameter resourceParameter);
        Task<PagedList<MultiValveEvent>> GetAllValvesEventsMultiByDate(PostEvents model);
        Task<IQueryable<MultiValveEvent>> DownloadValvesEventsByDate(PostEvents model);
        Task<PagedList<MultiSensorEvent>> GetAllSensorEventsMulti(ResourceParameter resourceParameter);
        Task<PagedList<MultiSensorEvent>> GetAllSensorEventsMultiByDate(PostEvents model);
        Task<IQueryable<MultiSensorEvent>> DownloadensorEventsMultiByDate(PostEvents model);

        Task<int> GetValveCountBySeqId(int seqId);
        Task<List<string>> UpdateMasterValveStartTime(UpdateMasterValveStartTimeViewModel model);
        Task<List<string>> UpdateSingleValveDuration(UpdateSingleValveDurationModel model);

        Task<string> Add(List<SequenceValveConfigViewModel> models, SequenceViewModel sequence, TimeSpan valveStartTime, TimeSpan valveEndTime);
        // Task<bool> CheckIfChannelConfigured(int channelId, TimeSpan valveStartTime, TimeSpan valveEndTime, int seqId);
        Task<bool> DeleteSpecificValve(SequenceValveConfig CurrSequence, string valveStartTime, string valveEndTime, int PrgId, int networdkId);
        Task<List<SequenceMasterConfigViewModel>> GetSequenceMaster(int seqId, int startId, int mstSeqId);
        Task<int> GetFertGroupNameForWebMethod(int ChannelId, int NetworkId, int ZoneId);
        Task<int> GetFertGrNo(int PrjId, int ZoneID);
        Task<bool> AddEventForSequences(int SeqId, int ChannelId, int ScheduleNo, string status, int PrgId, DateTime TimeZoneDateTime, int projectId);
        Task<bool> BulkInsertValveTimeStamp(int SeqGrEleId, int SeqId, int StartId);
        Task<bool> DeleteAllSingleHoriElement(DeleteSeqElementViewModel model);
        Task<bool> DeleteSingleVerticleElement(DeleteVerticleElementViewModel model);
        Task<List<ChannelConfiguredModel>> CheckIfValveConfigured(int channelid, int stRow, int endRow, int newseqid);
        Task<List<SequenceValveConfig>> GetAllValves(int SeqId, int StartId);

        Task<List<MultiFrameTypes>> GetMultiFrameTypes();
        Task<List<MultiValveType>> GetMultiValveType();
        Task<List<MultiSensorAlarmReason>> GetMultiSensorAlarmReason();
        Task<List<MultiValveAlarmReason>> GetMultiValveAlarmReason();
        Task<List<MultiSensorType>> GetMultiSensorType();
        Task<List<MultiAlarmTypes>> GetMultiAlarmTypes();
        Task<List<MultiAddonCardTypes>> GetMultiAddonCardType();
        Task<List<MultiValveReason>> GetMultiValveReason();
        Task<List<MultiValveState>> GetMultiValveState();
    }

    public class ValveService : IValveService
    {
        private readonly IMapper _mapper;
        private readonly MainDBContext _mainDBContext;
        private readonly EventDBContext _eventDBContext;
        private readonly TimestampDBContext _timestampDBContext;
        private readonly ILogger<ValveService> _logger;
        private readonly IZoneTimeService _zoneTimeService;
        private readonly ISubBlockService _subBlockService;
        public ValveService(ILogger<ValveService> logger,
                MainDBContext uciDBContext,
                EventDBContext eventDBContext,
                TimestampDBContext timestampDBContext,
                IMapper mapper,
                IZoneTimeService zoneTimeService, ISubBlockService subBlockService
            )
        {
            _mapper = mapper;
            _eventDBContext = eventDBContext;
            _mainDBContext = uciDBContext;
            _timestampDBContext = timestampDBContext;
            _logger = logger;
            _zoneTimeService = zoneTimeService;
            _subBlockService = subBlockService;
        }

        /// <summary>
        /// Get sequence valve count
        /// </summary>
        /// <param name="seqId"></param>
        /// <returns></returns>
        public async Task<int> GetValveCountBySeqId(int seqId)
        {
            try
            {
                return await _mainDBContext.SequenceValveConfig.Where(x => x.SeqId == seqId).CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveService)}.{nameof(GetValveCountBySeqId)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// add valve
        /// </summary>
        /// <param name="models"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public async Task<string> Add(
            List<SequenceValveConfigViewModel> models,
            SequenceViewModel sequence, TimeSpan valveStartTime, TimeSpan valveEndTime)
        {
            string errorMessage = "";
            using (var context = new MainDBContext())
            {
                //using (var dbContextTransaction = context.Database.BeginTransaction())
                //{
                try
                {
                    string strValveStartTime = valveStartTime.Hours.ToString("00") + ":" + valveStartTime.Minutes.ToString("00");
                    string strValveEndTime = valveEndTime.Hours.ToString("00") + ":" + valveEndTime.Minutes.ToString("00");

                    for (int i = 0; i < models.Count; i++)
                    {
                        int channelId = 0;
                        bool IsHorizontal = (bool)models[i].IsHorizontal;
                        if (sequence.SeqType.Equals("NRV", StringComparison.OrdinalIgnoreCase))
                        {
                            channelId = (int)models[i].ChannelId;
                        }
                        else
                        {
                            var subBlock = await _subBlockService.GetSubBlockById((int)models[i].ChannelId);
                            channelId = (int)subBlock.ChannelId;
                        }
                        if (!await ValidateValveForMoreThanThirtyTime(channelId, (int)models[i].SeqId, sequence.PrgId))
                        {
                            errorMessage = GlobalConstants.dhs35 + " " + "var1".Replace("var1", models[i].Valve);
                            errorMessage = errorMessage + " " + GlobalConstants.dhs36 + " " + "var2".Replace("var2", GlobalConstants.MaxValvesAllowed.ToString());
                            // ModelState.AddModelError("Error", errorMessage);
                            break;
                        }

                        if (!await CheckIfChannelConfigured(channelId, strValveStartTime.Trim().PadLeft(5, '0'), strValveEndTime.Trim().PadLeft(5, '0'), (int)models[i].SeqId))
                        {
                            //TODO:PA send message
                            errorMessage = (int)models[i].ChannelId + "-" + GlobalConstants.Valve + " " + models[i].Valve + " " + GlobalConstants.dhs39 + " " + valveStartTime + " " + GlobalConstants.to + " " + valveEndTime.Hours.ToString("00") + ":" + valveEndTime.Minutes.ToString("00") + ". " + GlobalConstants.dhs40;
                            errorMessage += errorMessage;
                            //ModelState.AddModelError("Error", errorMessage);
                            continue;
                        }

                        string GroupName = models[i].GroupName;
                        // This condition is checked here to check if the valve exist in that horizontal grid.
                        //and if exist delte id timestamp and insert new changed valve.
                        if (models[i].SeqGrEleId == 0)
                        {
                            TimeSpan Duratin = new TimeSpan();
                            List<SequenceValveConfig> CurrSequence = _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == models[i].MstseqId
                                                                      && n.StartId == models[i].StartId && n.SeqId == models[i].SeqId && n.HorizGrId == models[i].HorizGrId
                                                                      && n.SeqGrEleId == Convert.ToInt32(models[i].SeqGrEleId)).ToList();
                            if (CurrSequence != null)
                            {
                                if (CurrSequence.Count > 0)
                                {
                                    SequenceValveConfig vcurr = CurrSequence[0];
                                    string[] Dur = vcurr.ValveStartDuration.Split(':');
                                    if (Dur != null)
                                    {
                                        if (Dur.Length == 2)
                                        {
                                            int Newstarthr = Convert.ToInt32(Dur[0]);
                                            int Newstartmin = Convert.ToInt32(Dur[1]);

                                            Duratin = new TimeSpan(Newstarthr, Newstartmin, 0);
                                        }
                                    }

                                    for (int k = 0; k < CurrSequence.Count; k++)
                                    {
                                        int cid = Convert.ToInt32(CurrSequence[i].ChannelId);
                                        int schid = Convert.ToInt32(CurrSequence[i].ScheduleNo);
                                        int sid = Convert.ToInt32(CurrSequence[i].SeqId);

                                        // Delete all elements from timespandetails
                                        //TimeInterval stTimeInt = jdc.TimeIntervals.FirstOrDefault(t => t.StartTime == CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'));
                                        string[] temp = CurrSequence[i].ValveStartTime.Split(':');
                                        TimeSpan v1StartTime = new TimeSpan();
                                        if (temp != null)
                                        {
                                            if (temp.Length == 2)
                                            {
                                                int hr = Convert.ToInt32(temp[0]);
                                                int min = Convert.ToInt32(temp[1]);
                                                v1StartTime = new TimeSpan(hr, min, 0);
                                            }
                                        }

                                        string[] temp1 = CurrSequence[i].ValveStartDuration.ToString().Split(':');
                                        TimeSpan v1Dur = new TimeSpan();
                                        if (temp1 != null)
                                        {
                                            if (temp1.Length == 2)
                                            {
                                                int hr1 = Convert.ToInt32(temp1[0]);
                                                int min1 = Convert.ToInt32(temp1[1]);

                                                v1Dur = new TimeSpan(hr1, min1, 0);
                                            }
                                        }

                                        TimeSpan v1EndTime = v1StartTime.Add(v1Dur);
                                        var result = await DeleteSpecificValve(CurrSequence[i], CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"), sequence.PrgId, sequence.NetworkId);
                                    }
                                }
                            }
                        }
                        //check if channel id already configured for this time stamp
                        SequenceValveConfig v = context.SequenceValveConfig.FirstOrDefault(n => n.SeqId == models[i].SeqId && n.StartId == models[i].StartId && n.MstseqId == models[i].MstseqId && n.GroupName == GroupName && n.ChannelId == models[i].ChannelId && n.Valve == models[i].Valve && n.HorizGrId == models[i].HorizGrId && n.IsHorizontal == models[i].IsHorizontal);
                        if (v == null)
                        {
                            v = new SequenceValveConfig();
                            v.MstseqId = models[i].MstseqId;
                            v.SeqId = models[i].SeqId;
                            v.StartId = models[i].StartId;
                            v.ChannelId = models[i].ChannelId;
                            v.ValveStartTime = models[i].ValveStartTime;
                            v.ValveStartDuration = models[i].ValveStartDuration;
                            DeletedSchedule sch = context.DeletedSchedule.OrderBy(n => n.SchNo).FirstOrDefault(n => n.ChannelId == v.ChannelId && n.ProgId == sequence.PrgId && n.Reused == false);
                            List<int> lstSeqIds = await context.Sequence.Where(x => x.PrgId == sequence.PrgId).Select(x => x.SeqId).ToListAsync();
                            var maxNo = await context.SequenceValveConfig.Where(x => x.ChannelId == v.ChannelId && lstSeqIds.Contains((int)x.SeqId)).Select(x => x.ScheduleNo).MaxAsync();
                            int highestId = 0;
                            if (maxNo == null)
                                maxNo = 0;
                            highestId = Convert.ToInt32(maxNo);
                            if (sch != null)
                            {
                                v.ScheduleNo = sch.SchNo;
                                sch.Reused = true;
                            }
                            else
                            {
                                v.ScheduleNo = highestId + 1;
                            }

                            v.GroupName = models[i].GroupName;
                            v.Valve = models[i].Valve;
                            v.ChannelId = models[i].ChannelId;
                            v.IsFlushRelated = models[i].IsFlushRelated;
                            v.IsFertilizerRelated = models[i].IsFertilizerRelated;
                            v.Typeofoperation = 0;

                            if (v.IsFertilizerRelated == true)
                            {
                                int MSTFertPumpId = 0;
                                int WaterBeforeFirt = 0;
                                int TypeOfOperation = 0;
                                int DurationOfFirt = 0;
                                string Unit = "";
                                MSTFertPumpId = await GetFertGroupNameForWebMethod(Convert.ToInt32(v.ChannelId), sequence.NetworkId, sequence.ZoneId);
                                /*
                                 * New code to insert fert settings in FertGrSettings table
                                 * Max 15 settings are allowed
                                 */

                                //Step 1. Check if no of settings < 15
                                List<FertValveGroupSettingsConfig> lstNoOfSettings = context.FertValveGroupSettingsConfig.Where(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId).ToList();
                                if (lstNoOfSettings.Count >= 15)
                                {
                                    v.IsFertilizerRelated = false;
                                    errorMessage = GlobalConstants.dhs37;
                                    errorMessage += errorMessage;

                                }
                                else
                                {
                                    //TODO::PA
                                    //WaterBeforeFirt = gdss.TimeOfIrr;
                                    //DurationOfFirt = gdss.DurOfFert;
                                    ////Here time of operation saved 0 as its datatype is int DB
                                    //TypeOfOperation = 0;
                                    //Unit = gdss.Unit;

                                    if (WaterBeforeFirt != 0 && DurationOfFirt != 0 && MSTFertPumpId != 0)
                                    {
                                        //Step 2
                                        //No of settings are not 15. Check if this setting already exist in setting table
                                        int settingid = 0;
                                        FertValveGroupSettingsConfig f = context.FertValveGroupSettingsConfig.FirstOrDefault(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId && n.Waterbeforefertilizer == WaterBeforeFirt && n.Typeofoperation == TypeOfOperation && n.Duration == DurationOfFirt);
                                        if (f == null)
                                        {
                                            //Step 3
                                            //This setting does not exist.
                                            //Insert in settings table
                                            f = new FertValveGroupSettingsConfig();

                                            if (lstNoOfSettings.Count > 0)
                                            {
                                                MSTFertPumpId = lstNoOfSettings[0].MstfertPumpId;
                                                settingid = lstNoOfSettings.Count + 1;
                                            }
                                            else
                                            {
                                                FertValveGroupConfig fg = context.FertValveGroupConfig.FirstOrDefault(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId);
                                                if (fg != null)
                                                    MSTFertPumpId = fg.MstfertPumpId;
                                                settingid = 1;
                                            }
                                            f.MstfertPumpId = MSTFertPumpId;
                                            f.ProjectId = sequence.PrjId;
                                            //  f.NetworkId = NetworkID;
                                            f.ZoneId = sequence.ZoneId;
                                            f.Waterbeforefertilizer = WaterBeforeFirt;
                                            f.Typeofoperation = TypeOfOperation;
                                            f.Duration = DurationOfFirt;
                                            f.NominalSuctionRate = 0;
                                            /// f.Unit = lblUnit.Text;
                                            f.FirtGrNo = MSTFertPumpId;
                                            f.FirtGrSettingNo = settingid;

                                            await context.FertValveGroupSettingsConfig.AddAsync(f);
                                            await context.SaveChangesAsync();

                                            settingid = f.FrtgrEleSettingsId;
                                        }
                                        else
                                        {
                                            //Step 4
                                            //This setting exist.
                                            //Get setting no
                                            settingid = f.FrtgrEleSettingsId;
                                        }

                                        int FertGrNo = await GetFertGrNo(sequence.PrjId, sequence.ZoneId);
                                        v.FertGrNo = FertGrNo;
                                        v.FertGrSettingNo = settingid;
                                        v.TimeOfIrrigation = WaterBeforeFirt;
                                        v.Typeofoperation = TypeOfOperation;
                                        v.DurationOfFert = DurationOfFirt;
                                        ///v.Unit = lblUnit.Text;
                                    }
                                    else
                                    {
                                        v.IsFertilizerRelated = false;
                                    }
                                }
                            }

                            v.IsHorizontal = IsHorizontal;
                            v.HorizGrId = models[i].HorizGrId; ;

                            await context.SequenceValveConfig.AddAsync(v);
                            await context.SaveChangesAsync();

                            await AddEventForSequence(Convert.ToInt32(v.SeqId), Convert.ToInt32(v.ChannelId), Convert.ToInt32(v.ScheduleNo), "A", sequence.PrgId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);

                            TimeSpan v1StartTime = new TimeSpan();
                            TimeSpan v1Dur = new TimeSpan();
                            TimeSpan v1EndTime = new TimeSpan();
                            string[] temp = v.ValveStartTime.Split(':');
                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }
                            string[] temp1 = v.ValveStartDuration.Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }

                            v1EndTime = v1StartTime.Add(v1Dur);

                            //HD
                            //13 Oct 2015
                            //Bulk insert
                            //TODO:PA
                            await BulkInsertValveTimeStamp(v.SeqGrEleId, Convert.ToInt32(v.SeqId), Convert.ToInt32(v.StartId));

                        }
                        else
                        {
                            v = new SequenceValveConfig();
                            v.MstseqId = models[i].MstseqId;
                            v.SeqId = models[i].SeqId;
                            v.StartId = models[i].StartId;
                            v.ChannelId = models[i].ChannelId;
                            v.ValveStartTime = models[i].ValveStartTime;
                            v.ValveStartDuration = models[i].ValveStartDuration;
                            v.IsFlushRelated = Convert.ToBoolean(models[i].IsFlushRelated);
                            v.IsFertilizerRelated = Convert.ToBoolean(models[i].IsFertilizerRelated);
                            v.Typeofoperation = 0;
                            if (v.IsFertilizerRelated == true)
                            {
                                int MSTFertPumpId = 0;
                                int WaterBeforeFirt = 0;
                                int TypeOfOperation = 0;
                                int DurationOfFirt = 0;

                                MSTFertPumpId = await GetFertGroupNameForWebMethod(Convert.ToInt32(v.ChannelId), sequence.NetworkId, sequence.ZoneId);

                                /*
                                 * New code to insert fert settings in FertGrSettings table
                                 * Max 15 settings are allowed
                                 */

                                //Step 1. Check if no of settings < 15
                                List<FertValveGroupSettingsConfig> lstNoOfSettings = context.FertValveGroupSettingsConfig.Where(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId).ToList();
                                if (lstNoOfSettings.Count >= 15)
                                {
                                    v.IsFertilizerRelated = false;
                                    errorMessage = GlobalConstants.dhs37;
                                    errorMessage += errorMessage;
                                }
                                else
                                {

                                    if (WaterBeforeFirt != 0 && DurationOfFirt != 0 && MSTFertPumpId != 0)
                                    {
                                        //Step 2
                                        //No of settings are not 15. Check if this setting already exist in setting table
                                        int settingid = 0;
                                        FertValveGroupSettingsConfig f = context.FertValveGroupSettingsConfig.FirstOrDefault(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId && n.Waterbeforefertilizer == WaterBeforeFirt && n.Typeofoperation == TypeOfOperation && n.Duration == DurationOfFirt);
                                        if (f == null)
                                        {
                                            //Step 3
                                            //This setting does not exist.
                                            //Insert in settings table
                                            f = new FertValveGroupSettingsConfig();

                                            if (lstNoOfSettings.Count > 0)
                                            {
                                                MSTFertPumpId = lstNoOfSettings[0].MstfertPumpId;
                                                settingid = lstNoOfSettings.Count + 1;
                                            }
                                            else
                                            {
                                                FertValveGroupConfig fg = context.FertValveGroupConfig.FirstOrDefault(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == sequence.ZoneId);
                                                if (fg != null)
                                                    MSTFertPumpId = fg.MstfertPumpId;
                                                settingid = 1;
                                            }
                                            f.MstfertPumpId = MSTFertPumpId;
                                            f.ProjectId = sequence.PrjId;
                                            //f.NetworkId = NetworkID;
                                            f.ZoneId = sequence.ZoneId;
                                            f.Waterbeforefertilizer = WaterBeforeFirt;
                                            f.Typeofoperation = TypeOfOperation;
                                            f.Duration = DurationOfFirt;
                                            f.NominalSuctionRate = 0;
                                            /// f.Unit = lblUnit.Text;
                                            f.FirtGrNo = MSTFertPumpId;
                                            f.FirtGrSettingNo = settingid;

                                            await context.FertValveGroupSettingsConfig.AddAsync(f);
                                            await context.SaveChangesAsync();

                                            settingid = f.FrtgrEleSettingsId;
                                        }
                                        else
                                        {
                                            //Step 4
                                            //This setting exist.
                                            //Get setting no
                                            settingid = f.FrtgrEleSettingsId;
                                        }
                                        int FertGrNo = await GetFertGrNo(sequence.PrjId, sequence.ZoneId);
                                        v.FertGrNo = FertGrNo;
                                        v.FertGrSettingNo = settingid;
                                        v.TimeOfIrrigation = WaterBeforeFirt;
                                        v.Typeofoperation = TypeOfOperation;
                                        v.DurationOfFert = DurationOfFirt;
                                        /// v.Unit = lblUnit.Text;
                                    }
                                    else
                                    {
                                        v.IsFertilizerRelated = false;
                                    }
                                }
                            }

                            v.IsHorizontal = IsHorizontal;

                            v.HorizGrId = models[i].HorizGrId;
                            await context.SaveChangesAsync();
                            await AddEventForSequence(Convert.ToInt32(v.SeqId), Convert.ToInt32(v.ChannelId), Convert.ToInt32(v.ScheduleNo), "U", sequence.PrgId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);

                            TimeSpan v1StartTime = new TimeSpan();
                            TimeSpan v1Dur = new TimeSpan();
                            TimeSpan v1EndTime = new TimeSpan();
                            string[] temp = v.ValveStartTime.Split(':');
                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }

                            string[] temp1 = v.ValveStartDuration.Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }

                            v1EndTime = v1StartTime.Add(v1Dur);


                            //HD
                            //13 Oct 2015
                            //Bulk insert
                            //TODO:PA
                            await BulkInsertValveTimeStamp(v.SeqGrEleId, Convert.ToInt32(v.SeqId), Convert.ToInt32(v.StartId));
                        }

                        // TODO: call sp GetStartEndTimeInterval
                        //start from line no. 6518 (CheckIfChannelConfigured())
                        //show error if valve is already configured.
                        //this is to check line number 5383
                    }

                    //foreach (var item in models)
                    //{
                    //    SequenceValveConfig valveToAdd = _mapper.Map<SequenceValveConfig>(item);
                    //    await _mainDBContext.SequenceValveConfig.AddAsync(valveToAdd);
                    //    await _mainDBContext.SaveChangesAsync();
                    //}

                    // Commit transaction if all commands succeed, transaction will auto-rollback
                    // when disposed if either commands fails
                    // await dbContextTransaction.CommitAsync();
                    //await dbContextTransaction.DisposeAsync();
                    return errorMessage;
                }
                catch (Exception ex)
                {
                    // await dbContextTransaction.RollbackAsync();
                    _logger.LogError($"[{nameof(ValveService)}.{nameof(Add)}]{ex}");
                    throw ex;
                }
                // }
            }
        }

        /// <summary>
        ///Check if Channel Configured
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="seqId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfChannelConfigured(int channelId, string valveStartTime, string valveEndTime, int seqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {//TODO:cal GetTimespanRowId here -PA 
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ValveStartTime", valveStartTime.ToString());
                    parameters.Add("@ValveEndTime", valveEndTime.ToString());
                    var result = await sqlConnection.QueryMultipleAsync("GetStartEndTimeInterval", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    List<StartEndTimeInterval> list = resultIntervals.Select(x => new StartEndTimeInterval { StartRow = x.StartRow, EndRow = x.EndRow }).ToList();


                    if (list != null)
                    {
                        if (list.Count > 0)
                        {
                            if (Convert.IsDBNull(list[0].StartRow) == false && Convert.IsDBNull(list[0].EndRow) == false)
                            {
                                var parametersq = new DynamicParameters();
                                parametersq.Add("@Startrow", list[0].StartRow);
                                parametersq.Add("@Endrow", list[0].EndRow);
                                parametersq.Add("@ChannelId", channelId);
                                parametersq.Add("@seqId", seqId);
                                var resultC = await sqlConnection.QueryMultipleAsync("GetValveTimespanCount", parametersq, null, null, CommandType.StoredProcedure);
                                var resultIntervalsC = await resultC.ReadAsync();
                                var lst = resultIntervalsC.ToList();
                                if (lst.Count > 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveService)}.{nameof(ValidateValveForMoreThanThirtyTime)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get sequence master configurations
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="startId"></param>
        /// <param name="mstSeqId"></param>
        /// <returns></returns>
        public async Task<List<SequenceMasterConfigViewModel>> GetSequenceMaster(int seqId, int mstSeqId, int startId)
        {
            try
            {
                List<SequenceMasterConfigViewModel> sequenceMasterConfigViewModels = new List<SequenceMasterConfigViewModel>();
                var list = await _mainDBContext.SequenceMasterConfig
                        .Where(x => x.SeqId == seqId && x.StartId == startId).ToListAsync();

                if (mstSeqId != 0)
                {
                    list = list
                        .Where(x => x.MstseqId == mstSeqId).ToList();
                }
                return list.Select(x => new SequenceMasterConfigViewModel
                {
                    MstseqId = x.MstseqId,
                    NetworkId = x.NetworkId,
                    PrgId = x.PrgId,
                    ProjectId = x.ProjectId,
                    SeqId = x.SeqId,
                    StartId = x.StartId,
                    StartTime = x.StartTime,
                    ZoneId = x.ZoneId
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveService)}.{nameof(GetSequenceMaster)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<StartEndTimeInterval>> GetTimespanRowId(string valveStartTime, string valveEndTime)
        {
            List<StartEndTimeInterval> list = null;
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ValveStartTime", valveStartTime.ToString());
                    parameters.Add("@ValveEndTime", valveEndTime.ToString());
                    var result = await sqlConnection.QueryMultipleAsync("GetStartEndTimeInterval", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals.Select(x => new StartEndTimeInterval { StartRow = x.StartRow, EndRow = x.EndRow }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public async Task<bool> DeleteSpecificValve(SequenceValveConfig CurrSequence, string valveStartTime, string valveEndTime, int PrgId, int networdkId)
        {
            try
            {

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    //TODO:cal GetTimespanRowId here -PA 
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ValveStartTime", valveStartTime.ToString());
                    parameters.Add("@ValveEndTime", valveEndTime.ToString());
                    var result = await sqlConnection.QueryMultipleAsync("GetStartEndTimeInterval", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    List<StartEndTimeInterval> dsTimeIntervals = resultIntervals.Select(x => new StartEndTimeInterval { StartRow = x.StartRow, EndRow = x.EndRow }).ToList();

                    // var dsTimeIntervals = jas.GetTimespanRowId(CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                    if (dsTimeIntervals != null)
                    {
                        if (dsTimeIntervals.Count > 0)
                        {
                            if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                            {
                                int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow);
                                int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow);


                                var parameters1 = new DynamicParameters();
                                parameters1.Add("@DeletedSeqId", Convert.ToInt32(CurrSequence.SeqId));
                                parameters1.Add("@channelid", Convert.ToInt32(CurrSequence.ChannelId));
                                parameters1.Add("@stRow", stRow);
                                parameters1.Add("@endRow", endRow);
                                var result1 = await sqlConnection.QueryMultipleAsync("DeleteSpecificValveTimeSpan", parameters1, null, null, CommandType.StoredProcedure);

                                DeleteValveFromLoop(Convert.ToInt32(CurrSequence.SeqId), Convert.ToInt32(CurrSequence.StartId), Convert.ToInt32(CurrSequence.ChannelId), stRow, endRow, PrgId);
                            }
                        }
                    }

                    _mainDBContext.SequenceValveConfig.RemoveRange(CurrSequence);
                    await _mainDBContext.SaveChangesAsync();

                    //Add deleted schedule in schedule table
                    //HD
                    //14 Mar 2015
                    DeletedSchedule deletedSchedule = new DeletedSchedule();
                    deletedSchedule.SchNo = CurrSequence.ScheduleNo;
                    deletedSchedule.Reused = false;
                    deletedSchedule.ChannelId = CurrSequence.ChannelId;
                    deletedSchedule.ProgId = PrgId;
                    await _mainDBContext.DeletedSchedule.AddAsync(deletedSchedule);
                    await _mainDBContext.SaveChangesAsync();
                    // jas.InsertDeletedSchedule(schid, cid, PrgId, false);

                    //Add delete event in events table
                    //HD
                    //6 Feb 2015
                    var ss = await AddEventForSequences((int)CurrSequence.SeqId, (int)CurrSequence.ChannelId, (int)CurrSequence.ScheduleNo, "D", PrgId, await _zoneTimeService.TimeZoneDateTime(), GlobalConstants.PrjId);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<int> GetFertGrNo(int PrjId, int ZoneID)
        {
            List<FertValveGroupConfig> lstFertGr = await _mainDBContext.FertValveGroupConfig.Where(n => n.ProjectId == PrjId && n.ZoneId == ZoneID).ToListAsync();
            return lstFertGr.Count + 1;
        }

        public async Task<int> GetFertGroupNameForWebMethod(int ChannelId, int NetworkId, int ZoneId)
        {
            int MSTFertPumpId = 0;
            try
            {

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ChannelId", ChannelId);
                    parameters.Add("@NetworkId", NetworkId);
                    parameters.Add("@ZoneId", ZoneId);
                    var result = await sqlConnection.QueryMultipleAsync("GetFertGroupName", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    FertGroupIdModel fertGroupIdModel = resultIntervals.Select(x => new FertGroupIdModel { MSTFertPumpId = x.MSTFertPumpId }).FirstOrDefault();

                    if (fertGroupIdModel != null)
                    {
                        if (fertGroupIdModel.MSTFertPumpId > 0)
                        {
                            return fertGroupIdModel.MSTFertPumpId;
                        }
                    }
                    return MSTFertPumpId;
                }
            }
            catch (Exception ex)
            {
                return MSTFertPumpId;
            }
        }

        public async Task<List<string>> UpdateSingleValveDuration(UpdateSingleValveDurationModel model)
        {
            List<string> resultsText = new List<string>();
            try
            {

                // using (var context = new MainDBContext())
                //{
                //using (var dbContextTransaction = context.Database.BeginTransaction())
                // {
                Sequence sequence = await _mainDBContext.Sequence.Where(x => x.SeqId == model.seqId).FirstOrDefaultAsync();
                int NetworkID = sequence.NetworkId;
                int ZoneID = sequence.ZoneId;
                int ProgramId = sequence.PrgId;
                int hstartId = 1;
                int MSTSeqId = 0;
                int StartId = hstartId;
                int editSeqId = model.seqId;

                var durNewStartTIme = model.valveStartDuration.Split(':');
                string DDLDurHr = durNewStartTIme[0];
                string DDLDurMin = durNewStartTIme[1];

                MSTSeqId = await _mainDBContext.SequenceMasterConfig.Where(s => s.ZoneId == ZoneID && s.NetworkId == NetworkID && s.SeqId == model.seqId && s.StartId == hstartId).OrderByDescending(s => s.StartId).Select(s => s.MstseqId).FirstOrDefaultAsync();

                DateTime SeqStDate = await _zoneTimeService.TimeZoneDateTime();
                DateTime SeqEndDate = await _zoneTimeService.TimeZoneDateTime();
                int SeqNo = 0;

                List<DatesToConfigure> dsSeqDatesToConfigure = await GetDatesToConfigure(model.seqId);

                TimeSpan OldDuration = new TimeSpan();
                TimeSpan NewDuration = new TimeSpan();
                TimeSpan MinsUpdated = new TimeSpan();

                string[] OldDur = null;
                string[] NewDur = null;

                if (DDLDurHr == "00" && DDLDurMin == "00")
                {
                    resultsText.Add(GlobalConstants.dhs49);
                    return resultsText;
                }

                NewDuration = new TimeSpan(Convert.ToInt32(DDLDurHr), Convert.ToInt32(DDLDurMin), 0);

                await DeleteAllTimeStamp(editSeqId, MSTSeqId, StartId, model.horizGrId);
                //removed valve and group name from filter
                //SequenceValveConfig v = await _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == MSTSeqId && n.StartId == StartId && n.SeqId == editSeqId && n.HorizGrId == model.horizGrId && n.IsHorizontal == true).OrderBy(x => x.SeqGrEleId).FirstOrDefaultAsync();
                //if (v != null)
                //{
                //    //Calculate no of mins added/substracted from current duration
                //    OldDur = v.ValveStartDuration.Split(':');
                //    if (OldDur != null)
                //    {
                //        if (OldDur.Length == 2)
                //        {
                //            int Newstarthr = Convert.ToInt32(OldDur[0]);
                //            int Newstartmin = Convert.ToInt32(OldDur[1]);
                //            OldDuration = new TimeSpan(Newstarthr, Newstartmin, 0);
                //            MinsUpdated = OldDuration.Subtract(NewDuration);
                //        }
                //    }

                //    if (DDLDurHr != null && DDLDurMin != null)
                //    {
                //        if (string.IsNullOrEmpty(DDLDurHr) == false && string.IsNullOrEmpty(DDLDurMin) == false)
                //        {
                //            v.ValveStartDuration = DDLDurHr + ":" + DDLDurMin;
                //        }
                //    }
                //    await _mainDBContext.SaveChangesAsync();
                //    await AddEventForSequence(Convert.ToInt32(v.SeqId), Convert.ToInt32(v.ChannelId), Convert.ToInt32(v.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                //}

                List<SequenceValveConfig> vhorz = await _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == MSTSeqId && n.StartId == StartId && n.SeqId == editSeqId && n.HorizGrId == model.horizGrId && n.IsHorizontal == true).OrderBy(x => x.SeqGrEleId).ToListAsync();
                if (vhorz != null)
                {
                    foreach (var vTemp in vhorz)
                    {
                        if (DDLDurHr != null && DDLDurMin != null)
                        {
                            if (string.IsNullOrEmpty(DDLDurHr) == false && string.IsNullOrEmpty(DDLDurMin) == false)
                            {
                                vTemp.ValveStartDuration = DDLDurHr + ":" + DDLDurMin;
                            }
                        }
                    }
                    _mainDBContext.UpdateRange(vhorz);
                    await _mainDBContext.SaveChangesAsync();
                    foreach (var vTemp in vhorz)
                    {
                        await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                    }
                }




                List<SequenceValveConfig> vNext = await _mainDBContext.SequenceValveConfig.OrderBy(x => x.SeqGrEleId).Where(n => n.StartId > StartId && n.SeqId == editSeqId && n.HorizGrId == model.horizGrId).ToListAsync();
                if (vNext != null)
                {
                    foreach (var vTemp in vNext)
                    {
                        //if (txtDuration.Text != null)
                        //    vTemp.ValveStartDuration = txtDuration.Text;

                        if (DDLDurHr != null && DDLDurMin != null)
                        {
                            if (string.IsNullOrEmpty(DDLDurHr) == false && string.IsNullOrEmpty(DDLDurMin) == false)
                            {
                                vTemp.ValveStartDuration = DDLDurHr + ":" + DDLDurMin;
                            }
                        }
                        await _mainDBContext.SaveChangesAsync();
                        await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                        //edit timestamp entry for this valve
                    }
                }

                List<SequenceValveConfig> vert = await _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == MSTSeqId && n.StartId == StartId && n.SeqId == editSeqId && n.HorizGrId == model.horizGrId && n.IsHorizontal == false).ToListAsync();
                if (vert != null)
                {
                    foreach (var vTemp in vert)
                    {
                        if (DDLDurHr != null && DDLDurMin != null)
                        {
                            if (string.IsNullOrEmpty(DDLDurHr) == false && string.IsNullOrEmpty(DDLDurMin) == false)
                            {
                                vTemp.ValveStartDuration = DDLDurHr + ":" + DDLDurMin;
                            }
                        }
                    }
                    _mainDBContext.UpdateRange(vert);
                    await _mainDBContext.SaveChangesAsync();
                    foreach (var vTemp in vert)
                    {
                        await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                    }
                }

                List<SequenceValveConfig> vertNext = await _mainDBContext.SequenceValveConfig.Where(n => n.StartId > StartId && n.SeqId == editSeqId && n.HorizGrId == model.horizGrId && n.IsHorizontal == false).ToListAsync();

                if (vertNext != null)
                {
                    foreach (var vTemp in vertNext)
                    {
                        if (DDLDurHr != null && DDLDurMin != null)
                        {
                            if (string.IsNullOrEmpty(DDLDurHr) == false && string.IsNullOrEmpty(DDLDurMin) == false)
                            {
                                vTemp.ValveStartDuration = DDLDurHr + ":" + DDLDurMin;
                            }
                        }
                        await _mainDBContext.SaveChangesAsync();
                        await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                    }
                }

                List<SequenceValveConfig> NextElements = await _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == MSTSeqId && n.StartId == StartId && n.SeqId == editSeqId && n.HorizGrId > model.horizGrId).OrderBy(n => n.HorizGrId).ToListAsync();
                if (NextElements != null)
                {
                    foreach (var vTemp in NextElements)
                    {
                        //Substract mins updated from all other elements
                        string[] StTime = vTemp.ValveStartTime.Split(':');
                        if (StTime != null)
                        {
                            if (StTime.Length == 2)
                            {
                                int hr = Convert.ToInt32(StTime[0]);
                                int min = Convert.ToInt32(StTime[1]);

                                TimeSpan CurrentStTime = new TimeSpan(hr, min, 0);

                                CurrentStTime = CurrentStTime.Subtract(MinsUpdated);

                                //22 Aug 2015
                                //to solve -ve timespan issue
                                if (CurrentStTime.Minutes < 0)
                                {
                                    DateTime dateTime = await _zoneTimeService.TimeZoneDateTime();
                                    DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hr, min, 0);
                                    date1 = date1.Subtract(MinsUpdated);
                                    CurrentStTime = new TimeSpan(date1.Hour, date1.Minute, 0);
                                }

                                vTemp.ValveStartTime = CurrentStTime.Hours.ToString("00") + ":" + CurrentStTime.Minutes.ToString("00");

                                await _mainDBContext.SaveChangesAsync();
                                await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                            }
                        }
                    }
                }

                List<SequenceValveConfig> NextSeqElements = await _mainDBContext.SequenceValveConfig.Where(n => n.StartId > StartId && n.SeqId == editSeqId && n.HorizGrId > model.horizGrId).OrderBy(n => n.HorizGrId).ToListAsync();
                if (NextSeqElements != null)
                {
                    foreach (var vTemp in NextSeqElements)
                    {

                        //Substract mins updated from all other elements
                        string[] StTime = vTemp.ValveStartTime.Split(':');
                        if (StTime != null)
                        {
                            if (StTime.Length == 2)
                            {
                                int hr = Convert.ToInt32(StTime[0]);
                                int min = Convert.ToInt32(StTime[1]);

                                TimeSpan CurrentStTime = new TimeSpan(hr, min, 0);

                                CurrentStTime = CurrentStTime.Subtract(MinsUpdated);

                                //22 Aug 2015
                                //to solve -ve timespan issue
                                if (CurrentStTime.Minutes < 0)
                                {
                                    DateTime dateTime = await _zoneTimeService.TimeZoneDateTime();
                                    DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hr, min, 0);
                                    date1 = date1.Subtract(MinsUpdated);
                                    CurrentStTime = new TimeSpan(date1.Hour, date1.Minute, 0);
                                }

                                vTemp.ValveStartTime = CurrentStTime.Hours.ToString("00") + ":" + CurrentStTime.Minutes.ToString("00");
                                await _mainDBContext.SaveChangesAsync();

                                await AddEventForSequence(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.ChannelId), Convert.ToInt32(vTemp.ScheduleNo), "U", ProgramId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                            }
                        }
                    }

                }

                Sequence snew = await _mainDBContext.Sequence.FirstOrDefaultAsync(s1 => s1.SeqId == editSeqId);

                bool isValidSequence = false;
                isValidSequence = await ValidateZoneDayEndTime(snew);
                if (isValidSequence == false)
                {
                    // PopulateSequenceError();
                    //if (errormsg != "")
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "MSG", "alert('" + errormsg.ToString() + "');ConfigureSequence('" + SeqId.ToString() + "','" + SeqNo.ToString() + "','" + PrjId.ToString() + "','" + NetworkID.ToString() + "','" + ZoneID.ToString() + "');", true);
                    //}
                }
                try
                {
                    await UpdateSeqValidationState(model.seqId);
                }
                catch (Exception ex)
                {
                    string ErrorMessage = "Error occured in UpdateDuration() method in DisplayHorizontalSequence.aspx";
                    // _log.Error(ErrorMessage, ex);
                }

                //await dbContextTransaction.CommitAsync();
                // }
                // }
                // ScriptManager.RegisterStartupScript(this, this.GetType(), "MSG", "ConfigureSequence('" + SeqId.ToString() + "','" + SeqNo.ToString() + "','" + PrjId.ToString() + "','" + NetworkID.ToString() + "','" + ZoneID.ToString() + "');", true);
                return resultsText;

            }
            catch (Exception ex)
            {
                return resultsText;
            }
        }

        public async Task<List<string>> UpdateMasterValveStartTime(UpdateMasterValveStartTimeViewModel model)
        {
            List<string> resultsText = new List<string>();
            try
            {

                DateTime start = await _zoneTimeService.TimeZoneDateTime(); //DateTime.Now;
                                                                            //Validate if all valves will be valid for new start time.
                int hstartId = 1;
                int MSTSeqId = 0;
                int StartId = hstartId;
                int editSeqId = model.seqId;

                string NewStartTime = model.seqMasterStartTime;
                var durNewStartTIme = NewStartTime.Split(':');
                string DDLHr = durNewStartTIme[0];
                string DDLMin = durNewStartTIme[1];
                Sequence sequence = await _mainDBContext.Sequence.Where(x => x.SeqId == model.seqId).FirstOrDefaultAsync();
                MSTSeqId = (int)_mainDBContext.SequenceMasterConfig.Where(s => s.ZoneId == sequence.ZoneId && s.NetworkId == sequence.NetworkId && s.SeqId == model.seqId && s.StartId == hstartId).OrderByDescending(s => s.StartId).Select(s => s.MstseqId).FirstOrDefault();
                //GetSequenceDetails();

                List<DatesToConfigure> dsSeqDatesToConfigure = await GetDatesToConfigure(editSeqId);
                if (dsSeqDatesToConfigure != null)
                {
                    List<SequenceMasterConfig> mList = _mainDBContext.SequenceMasterConfig.Where(n => n.SeqId != editSeqId).OrderBy(n => n.StartId).ToList();
                    bool ProceedUpdate = true;

                    //GetAllValvesInOtherStTimes(editSeqId, StartId)
                    List<SequenceValveConfig> dsAllValves = await _mainDBContext.SequenceValveConfig.Where(x => x.SeqId == model.seqId && x.StartId != StartId).ToListAsync();

                    string stTime = DDLHr + ":" + DDLMin;

                    if (dsAllValves != null)
                    {
                        if (dsAllValves.Count > 0)
                        {
                            TimeSpan DayStart = new TimeSpan();
                            TimeSpan DayEnd = new TimeSpan();

                            Zone dsZone = await _mainDBContext.Zone.Where(x => x.ZoneId == sequence.ZoneId).FirstOrDefaultAsync();// GetZones(NetworkID, "All");
                            if (dsZone != null)
                            {
                                string[] dayst = dsZone.DayStartTime.ToString().Trim().Split(':');
                                if (dayst.Length == 2)
                                {
                                    DayStart = new TimeSpan(Convert.ToInt32(dayst[0]), Convert.ToInt32(dayst[1]), 0);
                                    DayEnd = DayStart.Add(TimeSpan.FromHours(23));
                                    DayEnd = DayEnd.Add(TimeSpan.FromMinutes(59));
                                }
                            }

                            for (int i = 0; i < dsAllValves.Count; i++)
                            {
                                TimeSpan v1StartTime = new TimeSpan();
                                TimeSpan v1Dur = new TimeSpan();
                                TimeSpan v1EndTime = new TimeSpan();
                                string[] temp = stTime.Split(':');

                                if (Convert.ToInt32(dsAllValves[i].HorizGrId.ToString()) == 1)
                                {
                                    temp = stTime.Split(':');
                                }
                                else
                                {
                                    //Calculate new start time
                                    if (string.IsNullOrEmpty(dsAllValves[i].IsHorizontal.ToString()) == false)
                                    {
                                        if (Convert.ToBoolean(dsAllValves[i].IsHorizontal.ToString()) == true)
                                        {
                                            if (string.IsNullOrEmpty(dsAllValves[i].ValveStartDuration.ToString()) == false)
                                            {
                                                if (Convert.ToInt32(dsAllValves[i].HorizGrId.ToString()) > 1)
                                                {
                                                    if (Convert.ToInt32(dsAllValves[i].HorizGrId.ToString()) != Convert.ToInt32(dsAllValves[i - 1].HorizGrId.ToString()))
                                                    {
                                                        int valhr = 0, valmin = 0;
                                                        TimeSpan valStartTime = new TimeSpan();

                                                        string[] ValStartTime = dsAllValves[i - 1].ValveStartDuration.ToString().Split(':');
                                                        if (ValStartTime != null)
                                                        {
                                                            if (ValStartTime.Length == 2)
                                                            {
                                                                valhr = Convert.ToInt32(ValStartTime[0]);
                                                                valmin = Convert.ToInt32(ValStartTime[1]);

                                                                v1Dur = new TimeSpan(valhr, valmin, 0);

                                                                v1StartTime = new TimeSpan(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), 0);
                                                                v1StartTime = v1StartTime.Add(v1Dur);
                                                                stTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                                                temp = stTime.Split(':');
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                                if (temp != null)
                                {
                                    if (temp.Length == 2)
                                    {
                                        int hr = Convert.ToInt32(temp[0]);
                                        int min = Convert.ToInt32(temp[1]);

                                        v1StartTime = new TimeSpan(hr, min, 0);
                                    }
                                }

                                string[] temp1 = dsAllValves[i].ValveStartDuration.ToString().Split(':');
                                if (temp1 != null)
                                {
                                    if (temp1.Length == 2)
                                    {
                                        int hr1 = Convert.ToInt32(temp1[0]);
                                        int min1 = Convert.ToInt32(temp1[1]);

                                        v1Dur = new TimeSpan(hr1, min1, 0);
                                    }
                                }

                                v1EndTime = v1StartTime.Add(v1Dur);



                                if (ValidateDayStartTime(DayStart, DayEnd, v1StartTime, v1EndTime) == false)
                                {
                                    ProceedUpdate = false;
                                    resultsText.Add(GlobalConstants.dhs45);
                                    break;
                                }
                            }

                            if (ProceedUpdate)
                            {
                                stTime = DDLHr + ":" + DDLMin;

                                if (await CheckIfAllChannelsConfigured(dsAllValves, stTime, editSeqId) == false)
                                    ProceedUpdate = true;
                                else
                                {
                                    ProceedUpdate = false;
                                    resultsText.Add(GlobalConstants.dhs46);
                                }
                            }

                        }
                    }

                    if (ProceedUpdate)
                    {
                        //Delete all time stamp in this sequence
                        await DeleteAllTimeStamp(editSeqId, MSTSeqId, StartId, 1);
                        var ds = await GetSequenceMaster(editSeqId, MSTSeqId, StartId);
                        if (ds != null)
                        {
                            if (ds.Count > 0)
                            {
                                //jas.UpdateSequenceMaster(editSeqId, MSTSeqId, StartId, stTime);
                                SequenceMasterConfig sequenceMasterConfig = _mainDBContext.SequenceMasterConfig
                                   .Where(x => x.SeqId == editSeqId && x.MstseqId == MSTSeqId && x.StartId == StartId).FirstOrDefault();
                                sequenceMasterConfig.StartTime = stTime;
                                await _mainDBContext.SaveChangesAsync();

                                //jas.GetAllValves(editSeqId, StartId);
                                dsAllValves = _mainDBContext.SequenceValveConfig.Where(x => x.SeqId == editSeqId && x.StartId == StartId).ToList();

                                if (dsAllValves != null)
                                {
                                    if (dsAllValves.Count > 0)
                                    {
                                        await UpdateAllValves(dsAllValves, stTime, dsSeqDatesToConfigure, sequence);
                                    }
                                }
                            }
                        }
                    }

                    try
                    {
                        await UpdateSeqValidationState(editSeqId);
                    }
                    catch (Exception ex)
                    {
                        string ErrorMessage = string.Format("Error occured in updating sequence details in DisplayHorizontalSeqPg.aspx page load");
                        //_log.Error(ErrorMessage, ex);
                    }
                    //btnValidateSequence.Enabled = true;
                }

                return resultsText;
            }
            catch (Exception ex)
            {

                return resultsText;
            }
        }

        public async Task<bool> UpdateAllValves(List<SequenceValveConfig> tblValves, string NewStartTime, List<DatesToConfigure> SeqDates, Sequence sequence)
        {
            try
            {
                if (tblValves != null)
                {
                    if (tblValves.Count > 0)
                    {
                        int channelid = 0;

                        for (int i = 0; i < tblValves.Count; i++)
                        {
                            int SeqGrEleId = 0;
                            string ValveStartTime = "";

                            int seqid = 0;

                            int startid = 0;

                            if (Convert.IsDBNull(tblValves[i].ChannelId) == false)
                                channelid = Convert.ToInt32(tblValves[i].ChannelId.ToString());

                            if (Convert.IsDBNull(tblValves[i].SeqId) == false)
                                seqid = Convert.ToInt32(tblValves[i].SeqId.ToString());

                            if (Convert.IsDBNull(tblValves[i].StartId) == false)
                                startid = Convert.ToInt32(tblValves[i].StartId.ToString());


                            if (Convert.IsDBNull(tblValves[i].SeqGrEleId) == false)
                                SeqGrEleId = Convert.ToInt32(tblValves[i].SeqGrEleId.ToString());

                            ValveStartTime = tblValves[i].ValveStartTime.ToString();
                            TimeSpan v1StartTime = new TimeSpan();


                            TimeSpan v1Dur = new TimeSpan();
                            TimeSpan v1EndTime = new TimeSpan();

                            string[] temp = NewStartTime.Split(':');

                            if (Convert.ToInt32(tblValves[i].HorizGrId.ToString()) == 1)
                            {
                                temp = NewStartTime.Split(':');

                            }
                            else
                            {
                                //Calculate new start time
                                if (string.IsNullOrEmpty(tblValves[i].IsHorizontal.ToString()) == false)
                                {
                                    if (Convert.ToBoolean(tblValves[i].IsHorizontal.ToString()) == true)
                                    {
                                        if (Convert.ToInt32(tblValves[i].HorizGrId.ToString()) > 1)
                                        {
                                            if (Convert.ToInt32(tblValves[i].HorizGrId.ToString()) != Convert.ToInt32(tblValves[i - 1].HorizGrId.ToString()))
                                            {
                                                if (string.IsNullOrEmpty(tblValves[i].ValveStartDuration.ToString()) == false)
                                                {
                                                    int valhr = 0, valmin = 0;
                                                    TimeSpan valStartTime = new TimeSpan();

                                                    string[] ValStartTime = tblValves[i - 1].ValveStartDuration.ToString().Split(':');
                                                    if (ValStartTime != null)
                                                    {
                                                        if (ValStartTime.Length == 2)
                                                        {
                                                            valhr = Convert.ToInt32(ValStartTime[0]);
                                                            valmin = Convert.ToInt32(ValStartTime[1]);

                                                            v1Dur = new TimeSpan(valhr, valmin, 0);

                                                            v1StartTime = new TimeSpan(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), 0);
                                                            v1StartTime = v1StartTime.Add(v1Dur);
                                                            NewStartTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                                            temp = NewStartTime.Split(':');
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }

                            string[] temp1 = tblValves[i].ValveStartDuration.ToString().Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }

                            v1EndTime = v1StartTime.Add(v1Dur);

                            List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00"), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                            int loopingseqid = 0;

                            if (dsTimeIntervals != null)
                            {
                                if (dsTimeIntervals.Count > 0)
                                {
                                    if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                    {
                                        int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                        int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());
                                        List<ValveTimespanDetails> vtime = new List<ValveTimespanDetails>();
                                        for (int cnt = 0; cnt < SeqDates.Count; cnt++)
                                        {
                                            DateTime dt = Convert.ToDateTime(SeqDates[cnt].date.ToString());

                                            ValveTimespanDetails v = new ValveTimespanDetails();
                                            v.SeqId = sequence.SeqId;
                                            v.PrjId = sequence.PrjId;
                                            v.PrgId = sequence.PrgId;
                                            v.NetworkId = sequence.NetworkId;
                                            v.ZoneId = sequence.ZoneId;
                                            v.ChannelId = channelid;
                                            v.StartTimeSpanId = stRow;
                                            if (stRow == endRow)
                                            {
                                                v.EndTimeSpanId = endRow;
                                            }
                                            else
                                            {
                                                v.EndTimeSpanId = endRow - 1;
                                            }
                                            v.SeqDate = dt;

                                            if (GlobalConstants.ProgramLoopStatus == true)
                                                v.LoopingSeqId = loopingseqid;
                                            vtime.Add(v);
                                            //}
                                        }
                                        if (vtime != null)
                                        {
                                            await _timestampDBContext.ValveTimespanDetails.AddRangeAsync(vtime);
                                            await _timestampDBContext.SaveChangesAsync();
                                        }

                                    }
                                }

                            }

                            //Update valve
                            SequenceValveConfig vStep = await _mainDBContext.SequenceValveConfig.FirstOrDefaultAsync(n => n.SeqGrEleId == SeqGrEleId);
                            if (vStep != null)
                            {
                                vStep.ValveStartTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                await _mainDBContext.SaveChangesAsync();

                                await AddEventForSequence(Convert.ToInt32(vStep.SeqId), Convert.ToInt32(vStep.ChannelId), Convert.ToInt32(vStep.ScheduleNo), "U", sequence.PrgId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error occured in UpdateAllValves in DisplayHorizontalSequence.aspx";
                //_log.Error(ErrorMessage, ex);
                return false;
            }
        }


        public async Task<bool> DeleteSingleVerticleElement(DeleteVerticleElementViewModel model)
        {
            try
            {


                using (var context = new MainDBContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        Sequence sequence = await context.Sequence.Where(x => x.SeqId == model.seqId).FirstOrDefaultAsync();
                        SequenceMasterConfig sequenceMasterConfig = await context.SequenceMasterConfig.Where(x => x.SeqId == model.seqId).FirstOrDefaultAsync();
                        var startid = sequenceMasterConfig.StartId;
                        var MSTSeqId = sequenceMasterConfig.MstseqId;
                        int NetworkId = sequence.NetworkId;
                        int ZoneId = sequence.ZoneId;
                        int ProjectId = sequence.PrjId;
                        int ProgId = sequence.PrgId;
                        int editSeqId = model.seqId;

                        //We will need to adjust start time of next elements in sequence.
                        //This variable will contain duration of the element being deleted
                        TimeSpan Duration = new TimeSpan();
                        //ITs verticele Element
                        try
                        {
                            await UpdateSeqValidationState(model.seqId);
                        }
                        catch (Exception ex)
                        {
                            string ErrorMessage = string.Format("Error occured in updating sequence details in SequenceStepConfig.aspx page load");
                            //_log.Error(ErrorMessage, ex);
                        }
                        int seqgreleID = 0;
                        List<SequenceValveConfig> sequenceValveConfig = await context.SequenceValveConfig
                           .Where(x => x.HorizGrId == model.elementNo && x.IsHorizontal == false && x.GroupName == model.GroupName
                                   && x.Valve == model.Valve && x.StartId == startid).OrderBy(x => x.SeqGrEleId).ToListAsync();

                        List<string> seqGrId = sequenceValveConfig.Select(x => x.SeqGrEleId.ToString()).ToList();
                        List<int> lstSeqGrIds = new List<int>();

                        for (int iseqcount = 0; iseqcount < seqGrId.Count; iseqcount++)
                        {
                            if (seqGrId[iseqcount] != "0")
                                lstSeqGrIds.Add(Convert.ToInt32(seqGrId[iseqcount]));
                        }

                        for (int icount = 0; icount < sequenceValveConfig.Count; icount++)
                        {


                            SequenceValveConfig CurrSequence = null;
                            CurrSequence = sequenceValveConfig[icount];
                            if (CurrSequence != null)
                            {
                                string[] Dur = CurrSequence.ValveStartDuration.Split(':');
                                if (Dur != null)
                                {
                                    if (Dur.Length == 2)
                                    {
                                        int Newstarthr = Convert.ToInt32(Dur[0]);
                                        int Newstartmin = Convert.ToInt32(Dur[1]);

                                        Duration = new TimeSpan(Newstarthr, Newstartmin, 0);
                                    }
                                }

                                // Delete all elements from timespandetails
                                //TimeInterval stTimeInt = jdc.TimeIntervals.FirstOrDefault(t => t.StartTime == CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'));
                                string[] temp = CurrSequence.ValveStartTime.Split(':');
                                TimeSpan v1StartTime = new TimeSpan();
                                if (temp != null)
                                {
                                    if (temp.Length == 2)
                                    {
                                        int hr = Convert.ToInt32(temp[0]);
                                        int min = Convert.ToInt32(temp[1]);

                                        v1StartTime = new TimeSpan(hr, min, 0);
                                    }
                                }

                                string[] temp1 = CurrSequence.ValveStartDuration.ToString().Split(':');
                                TimeSpan v1Dur = new TimeSpan();
                                if (temp1 != null)
                                {
                                    if (temp1.Length == 2)
                                    {
                                        int hr1 = Convert.ToInt32(temp1[0]);
                                        int min1 = Convert.ToInt32(temp1[1]);

                                        v1Dur = new TimeSpan(hr1, min1, 0);
                                    }
                                }

                                TimeSpan v1EndTime = v1StartTime.Add(v1Dur);
                                //TimeInterval endTimeInt = jdc.TimeIntervals.FirstOrDefault(t => t.EndTime == v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));


                                List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(CurrSequence.ValveStartTime.Trim().PadLeft(5, '0'), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                                if (dsTimeIntervals != null)
                                {
                                    if (dsTimeIntervals.Count > 0)
                                    {
                                        if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                        {
                                            int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow);
                                            int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow);

                                            //HD
                                            //14 Oct 2015
                                            //Bulk delete
                                            await DeleteSpecifigValveTimeSpan(Convert.ToInt32(CurrSequence.SeqId), Convert.ToInt32(CurrSequence.ChannelId), stRow, endRow - 1);

                                        }
                                    }
                                }

                                int cid = Convert.ToInt32(CurrSequence.ChannelId);
                                int schid = Convert.ToInt32(CurrSequence.ScheduleNo);
                                int sid = Convert.ToInt32(CurrSequence.SeqId);

                                context.SequenceValveConfig.Remove(CurrSequence);
                                context.SaveChanges();

                                bool ErrorCode = await IsChannelUsed(Convert.ToInt32(CurrSequence.ChannelId));
                                if (ErrorCode == false)
                                {
                                    Channel channelObj = await context.Channel.FirstOrDefaultAsync(c => c.ChannelId == Convert.ToInt32(CurrSequence.ChannelId));
                                    if (channelObj != null)
                                    {
                                        channelObj.TypeId = 9;
                                        context.SaveChanges();
                                    }

                                }

                                //Add deleted schedule in schedule table
                                //HD
                                //14 Mar 2015
                                await InsertDeletedSchedule(schid, cid, ProgId, false);

                                await AddEventForSequence(sid, cid, schid, "D", ProgId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);

                                //Delete same valve from next replca sequences
                                List<SequenceValveConfig> NextSequence = context.SequenceValveConfig.Where(n => n.ChannelId == CurrSequence.ChannelId && n.StartId > CurrSequence.StartId && n.HorizGrId == CurrSequence.HorizGrId && n.IsHorizontal == false).ToList();
                                if (NextSequence != null)
                                {
                                    if (NextSequence.Count > 0)
                                    {
                                        for (int j = 0; j < NextSequence.Count; j++)
                                        {
                                            // Delete all elements from timespandetails
                                            //TimeInterval stTimeInt = jdc.TimeIntervals.FirstOrDefault(t => t.StartTime == CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'));
                                            string[] tempNext = NextSequence[j].ValveStartTime.Split(':');
                                            TimeSpan v1StartTime1 = new TimeSpan();
                                            if (tempNext != null)
                                            {
                                                if (tempNext.Length == 2)
                                                {
                                                    int hr = Convert.ToInt32(tempNext[0]);
                                                    int min = Convert.ToInt32(tempNext[1]);

                                                    v1StartTime1 = new TimeSpan(hr, min, 0);
                                                }
                                            }

                                            TimeSpan v1NextEndTime = v1StartTime1.Add(v1Dur);

                                            List<StartEndTimeInterval> dsNextTimeIntervals = await GetTimespanRowId(NextSequence[j].ValveStartTime.Trim().PadLeft(5, '0'), v1NextEndTime.Hours.ToString("00") + ":" + v1NextEndTime.Minutes.ToString("00"));

                                            if (dsNextTimeIntervals != null)
                                            {
                                                if (dsNextTimeIntervals.Count > 0)
                                                {
                                                    if (Convert.IsDBNull(dsNextTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsNextTimeIntervals[0].EndRow) == false)
                                                    {
                                                        int stRow = Convert.ToInt32(dsNextTimeIntervals[0].StartRow);
                                                        int endRow = Convert.ToInt32(dsNextTimeIntervals[0].StartRow);
                                                        //HD
                                                        //14 Oct 2015
                                                        //Bulk delete
                                                        await DeleteSpecifigValveTimeSpan(Convert.ToInt32(NextSequence[j].SeqId), Convert.ToInt32(NextSequence[j].ChannelId), stRow, endRow - 1);

                                                        //DeleteValveFromLoop(Convert.ToInt32(NextSequence[j].SeqId), Convert.ToInt32(NextSequence[j].StartId), Convert.ToInt32(NextSequence[j].ChannelId), stRow, endRow);
                                                    }
                                                }
                                            }

                                            cid = Convert.ToInt32(NextSequence[j].ChannelId);
                                            schid = Convert.ToInt32(NextSequence[j].ScheduleNo);
                                            sid = Convert.ToInt32(NextSequence[j].SeqId);

                                            context.SequenceValveConfig.Remove(NextSequence[j]);
                                            context.SaveChanges();

                                            ErrorCode = await IsChannelUsed(Convert.ToInt32(NextSequence[j].ChannelId));
                                            if (ErrorCode == false)
                                            {
                                                Channel channelObj = context.Channel.FirstOrDefault(c => c.ChannelId == Convert.ToInt32(NextSequence[j].ChannelId));
                                                if (channelObj != null)
                                                {
                                                    channelObj.TypeId = 9;
                                                    context.SaveChanges();
                                                }
                                            }

                                            //Add deleted schedule in schedule table
                                            //HD
                                            //14 Mar 2015
                                            await InsertDeletedSchedule(schid, cid, ProgId, false);

                                            await AddEventForSequence(sid, cid, schid, "D", ProgId, await _zoneTimeService.TimeZoneDateTime(), sequence.PrjId);
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        await dbContextTransaction.CommitAsync();

                        return true;

                    }
                }


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAllSingleHoriElement(DeleteSeqElementViewModel model)
        {
            try
            {
                int HorizGrId = model.horizGrId;
                SequenceMasterConfig seqMaster = _mainDBContext.SequenceMasterConfig.Where(s => s.SeqId == model.seqId).FirstOrDefault();
                var startid = seqMaster.StartId;
                var MSTSeqId = seqMaster.MstseqId;
                int SeqNo = 0;
                TimeSpan Duration = new TimeSpan();

                // DateTime SeqStDate = GetZoneTime.TimeZoneDateTime();// DateTime.Now;
                // DateTime SeqEndDate = GetZoneTime.TimeZoneDateTime(); //DateTime.Now;
                try
                {
                    await UpdateSeqValidationState(model.seqId);

                }
                catch (Exception ex)
                {
                    string ErrorMessage = string.Format("Error occured in updating sequence details in SequenceStepConfig.aspx page load");
                    //  _log.Error(ErrorMessage, ex);
                }

                List<SequenceValveConfig> CurrSequence = _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == MSTSeqId && n.StartId == startid && n.SeqId == model.seqId && n.HorizGrId == HorizGrId).ToList();
                if (CurrSequence != null)
                {
                    if (CurrSequence.Count > 0)
                    {
                        SequenceValveConfig v = CurrSequence[0];
                        string[] Dur = v.ValveStartDuration.Split(':');
                        if (Dur != null)
                        {
                            if (Dur.Length == 2)
                            {
                                int Newstarthr = Convert.ToInt32(Dur[0]);
                                int Newstartmin = Convert.ToInt32(Dur[1]);

                                Duration = new TimeSpan(Newstarthr, Newstartmin, 0);
                            }
                        }

                        for (int i = 0; i < CurrSequence.Count; i++)
                        {
                            int cid = Convert.ToInt32(CurrSequence[i].ChannelId);
                            int schid = Convert.ToInt32(CurrSequence[i].ScheduleNo);
                            int sid = Convert.ToInt32(CurrSequence[i].SeqId);

                            // Delete all elements from timespandetails
                            //TimeInterval stTimeInt = jdc.TimeIntervals.FirstOrDefault(t => t.StartTime == CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'));
                            string[] temp = CurrSequence[i].ValveStartTime.Split(':');
                            TimeSpan v1StartTime = new TimeSpan();
                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }

                            string[] temp1 = CurrSequence[i].ValveStartDuration.ToString().Split(':');
                            TimeSpan v1Dur = new TimeSpan();
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }
                            TimeSpan v1EndTime = v1StartTime.Add(v1Dur);
                            List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(CurrSequence[i].ValveStartTime.Trim().PadLeft(5, '0'), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                            if (dsTimeIntervals != null)
                            {
                                if (dsTimeIntervals.Count > 0)
                                {
                                    if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                    {
                                        int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                        int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());

                                        using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                                        {
                                            var parameters1 = new DynamicParameters();
                                            parameters1.Add("@DeletedSeqId", Convert.ToInt32(CurrSequence[i].SeqId));
                                            parameters1.Add("@channelid", Convert.ToInt32(CurrSequence[i].ChannelId));
                                            parameters1.Add("@stRow", stRow);
                                            parameters1.Add("@endRow", endRow - 1);
                                            var result1 = await sqlConnection.QueryMultipleAsync("DeleteSpecificValveTimeSpan", parameters1, null, null, CommandType.StoredProcedure);

                                            DeleteValveFromLoop(Convert.ToInt32(CurrSequence[i].SeqId), Convert.ToInt32(CurrSequence[i].StartId), Convert.ToInt32(CurrSequence[i].ChannelId), stRow, endRow, (int)seqMaster.PrgId);
                                        }
                                    }
                                }
                            }

                            _mainDBContext.SequenceValveConfig.RemoveRange(CurrSequence[i]);
                            await _mainDBContext.SaveChangesAsync();

                            //Add deleted schedule in schedule table
                            //HD
                            //14 Mar 2015
                            //DeletedSchedule deletedSchedule = new DeletedSchedule();
                            //deletedSchedule.SchNo = CurrSequence[i].ScheduleNo;
                            //deletedSchedule.Reused = false;
                            //deletedSchedule.ChannelId = CurrSequence[i].ChannelId;
                            //deletedSchedule.ProgId = (int)seqMaster.PrgId;
                            //await _mainDBContext.DeletedSchedule.AddAsync(deletedSchedule);
                            //await _mainDBContext.SaveChangesAsync();
                            // jas.InsertDeletedSchedule(schid, cid, PrgId, false);
                            await InsertDeletedSchedule((int)CurrSequence[i].ScheduleNo, (int)CurrSequence[i].ChannelId, (int)seqMaster.PrgId, false);
                            //Add delete event in events table
                            //HD
                            //6 Feb 2015
                            var ss = await AddEventForSequences((int)CurrSequence[i].SeqId, (int)CurrSequence[i].ChannelId, (int)CurrSequence[i].ScheduleNo, "D", (int)seqMaster.PrgId, await _zoneTimeService.TimeZoneDateTime(), GlobalConstants.PrjId);

                        }
                    }
                }
                //DeleteHorizontalFullElementSeq
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@DeletedSeqId", model.seqId);
                    parameters1.Add("@ElementNo", model.horizGrId);
                    var result1 = await sqlConnection.QueryMultipleAsync("DeleteHorizontalSeqFull", parameters1, null, null, CommandType.StoredProcedure);
                }

                //Now adjust start time of all next elements in original sequence
                List<SequenceValveConfig> NextElements = _mainDBContext.SequenceValveConfig.Where(n => n.MstseqId == Convert.ToInt32(MSTSeqId) && n.StartId == Convert.ToInt32(startid) && n.SeqId == model.seqId && n.HorizGrId > HorizGrId).OrderBy(n => n.HorizGrId).ToList();
                if (NextElements != null)
                {
                    foreach (var vTemp in NextElements)
                    {
                        //Substract mins updated from all other elements
                        string[] StTime = vTemp.ValveStartTime.Split(':');

                        TimeSpan v1StartTime = new TimeSpan();
                        if (StTime != null)
                        {
                            if (StTime.Length == 2)
                            {
                                int hr = Convert.ToInt32(StTime[0]);
                                int min = Convert.ToInt32(StTime[1]);

                                v1StartTime = new TimeSpan(hr, min, 0);
                            }
                        }

                        string[] temp1 = vTemp.ValveStartDuration.ToString().Split(':');
                        TimeSpan v1Dur = new TimeSpan();
                        if (temp1 != null)
                        {
                            if (temp1.Length == 2)
                            {
                                int hr1 = Convert.ToInt32(temp1[0]);
                                int min1 = Convert.ToInt32(temp1[1]);

                                v1Dur = new TimeSpan(hr1, min1, 0);
                            }
                        }

                        TimeSpan v1EndTime = v1StartTime.Add(v1Dur);

                        List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(vTemp.ValveStartTime.Trim().PadLeft(5, '0'), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                        if (dsTimeIntervals != null)
                        {
                            if (dsTimeIntervals.Count > 0)
                            {

                                int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());

                                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                                {
                                    var parameters1 = new DynamicParameters();
                                    parameters1.Add("@DeletedSeqId", Convert.ToInt32(vTemp.SeqId));
                                    parameters1.Add("@channelid", Convert.ToInt32(vTemp.ChannelId));
                                    parameters1.Add("@stRow", stRow);
                                    parameters1.Add("@endRow", endRow - 1);
                                    var result1 = await sqlConnection.QueryMultipleAsync("DeleteSpecificValveTimeSpan", parameters1, null, null, CommandType.StoredProcedure);

                                    DeleteValveFromLoop(Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.StartId), Convert.ToInt32(vTemp.ChannelId), stRow, endRow, (int)seqMaster.PrgId);
                                }
                            }
                        }
                        if (StTime != null)
                        {
                            if (StTime.Length == 2)
                            {
                                int hr = Convert.ToInt32(StTime[0]);
                                int min = Convert.ToInt32(StTime[1]);

                                TimeSpan CurrentStTime = new TimeSpan(hr, min, 0);
                                CurrentStTime = CurrentStTime.Subtract(Duration);
                                vTemp.ValveStartTime = CurrentStTime.Hours.ToString("00") + ":" + CurrentStTime.Minutes.ToString("00");
                                //Adjust horiz gr id also
                                //HD
                                //6 Feb 2015
                                vTemp.HorizGrId = vTemp.HorizGrId - 1;
                                TimeSpan v1NewEndTime = CurrentStTime.Add(v1Dur);
                                await _mainDBContext.SaveChangesAsync();

                                await BulkInsertValveTimeStamp(vTemp.SeqGrEleId, Convert.ToInt32(vTemp.SeqId), Convert.ToInt32(vTemp.StartId));

                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<bool> AddEventForSequence(int SeqId, int ChannelId, int ScheduleNo, string status, int PrgId, DateTime TimeZoneDateTime, int projectId)
        {
            try
            {
                EventViewModel evt = new EventViewModel();

                evt.ObjName = "Sequence";


                Channel ch = await _mainDBContext.Channel.FirstOrDefaultAsync(s => s.ChannelId == ChannelId);
                if (ch != null)
                {
                    Rtu rtu = await _mainDBContext.Rtu.FirstOrDefaultAsync(s => s.Rtuid == ch.Rtuid);
                    if (rtu != null)
                    {
                        evt.networkId = rtu.NetworkId;
                    }
                }


                evt.prjId = projectId;
                evt.objIdinDB = SeqId;
                evt.action = status;
                evt.TimeZoneDateTime = TimeZoneDateTime;
                string SchID = ChannelId.ToString() + "Z" + ScheduleNo.ToString() + "Z" + PrgId.ToString();   //Added ProgramID as per req if GP on 07-07-2016
                if (status == "D")
                {
                    //Check if there are any events mark as "M" for this valve schedule. If yes, delete them. Don't add new sequence
                    //If no, add new sequence with mark as "N"
                    List<Events> schedulelist = await _eventDBContext.Events.Where(x => x.ValveScheduleInDb == SchID && x.Status == "M" && x.ObjectIdInDb == SeqId && x.ObjTypeId == 12).ToListAsync();
                    if (schedulelist != null)
                    {
                        if (schedulelist.Count > 0)
                        {
                            _eventDBContext.Events.RemoveRange(schedulelist);
                            await _eventDBContext.SaveChangesAsync();
                        }
                        else
                        {
                            await AddEventForSequence(evt, SchID);
                        }
                    }
                    else
                    {
                        await AddEventForSequence(evt, SchID);
                    }
                }
                else
                {
                    await AddEventForSequence(evt, SchID);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        #region Private Methods


        private async Task<bool> ValidateZoneDayEndTime(Sequence s)
        {
            string errormsg = "";
            string errormsgForRes = "";
            try
            {
                List<DatesToConfigure> dsSeqDatesToConfigure = await GetDatesToConfigure(s.SeqId);
                TimeSpan DayStart = new TimeSpan();
                TimeSpan DayEnd = new TimeSpan();
                Zone dsZone = await _mainDBContext.Zone.Where(x => x.ZoneId == s.ZoneId).FirstOrDefaultAsync();// GetZones(NetworkID, "All");
                if (dsZone != null)
                {
                    string[] dayst = dsZone.DayStartTime.ToString().Trim().Split(':');
                    if (dayst.Length == 2)
                    {

                        DayStart = new TimeSpan(Convert.ToInt32(dayst[0]), Convert.ToInt32(dayst[1]), 0);
                        DayEnd = DayStart.Add(TimeSpan.FromHours(23));
                        DayEnd = DayEnd.Add(TimeSpan.FromMinutes(59));

                        //Get all elements(different start times) for this sequence
                        List<SequenceMasterConfig> mList = await _mainDBContext.SequenceMasterConfig.Where(n => n.SeqId == s.SeqId).OrderBy(n => n.StartId).ToListAsync();
                        TimeSpan SeqEndTime = new TimeSpan();

                        if (mList != null)
                        {
                            for (int cnt = 0; cnt < mList.Count; cnt++)
                            {
                                //Get element start time.
                                TimeSpan starttime = new TimeSpan();
                                TimeSpan duration = new TimeSpan();

                                string[] sttime = mList[cnt].StartTime.Split(':');


                                if (sttime.Length == 2)
                                {
                                    starttime = new TimeSpan(Convert.ToInt32(sttime[0]), Convert.ToInt32(sttime[1]), 0);

                                    SeqEndTime = starttime;
                                }
                                //Get all horizontal durations for each element
                                var valveDuration = await _mainDBContext.SequenceValveConfig.Where(n => n.SeqId == s.SeqId && n.StartId == mList[cnt].StartId && n.MstseqId == mList[cnt].MstseqId && n.IsHorizontal == true).Select(n => new { n.SeqId, n.StartId, n.ValveStartTime, n.ValveStartDuration }).Distinct().ToListAsync();

                                if (valveDuration != null)
                                {
                                    for (int i = 0; i < valveDuration.Count; i++)
                                    {
                                        string[] dur = valveDuration[i].ValveStartDuration.Split(':');
                                        if (dur.Length == 2)
                                        {
                                            duration = new TimeSpan(Convert.ToInt32(dur[0]), Convert.ToInt32(dur[1]), 0);
                                            SeqEndTime = SeqEndTime.Add(duration);
                                        }
                                    }

                                    if (ValidateDayStartTime(DayStart, DayEnd, starttime, SeqEndTime) == false)
                                    {
                                        errormsg = GlobalConstants.DayEndOverflowErrMsg.Replace("var1", s.SeqName);
                                        errormsgForRes = GlobalConstants.dhs64 + " " + "var1".Replace("var1", s.SeqName);
                                        return false;
                                    }

                                    List<SequenceValveConfig> dsAllValves = new List<SequenceValveConfig>();
                                    dsAllValves = await GetAllValves(s.SeqId, Convert.ToInt32(mList[cnt].StartId));

                                    List<SequenceValveConfig> dsAllValesInOtherStTimes = new List<SequenceValveConfig>();
                                    dsAllValesInOtherStTimes = await GetAllValvesInOtherStTimes(s.SeqId, Convert.ToInt32(mList[cnt].StartId));

                                    List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(starttime.Hours.ToString("00") + ":" + starttime.Minutes.ToString("00"), SeqEndTime.Hours.ToString("00") + ":" + SeqEndTime.Minutes.ToString("00"));
                                    int stRow = 0;
                                    int endRow = 0;

                                    if (dsTimeIntervals != null)
                                    {
                                        if (dsTimeIntervals.Count > 0)
                                        {
                                            if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                            {
                                                stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                                endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());
                                                //HD
                                                //6 May 2015
                                                //Call stored procedure to check if channel already configured
                                                if (stRow > endRow)
                                                    endRow = endRow + GlobalConstants.LastTimespanEntry;
                                            }
                                        }
                                    }

                                    if (dsAllValesInOtherStTimes != null)
                                    {
                                        if (dsAllValesInOtherStTimes.Count > 0)
                                        {
                                            if (await CheckIfChannelsConfiguredInOtherStTimes(dsAllValesInOtherStTimes, starttime.Hours.ToString("00") + ":" + starttime.Minutes.ToString("00"), stRow, endRow, s.SeqId) == true)
                                            {
                                                errormsg = GlobalConstants.ElementTimeErrMsg.Replace("var1", s.SeqName);
                                                errormsgForRes = "var1" + " " + GlobalConstants.dhs65.Replace("var1", s.SeqName);
                                                return false;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", mList[cnt].StartId.ToString()).Replace("var2", s.SeqName.ToString());
                                    errormsgForRes = GlobalConstants.dhs66 + " " + "var1".Replace("var1", mList[cnt].StartId.ToString());
                                    errormsgForRes = errormsgForRes + " " + GlobalConstants.forr + " " + "var2" + GlobalConstants.dhs67.Replace("var2", s.SeqName.ToString());
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            errormsg = GlobalConstants.NullElementErrMsg.Replace("var1", s.SeqName);
                            errormsgForRes = GlobalConstants.dhs68 + "var1" + GlobalConstants.dhs69.Replace("var1", s.SeqName);
                            return false;
                        }
                    }
                    else
                    {
                        errormsg = GlobalConstants.DayEndErrMsg;
                        errormsgForRes = GlobalConstants.dhs70;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errormsg = "Error while validating zone end time for sequence " + s.SeqId.ToString();
                errormsgForRes = GlobalConstants.dhs72 + " " + s.SeqId.ToString();
                //_log.Error(errormsg, ex);
                return false;
            }
            finally
            {

            }
            return true;
        }

        /// <summary>
        /// validate valve for more than thirty times
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="seqId"></param>
        /// <param name="programId"></param>
        /// <returns></returns>
        public async Task<bool> ValidateValveForMoreThanThirtyTime(int channelId, int seqId, int programId)
        {
            try
            {

                var valveCount = await _mainDBContext.SequenceValveConfig
                    .Join(_mainDBContext.SequenceMasterConfig.Where(x => x.PrgId == programId),
                    v => v.MstseqId,
                    m => m.MstseqId,
                    (v, m) => new { v, m })
                    .Where(x => x.v.ChannelId == channelId && x.v.SeqId == seqId)
                    .CountAsync();

                var maxStartId = await _mainDBContext.SequenceMasterConfig.Where(n => n.SeqId == seqId).MaxAsync(n => n.StartId);
                //If maxstartid is 0 that means this valve will be added once in the sequence
                if (maxStartId == 0)
                    maxStartId = 1;

                valveCount = Convert.ToInt32(valveCount) + Convert.ToInt32(maxStartId);
                if (valveCount > GlobalConstants.MaxValvesAllowed)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ValveService)}.{nameof(ValidateValveForMoreThanThirtyTime)}]{ex}");
                return false;
            }
        }

        public async Task<PagedList<MultiSensorEvent>> GetAllSensorEventsMulti(ResourceParameter resourceParameter)
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
                var lists = _mainDBContext.MultiSensorEvent.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiSensorEvent>.Create(lists, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PagedList<MultiSensorEvent>> GetAllSensorEventsMultiByDate(PostEvents model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (model.PageNumber == 0 & model.PageSize == 0)
            {
                throw new ArgumentNullException(nameof(model));
            }

            List<MultiSensorEvent> list = new List<MultiSensorEvent>();
            try
            {
                var lists = _mainDBContext.MultiSensorEvent.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiSensorEvent>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IQueryable<MultiSensorEvent>> DownloadensorEventsMultiByDate(PostEvents model)
        {
            List<MultiSensorEvent> list = new List<MultiSensorEvent>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiSensorEvent.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return lists;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PagedList<MultiNodeAlarm>> GetNodeAlarm(ResourceParameter resourceParameter)
        {
            List<MultiNodeAlarm> list = new List<MultiNodeAlarm>();
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
                var lists = _mainDBContext.MultiNodeAlarm.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiNodeAlarm>.Create(lists, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IQueryable<MultiNodeAlarm>> DownloadNodeAlarmByDate(PostEvents model)
        {
            List<MultiNodeAlarm> list = new List<MultiNodeAlarm>();
            try
            {
                var lists = _mainDBContext.MultiNodeAlarm.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return lists;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<MultiNodeAlarm>> GetNodeAlarmByDate(PostEvents model)
        {
            List<MultiNodeAlarm> list = new List<MultiNodeAlarm>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                if (model.PageNumber == 0 & model.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiNodeAlarm.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiNodeAlarm>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<PagedList<MultiSensorAlarmData>> GetSensorAlarm(ResourceParameter resourceParameter)
        {
            List<MultiSensorAlarmData> list = new List<MultiSensorAlarmData>();
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
                var lists = _mainDBContext.MultiSensorAlarmData.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiSensorAlarmData>.Create(lists, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IQueryable<MultiSensorAlarmData>> DownloadSensorAlarmByDate(PostEvents model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiSensorAlarmData.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return lists;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<MultiSensorAlarmData>> GetSensorAlarmByDate(PostEvents model)
        {
            List<MultiSensorAlarmData> list = new List<MultiSensorAlarmData>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                if (model.PageNumber == 0 & model.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiSensorAlarmData.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiSensorAlarmData>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<PagedList<MultiValveAlarmData>> GetValvesAlarmData(ResourceParameter resourceParameter)
        {
            List<MultiValveAlarmData> list = new List<MultiValveAlarmData>();
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
                var lists = _mainDBContext.MultiValveAlarmData.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiValveAlarmData>.Create(lists, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IQueryable<MultiValveAlarmData>> DownloadValvesAlarmDataByDate(PostEvents model)
        {
            List<MultiValveAlarmData> list = new List<MultiValveAlarmData>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiValveAlarmData.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return lists;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<MultiValveAlarmData>> GetValvesAlarmDataByDate(PostEvents model)
        {
            List<MultiValveAlarmData> list = new List<MultiValveAlarmData>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                if (model.PageNumber == 0 & model.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiValveAlarmData.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiValveAlarmData>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<IQueryable<MultiValveEvent>> DownloadValvesEventsByDate(PostEvents model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var list = _mainDBContext.MultiValveEvent.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return list;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<PagedList<MultiValveEvent>> GetAllValvesEventsMultiByDate(PostEvents model)
        {
            List<MultiValveEvent> list = new List<MultiValveEvent>();
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                if (model.PageNumber == 0 & model.PageSize == 0)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                var lists = _mainDBContext.MultiValveEvent.Where(x => x.AddedDateTime >= model.StartDateTime && x.AddedDateTime <= model.EndDateTime).OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiValveEvent>.Create(lists, model.PageNumber, model.PageSize, model.OrderBy, model.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<PagedList<MultiValveEvent>> GetAllValvesEventsMulti(ResourceParameter resourceParameter)
        {
            List<MultiValveEvent> list = new List<MultiValveEvent>();
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
                var lists = _mainDBContext.MultiValveEvent.OrderByDescending(x => x.AddedDateTime).AsQueryable();
                return PagedList<MultiValveEvent>.Create(lists, resourceParameter.PageNumber, resourceParameter.PageSize, resourceParameter.OrderBy, resourceParameter.OrderDir == "desc" ? true : false);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<SequenceValveConfig>> GetAllValves(int SeqId, int StartId)
        {
            List<SequenceValveConfig> list = new List<SequenceValveConfig>();

            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", SeqId);
                    parameters.Add("@StartId", StartId);
                    var result = await sqlConnection.QueryMultipleAsync("GetAllValvesUnderSeq", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals as List<SequenceValveConfig>;
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<SequenceValveConfig>> GetAllValvesInOtherStTimes(int SeqId, int StartId)
        {
            List<SequenceValveConfig> list = new List<SequenceValveConfig>();

            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", SeqId);
                    parameters.Add("@StartId", StartId);
                    var result = await sqlConnection.QueryMultipleAsync("GetAllValvesUnderOtherStartTimes", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals as List<SequenceValveConfig>;
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<List<MultiSensorType>> GetMultiSensorType()
        {
            List<MultiSensorType> list = new List<MultiSensorType>();
            try
            {
                var lists = _mainDBContext.MultiSensorType.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<MultiAlarmTypes>> GetMultiAlarmTypes()
        {
            List<MultiAlarmTypes> list = new List<MultiAlarmTypes>();
            try
            {
                var lists = _mainDBContext.MultiAlarmTypes.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<MultiAddonCardTypes>> GetMultiAddonCardType()
        {
            List<MultiAddonCardTypes> list = new List<MultiAddonCardTypes>();
            try
            {
                var lists = _mainDBContext.MultiAddonCardTypes.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //public async Task<List<MultiAlarmTypes>> MultiAlarmTypes()
        //{
        //    List<MultiAlarmTypes> list = new List<MultiAlarmTypes>();
        //    try
        //    {
        //        var lists = _mainDBContext.MultiAlarmTypes.AsEnumerable();
        //        list = await lists.ToListAsync();
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}


        public async Task<List<MultiSensorAlarmReason>> GetMultiSensorAlarmReason()
        {
            List<MultiSensorAlarmReason> list = new List<MultiSensorAlarmReason>();
            try
            {
                var lists = _mainDBContext.MultiSensorAlarmReason.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<MultiValveAlarmReason>> GetMultiValveAlarmReason()
        {
            List<MultiValveAlarmReason> list = new List<MultiValveAlarmReason>();
            try
            {
                var lists = _mainDBContext.MultiValveAlarmReason.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<MultiValveType>> GetMultiValveType()
        {
            List<MultiValveType> list = new List<MultiValveType>();
            try
            {
                var lists = _mainDBContext.MultiValveType.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<MultiFrameTypes>> GetMultiFrameTypes()
        {
            List<MultiFrameTypes> list = new List<MultiFrameTypes>();
            try
            {
                var lists = _mainDBContext.MultiFrameTypes.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<MultiValveReason>> GetMultiValveReason()
        {
            List<MultiValveReason> list = new List<MultiValveReason>();
            try
            {
                var lists = _mainDBContext.MultiValveReason.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<List<MultiValveState>> GetMultiValveState()
        {
            List<MultiValveState> list = new List<MultiValveState>();
            try
            {
                var lists = _mainDBContext.MultiValveState.AsEnumerable();
                list = lists.ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
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
        public async Task<bool> AddEventForSequences(int SeqId, int ChannelId, int ScheduleNo, string status, int PrgId, DateTime TimeZoneDateTime, int projectId)
        {
            try
            {
                EventViewModel evt = new EventViewModel();

                evt.ObjName = "Sequence";

                Channel ch = await _mainDBContext.Channel.FirstOrDefaultAsync(s => s.ChannelId == ChannelId);
                if (ch != null)
                {
                    Rtu rtu = await _mainDBContext.Rtu.FirstOrDefaultAsync(s => s.Rtuid == ch.Rtuid);
                    if (rtu != null)
                    {
                        evt.networkId = rtu.NetworkId;
                    }
                }


                evt.prjId = projectId;
                evt.objIdinDB = SeqId;
                evt.action = status;
                evt.TimeZoneDateTime = TimeZoneDateTime;
                string SchID = ChannelId.ToString() + "Z" + ScheduleNo.ToString() + "Z" + PrgId.ToString();   //Added ProgramID as per req if GP on 07-07-2016
                if (status == "D")
                {
                    //Check if there are any events mark as "M" for this valve schedule. If yes, delete them. Don't add new sequence
                    //If no, add new sequence with mark as "N"
                    List<Events> schedulelist = await _eventDBContext.Events.Where(x => x.ValveScheduleInDb == SchID && x.Status == "M" && x.ObjectIdInDb == SeqId && x.ObjTypeId == 12).ToListAsync();
                    if (schedulelist != null)
                    {
                        if (schedulelist.Count > 0)
                        {
                            _eventDBContext.Events.RemoveRange(schedulelist);
                            await _eventDBContext.SaveChangesAsync();
                        }
                        else
                        {
                            await AddEventForSequence(evt, SchID);
                        }
                    }
                    else
                    {
                        await AddEventForSequence(evt, SchID);
                    }
                }
                else
                {
                    await AddEventForSequence(evt, SchID);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public async Task<bool> AddEventForSequence(EventViewModel evt, string SchID)
        {
            try
            {
                var timezone = await _zoneTimeService.TimeZoneDateTime();
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ObjName", evt.ObjName);
                    parameters.Add("@action", evt.action);
                    parameters.Add("@prjId", evt.prjId);
                    parameters.Add("@networkId", evt.networkId);
                    parameters.Add("@objIdinDB", evt.objIdinDB);
                    parameters.Add("@ValveScheduleInDB", SchID);
                    parameters.Add("@TimeZoneDateTime", timezone);
                    var result = await sqlConnection.QueryMultipleAsync("AddEventsForSequence", parameters, null, null, CommandType.StoredProcedure);
                    //var resultIntervals = await result.ReadAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> BulkInsertValveTimeStamp(int SeqGrEleId, int SeqId, int StartId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "TimeStamp")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqGrEleId", SeqGrEleId);
                    parameters.Add("@SeqId", SeqId);
                    parameters.Add("@StartId", StartId);
                    var result = sqlConnection.QueryMultipleAsync("BulkInsertValveTimeSpan", parameters, null, null, CommandType.StoredProcedure);
                    //var resultIntervals = await result.ReadAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void SaveValveInLoop(int SeqId, int StartId, SequenceValveConfig v, string valveStartTime, string valveEndTime)
        {
            try
            {

                //DataSet dsTimeIntervals = jas.GetTimespanRowId(valveStartTime, valveEndTime);
                //ProgramLooping p = _mainDBContext.ProgramLooping.FirstOrDefault(p1 => p1.PrgId == ProgId);
                //if (p != null)
                //{
                //    List<SequenceLooping> sl = jdc.SequenceLoopings.Where(s => s.ActualSeqId == actualseqid && s.StartId == startid).ToList();
                //    if (sl != null)
                //    {
                //        for (int i = 1; i < sl.Count; i++)
                //        {
                //            DataSet loopDatesToConfigure = new DataSet();
                //            DateTime SeqStDate = new DateTime();
                //            DateTime SeqEndDate = new DateTime();

                //            if (dsTimeIntervals != null)
                //            {
                //                if (dsTimeIntervals.Tables[0].Rows.Count > 0)
                //                {
                //                    if (Convert.IsDBNull(dsTimeIntervals.Tables[0].Rows[0]["StartRow"]) == false && Convert.IsDBNull(dsTimeIntervals.Tables[0].Rows[0]["EndRow"]) == false)
                //                    {
                //                        int stRow = Convert.ToInt32(dsTimeIntervals.Tables[0].Rows[0]["StartRow"].ToString());
                //                        int endRow = Convert.ToInt32(dsTimeIntervals.Tables[0].Rows[0]["EndRow"].ToString());
                //                        if (stRow != endRow)
                //                        {
                //                            endRow = endRow - 1;
                //                        }

                //                        SequenceCommonFunctions scm = new SequenceCommonFunctions();

                //                        DataSet dsTimeSpanEntries = jas.CheckIfValveConfiguredInLoop(Convert.ToInt32(v.ChannelId), stRow, endRow, actualseqid, sl[i].SeqId);
                //                        if (dsTimeSpanEntries != null)
                //                        {
                //                            if (dsTimeSpanEntries.Tables.Count > 0)
                //                            {
                //                                if (dsTimeSpanEntries.Tables[0].Rows.Count == 0)
                //                                {
                //                                    scm.BulkInsertValveTimeStampInLoop(v.SeqGrEleId, Convert.ToInt32(v.SeqId), Convert.ToInt32(v.StartId), Convert.ToInt32(sl[i].SeqId));
                //                                }
                //                            }
                //                            else
                //                            {
                //                                if (dsTimeSpanEntries.Tables[0].Rows.Count == 0)
                //                                {
                //                                    scm.BulkInsertValveTimeStampInLoop(v.SeqGrEleId, Convert.ToInt32(v.SeqId), Convert.ToInt32(v.StartId), Convert.ToInt32(sl[i].SeqId));
                //                                }
                //                            }
                //                        }
                //                        else
                //                        {
                //                            if (dsTimeSpanEntries.Tables[0].Rows.Count == 0)
                //                            {
                //                                scm.BulkInsertValveTimeStampInLoop(v.SeqGrEleId, Convert.ToInt32(v.SeqId), Convert.ToInt32(v.StartId), Convert.ToInt32(sl[i].SeqId));
                //                            }
                //                        }          
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in saving valve loop in sequence in displayhorizontalseq.aspx page.");
                _logger.LogError(ErrorMessage, ex);

                throw;
            }
        }

        protected async Task<bool> CheckIfAllChannelsConfigured(List<SequenceValveConfig> tblValves, string NewStartTime, int newseqid)
        {
            try
            {

                if (tblValves != null)
                {
                    if (tblValves.Count > 0)
                    {
                        int channelid = 0;

                        for (int i = 0; i < tblValves.Count; i++)
                        {

                            channelid = Convert.ToInt32(tblValves[i].ChannelId.ToString());

                            TimeSpan v1StartTime = new TimeSpan();
                            TimeSpan v1Dur = new TimeSpan();
                            TimeSpan v1EndTime = new TimeSpan();
                            string[] temp = NewStartTime.Split(':');

                            if (Convert.ToInt32(tblValves[i].HorizGrId.ToString()) == 1)
                            {
                                temp = NewStartTime.Split(':');
                                //temp = tblValves.Rows[i]["ValveStartTime"].ToString().Split(':');
                            }
                            else
                            {
                                //Calculate new start time
                                if (string.IsNullOrEmpty(tblValves[i].IsHorizontal.ToString()) == false)
                                {
                                    if (Convert.ToBoolean(tblValves[i].IsHorizontal.ToString()) == true)
                                    {
                                        if (string.IsNullOrEmpty(tblValves[i].ValveStartDuration.ToString()) == false)
                                        {
                                            int valhr = 0, valmin = 0;
                                            TimeSpan valStartTime = new TimeSpan();

                                            string[] ValStartTime = tblValves[i - 1].ValveStartDuration.ToString().Split(':');
                                            if (ValStartTime != null)
                                            {
                                                if (ValStartTime.Length == 2)
                                                {
                                                    valhr = Convert.ToInt32(ValStartTime[0]);
                                                    valmin = Convert.ToInt32(ValStartTime[1]);

                                                    v1Dur = new TimeSpan(valhr, valmin, 0);

                                                    v1StartTime = new TimeSpan(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), 0);
                                                    v1StartTime = v1StartTime.Add(v1Dur);
                                                    NewStartTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                                    temp = NewStartTime.Split(':');
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }

                            string[] temp1 = tblValves[i].ValveStartDuration.ToString().Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }

                            v1EndTime = v1StartTime.Add(v1Dur);

                            List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00"), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                            if (dsTimeIntervals != null)
                            {
                                if (dsTimeIntervals.Count > 0)
                                {
                                    if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                    {
                                        int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                        int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());

                                        //Call stored procedure to check if channel already configured
                                        if (stRow > endRow)
                                            endRow = endRow + GlobalConstants.LastTimespanEntry;

                                        List<ChannelConfiguredModel> dsTimeSpanEntries = await CheckIfValveConfigured(channelid, stRow, endRow, newseqid);

                                        if (dsTimeSpanEntries != null)
                                        {
                                            if (dsTimeSpanEntries.Count > 0)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error occured in checkifallchannelsconfigured() method in DisplayHorizontalSequence.aspx";
                //_log.Error(ErrorMessage, ex);
            }
            return false;
        }

        protected async Task<bool> CheckIfChannelsConfiguredInOtherStTimes(List<SequenceValveConfig> tblValves, string NewStartTime, int start, int end, int newseqid)
        {
            try
            {
                if (tblValves != null)
                {
                    if (tblValves.Count > 0)
                    {
                        int channelid = 0;


                        for (int i = 0; i < tblValves.Count; i++)
                        {

                            if (Convert.IsDBNull(tblValves[i].ChannelId) == false)
                                channelid = Convert.ToInt32(tblValves[i].ChannelId.ToString());

                            TimeSpan v1StartTime = new TimeSpan();
                            TimeSpan v1Dur = new TimeSpan();
                            TimeSpan v1EndTime = new TimeSpan();
                            string[] temp = NewStartTime.Split(':');

                            if (Convert.ToInt32(tblValves[i].HorizGrId.ToString()) == 1)
                            {
                                //temp = NewStartTime.Split(':');
                                temp = tblValves[i].ValveStartTime.ToString().Split(':');
                                NewStartTime = tblValves[i].ValveStartTime.ToString();
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    temp = tblValves[i].ValveStartTime.ToString().Split(':');
                                    NewStartTime = tblValves[i].ValveStartTime.ToString();
                                }
                                else
                                {
                                    //Calculate new start time
                                    if (string.IsNullOrEmpty(tblValves[i].IsHorizontal.ToString()) == false)
                                    {
                                        if (Convert.ToBoolean(tblValves[i].IsHorizontal.ToString()) == true)
                                        {
                                            if (string.IsNullOrEmpty(tblValves[i].ValveStartDuration.ToString()) == false)
                                            {
                                                int valhr = 0, valmin = 0;
                                                TimeSpan valStartTime = new TimeSpan();

                                                string[] ValStartTime = tblValves[i - 1].ValveStartDuration.ToString().Split(':');
                                                if (ValStartTime != null)
                                                {
                                                    if (ValStartTime.Length == 2)
                                                    {
                                                        valhr = Convert.ToInt32(ValStartTime[0]);
                                                        valmin = Convert.ToInt32(ValStartTime[1]);

                                                        v1Dur = new TimeSpan(valhr, valmin, 0);

                                                        v1StartTime = new TimeSpan(Convert.ToInt32(temp[0]), Convert.ToInt32(temp[1]), 0);
                                                        v1StartTime = v1StartTime.Add(v1Dur);
                                                        NewStartTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                                        temp = NewStartTime.Split(':');
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            if (temp != null)
                            {
                                if (temp.Length == 2)
                                {
                                    int hr = Convert.ToInt32(temp[0]);
                                    int min = Convert.ToInt32(temp[1]);

                                    v1StartTime = new TimeSpan(hr, min, 0);
                                }
                            }

                            string[] temp1 = tblValves[i].ValveStartDuration.ToString().Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }

                            v1EndTime = v1StartTime.Add(v1Dur);

                            List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00"), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                            if (dsTimeIntervals != null)
                            {
                                if (dsTimeIntervals.Count > 0)
                                {
                                    if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                    {
                                        int stRow = Convert.ToInt32(dsTimeIntervals[0].StartRow.ToString());
                                        int endRow = Convert.ToInt32(dsTimeIntervals[0].EndRow.ToString());
                                        //Call stored procedure to check if channel already configured
                                        if (stRow > endRow)
                                            endRow = endRow + GlobalConstants.LastTimespanEntry;

                                        List<ChannelConfiguredModel> dsTimeSpanEntries = await CheckIfValveConfigured(channelid, stRow, endRow, newseqid);

                                        if (dsTimeSpanEntries != null)
                                        {
                                            if (dsTimeSpanEntries.Count > 0)
                                            {
                                                if (dsTimeSpanEntries.Count > 1)
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error occured in checkifchannelsconfiguredinothersttimes() method in DisplayHorizontalSequence.aspx";
                //_log.Error(ErrorMessage, ex);
            }
            return false;
        }

        public async Task<List<ChannelConfiguredModel>> CheckIfValveConfigured(int channelid, int stRow, int endRow, int newseqid)
        {
            List<ChannelConfiguredModel> list = new List<ChannelConfiguredModel>();

            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ChannelId", channelid);
                    parameters.Add("@NewStTime", stRow);
                    parameters.Add("@NewEndTime", endRow);
                    parameters.Add("@SeqId", newseqid);
                    var result = await sqlConnection.QueryMultipleAsync("GetSequenceDatesToConfigure", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals as List<ChannelConfiguredModel>;
                    return list;
                }
            }
            catch (Exception ex)
            {
                return list;
            }
        }
        public void DeleteValveFromLoop(int SeqId, int StartId, int ChannelId, int stRow, int endRow, int PrgId)
        {


        }
        protected async Task<bool> DeleteAllTimeStamp(int seqid, int mstseqid, int startid, int horizgrid)
        {
            //First we will delete all sequence timestamp for all horizontal durations of this sequence
            try
            {
                //GetSequenceHorizDur(seqid, mstseqid, startid, horizgrid);
                List<SequenceValveConfig> dsSeq = await _mainDBContext.SequenceValveConfig
                    .Where(x => x.SeqId == seqid &&
                                x.StartId == startid &&
                                x.MstseqId == mstseqid &&
                                x.HorizGrId == horizgrid &&
                                x.IsHorizontal == true).ToListAsync();

                if (dsSeq != null)
                {
                    TimeSpan v1StartTime = new TimeSpan();
                    TimeSpan v1Dur = new TimeSpan();
                    TimeSpan v1EndTime = new TimeSpan();
                    TimeSpan v1TotalDuration = new TimeSpan();
                    if (dsSeq.Count > 0)
                    {
                        string[] temp = dsSeq[0].ValveStartTime.ToString().Trim().Split(':');
                        if (temp != null)
                        {
                            if (temp.Length == 2)
                            {
                                int hr = Convert.ToInt32(temp[0]);
                                int min = Convert.ToInt32(temp[1]);

                                v1StartTime = new TimeSpan(hr, min, 0);
                            }
                        }

                        for (int i = 0; i < dsSeq.Count; i++)
                        {
                            string[] temp1 = dsSeq[i].ValveStartDuration.ToString().Trim().Split(':');
                            if (temp1 != null)
                            {
                                if (temp1.Length == 2)
                                {
                                    int hr1 = Convert.ToInt32(temp1[0]);
                                    int min1 = Convert.ToInt32(temp1[1]);

                                    v1Dur = new TimeSpan(hr1, min1, 0);
                                }
                            }
                            v1TotalDuration = v1TotalDuration.Add(v1Dur);
                        }

                        v1EndTime = v1StartTime.Add(v1TotalDuration);
                        //Delete all timestamp between these start and end time
                        List<StartEndTimeInterval> dsTimeIntervals = await GetTimespanRowId(v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00"), v1EndTime.Hours.ToString("00") + ":" + v1EndTime.Minutes.ToString("00"));

                        if (dsTimeIntervals != null)
                        {
                            if (dsTimeIntervals.Count > 0)
                            {
                                if (Convert.IsDBNull(dsTimeIntervals[0].StartRow) == false && Convert.IsDBNull(dsTimeIntervals[0].EndRow) == false)
                                {
                                    int strow = Convert.ToInt32(dsTimeIntervals[0].StartRow);
                                    int endrow = Convert.ToInt32(dsTimeIntervals[0].EndRow);

                                    //Bulk Delete
                                    await DeleteSpecifigValveTimeSpan(seqid, -1, Convert.ToInt32(dsTimeIntervals[0].StartRow), Convert.ToInt32(dsTimeIntervals[0].EndRow));
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in deleting sequence timespan in deletealltimestamp function");
                //_log.Error(ErrorMessage, ex);
                return false;
            }
        }
        public async Task<bool> UpdateSeqValidationState(int SeqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@SeqID", SeqId);
                    var result1 = await sqlConnection.QueryMultipleAsync("UpdateSeqValidationState", parameters1, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> IsChannelUsed(int channelid)
        {
            bool errorCode = false;
            string type = "C";
            try
            {
                //using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                //{
                //    var parameters1 = new DynamicParameters();
                //    parameters1.Add("@Id", channelid);
                //    parameters1.Add("@type", type);
                //    await sqlConnection.OpenAsync();
                //    var result1 = await sqlConnection.QueryMultipleAsync("GetChannelInfo", parameters1, null, 120, CommandType.StoredProcedure);
                //    var resultUsed = await result1.ReadAsync();
                //    if(resultUsed != null)
                //    {
                //        if(resultUsed.ToList().Count > 0)
                //        {
                //            return errorCode;
                //        }
                //        else
                //        {
                //            errorCode = false;
                //            return errorCode;
                //        }
                //    }

                //    return errorCode;
                //}
                // DataSet ds = jas.GetChannelInfo(channelid, "C");
                //Sub block
                var subBlock = _mainDBContext.SubBlock.Where(x => x.ChannelId == channelid).ToList();
                if (subBlock.Count > 0)
                {
                    errorCode = true;
                }

                // Valve Group
                var valveGroup = await _mainDBContext.ValveGroupConfig.Join(
                             _mainDBContext.ValveGroupElementConfig,
                             c => c.ValveConfigId,
                             r => r.ValveConfigId,
                             (c, r) => new { c, r })
                              .Where(m => m.r.ChannelId == channelid)
                             .ToListAsync();

                if (valveGroup.Count > 0)
                {
                    errorCode = true;
                }

                //--=== 2.Filter group ====
                var filterGroup = await _mainDBContext.FilterValveGroupConfig.Join(
                             _mainDBContext.FilterValveGroupElementsConfig,
                             c => c.MstfilterGroupId,
                             r => r.MstfilterGroupId,
                             (c, r) => new { c, r })
                              .Where(m => m.r.ChannelId == channelid)
                             .ToListAsync();
                if (filterGroup.Count > 0)
                {
                    errorCode = true;
                }
                //	--===3. Fertilizer Pump group====	
                var fertilizerGroup = await _mainDBContext.FertValveGroupConfig.Join(
                             _mainDBContext.FertValveGroupElementsConfig,
                             c => c.MstfertPumpId,
                             r => r.MstfertPumpId,
                             (c, r) => new { c, r })
                              .Where(m => m.r.ChannelId == channelid && m.c.FertCounterNo == channelid)
                             .ToListAsync();

                if (fertilizerGroup.Count > 0)
                {
                    errorCode = true;
                }
                //	--===4. Master valve group====		
                var mastervalveGroup = await _mainDBContext.MasterValveGroupConfig.Join(
                             _mainDBContext.MasterValveGroupElementConfig,
                             c => c.MstValveConfigId,
                             r => r.MstValveConfigId,
                             (c, r) => new { c, r })
                              .Where(m => m.r.ChannelId == channelid)
                             .ToListAsync();

                if (mastervalveGroup.Count > 0)
                {
                    errorCode = true;
                }
                //	--===5. Pump station group====		
                var pumpvalveGroup = await _mainDBContext.MasterPumpStationConfig.Join(
                     _mainDBContext.MasterPumpStationSteps,
                     c => c.MstpumpStationId,
                     r => r.MstpumpStationId,
                     (c, r) => new { c, r })
                      .Where(m => m.r.PumpId == channelid)
                     .ToListAsync();

                if (pumpvalveGroup.Count > 0)
                {
                    errorCode = true;
                }
                ////----=========Get Sequence name=============
                var seqValve = await (from s in _mainDBContext.Sequence
                                      join se in _mainDBContext.SequenceValveConfig on s.SeqId equals se.SeqId                                     
                                      where se.ChannelId == channelid
                                      select new
                                      {
                                          name = 0
                                      }).ToListAsync();

                if (seqValve.Count > 0)
                {
                    errorCode = true;
                }

                var filter1 = await _mainDBContext.FilterValveGroupConfig.Join(
                     _mainDBContext.FilterValveGroupElementsConfig,
                     c => c.MstfilterGroupId,
                     r => r.MstfilterGroupId,
                     (c, r) => new { c, r })
                      .Where(m => m.c.WaterMeterNumber == channelid)
                     .ToListAsync();
                if (filter1.Count > 0)
                {
                    errorCode = true;
                }
                var result1 = await _mainDBContext.FilterValveGroupConfig.Join(
                    _mainDBContext.FilterValveGroupElementsConfig,
                    c => c.MstfilterGroupId,
                    r => r.MstfilterGroupId,
                    (c, r) => new { c, r })
                     .Where(m => m.c.PdsensorNo == channelid)
                    .ToListAsync();
                if (result1.Count > 0)
                {
                    errorCode = true;
                }
                var result2 = await _mainDBContext.FilterValveGroupConfig.Join(
                  _mainDBContext.FilterValveGroupElementsConfig,
                  c => c.MstfilterGroupId,
                  r => r.MstfilterGroupId,
                  (c, r) => new { c, r })
                   .Where(m => m.r.PressSustainingOpNo == channelid)
                  .ToListAsync();
                if (result2.Count > 0)
                {
                    errorCode = true;
                }
                var result3 = await _mainDBContext.MasterPumpStationConfig.Join(
                 _mainDBContext.MasterPumpStationSteps,
                 c => c.MstpumpStationId,
                 r => r.MstpumpStationId,
                 (c, r) => new { c, r })
                  .Where(m => m.c.ControllingIpsensor == channelid)
                 .ToListAsync();
                if (result3.Count > 0)
                {
                    errorCode = true;
                }
                List<int> ids = new List<int>();
                ids.Add(6);
                ids.Add(8);
                ids.Add(9);
                var channelUsedMO = await (from r in _mainDBContext.Rtu
                                           join c in _mainDBContext.Channel on r.Rtuid equals c.Rtuid
                                           join me in _eventDBContext.ManualOverrideElements on c.ChannelId equals me.ObjectId
                                           join j in _eventDBContext.ManualOverrideMaster on me.Moid equals j.Moid
                                           where c.ChannelId == channelid && j.Status == "N" && j.IsDeleted == true && ids.Contains((int)me.ElementTypeId)
                                           select new { rtuname = r.Rtuname, channelname = c.ChannelName, MOName = j.Moname }).ToListAsync();
                if (channelUsedMO.Count > 0)
                {
                    errorCode = true;
                }
                List<int> ids2 = new List<int>();
                ids.Add(5);
                ids.Add(6);
                ids.Add(10);
                ids.Add(8);
                ids.Add(9);
                var channelUsedRule = await (from r in _mainDBContext.Rtu
                                             join c in _mainDBContext.Channel on r.Rtuid equals c.Rtuid
                                             join me in _eventDBContext.CustomRulesTarget on c.ChannelId equals me.ObjectId
                                             join j in _eventDBContext.CustomRulesMaster on me.RuleId equals j.RuleId
                                             where c.ChannelId == channelid && j.IsDeleted == true && ids.Contains((int)me.EventObjTypeId)
                                             select new { rtuname = r.Rtuname, channelname = c.ChannelName, rulename = j.RuleName }).ToListAsync();
                if (channelUsedRule.Count > 0)
                {
                    errorCode = true;
                }
                var channelUsedRuleCondition = await (from r in _mainDBContext.Rtu
                                                      join c in _mainDBContext.Channel on r.Rtuid equals c.Rtuid
                                                      join me in _eventDBContext.CustomRulesConditions on c.ChannelId equals me.ObjectId
                                                      join j in _eventDBContext.CustomRulesMaster on me.RuleId equals j.RuleId
                                                      where c.ChannelId == channelid && j.IsDeleted == true && ids.Contains((int)me.EventObjTypeId)
                                                      select new { rtuname = r.Rtuname, channelname = c.ChannelName, rulename = j.RuleName }).ToListAsync();
                if (channelUsedRuleCondition.Count > 0)
                {
                    errorCode = true;
                }
                List<string> ids2sensor = new List<string>();
                ids2sensor.Add("Primary Sensor");
                ids2sensor.Add("Secondary Sensor");
                var doconf = await (from r in _mainDBContext.Rtu
                                    join c in _mainDBContext.Channel on r.Rtuid equals c.Rtuid
                                    join me in _mainDBContext.EquipmentConfigValues on c.EqpId equals me.EqpId
                                    where c.ChannelId == channelid && ids2sensor.Contains(me.FieldName)
                                    select new { rtuname = r.Rtuname, channelname = c.ChannelName }).ToListAsync();

                if (doconf.Count > 0)
                {
                    errorCode = true;
                }

            }
            catch (Exception ex)
            {
                return errorCode;
            }
            return errorCode;
        }

        public async Task<bool> InsertDeletedSchedule(int schid, int cid, int PrgId, bool flag)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@SchNo", schid);
                    parameters1.Add("@ChannelId", cid);
                    parameters1.Add("@PrgId", PrgId);
                    parameters1.Add("@Reused", flag);
                    var result1 = await sqlConnection.QueryMultipleAsync("InsertDeletedSchedule", parameters1, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteSpecifigValveTimeSpan(int DeletedSeqId, int channelid, int strow, int endrow)
        {
            try
            {

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@DeletedSeqId", DeletedSeqId);
                    parameters1.Add("@channelid", channelid);
                    parameters1.Add("@stRow", strow);
                    parameters1.Add("@endRow", endrow);
                    var result1 = await sqlConnection.QueryMultipleAsync("DeleteSpecificValveTimeSpan", parameters1, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<DatesToConfigure>> GetDatesToConfigure(int SeqId)
        {
            List<DatesToConfigure> list = new List<DatesToConfigure>();
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", SeqId);
                    var result = await sqlConnection.QueryMultipleAsync("GetSequenceDatesToConfigure", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals.Select(x => new DatesToConfigure { date = x.date }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        #endregion
    }
}
