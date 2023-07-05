using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.EventEntities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface ISequenceService
    {

        Task<List<SequenceShortViewModel>> GetSequenceByProgramIdMulti(int progId);
        Task<List<SequenceShortViewModel>> GetSequenceByProgramId(int progId);
        Task<SequenceViewModel> GetSequenceById(int id);
        Task<List<OperationTypeViewModel>> GetOperationTypes();
        Task<int> GetUnusedDeletedSeqNoByProgNo(int progId);
        Task<int> GetSeqMaxNoByProgId(int progId);
        Task<bool> GetDummySequenceById(int seqId);
        Task<List<SequenceErrDetailsViewModel>> ValidateSequenceById(int seqId);
        Task<string> DeleteSequenceBySeqId(List<int> seqIds);

        Task<string> DeleteSequenceBySeqIdNew(int seqIds);

        Task<string> ResetUploadSeq();
        Task<string> DeleteAllSequence();
        Task<List<DatesToConfigure>> GetDatesToConfigure(int SeqId);
        Task<SequenceViewModel> Add(SequenceViewModel model);
        Task<SequenceViewModel> Edit(SequenceViewModel model);
        Task<List<SequenceValveDataViewModel>> GetSequenceValveData(int seqId, int prjId, int progId, int networkId, int zoneId, string seqType);
        Task<List<SequenceValveDataViewModel>> GetSequenceValveData(int seqId);
        Task<List<SequenceValveDataViewModel>> GetSequenceValveDataMulti(int seqId);

        Task<SeqValveGroupViewModel> GetSeqValveGroup(string seqType, int networkId, int zoneId);
    }

    public class SequenceService : ISequenceService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<SequenceService> _logger;
        private IZoneTimeService _zoneTimeService;
        private IValveService _valveService;
        private EventDBContext _eventDBContext;

        public SequenceService(ILogger<SequenceService> logger,
                MainDBContext uciDBContext, EventDBContext eventDBContext,
                IMapper mapper,
                 IZoneTimeService zoneTimeService,
                   IValveService valveService
            )
        {
            _valveService = valveService;
            _mapper = mapper;
            _mainDBContext = uciDBContext;
            _eventDBContext = eventDBContext;
            _logger = logger;
            _zoneTimeService = zoneTimeService;
        }

        /// <summary>
        /// Get sequences by program id
        /// </summary>
        /// <param name="progId"></param>
        /// <returns>List<SequenceShortViewModel></returns>
        public async Task<List<SequenceShortViewModel>> GetSequenceByProgramIdMulti(int progId)
        {
            try
            {
                var sequences = await _mainDBContext.NewSequence.Select(x => new SequenceShortViewModel
                {
                    BasisOfOp = x.BasisOfOp,
                    NetworkId = x.NetworkId,
                    PrgId = x.PrgId,
                    PrjId = x.PrjId,
                    SeqEndDate = x.SeqEndDate,
                    SeqId = x.SeqId,
                    SeqStartDate = x.SeqStartDate,
                    SeqTagName = x.SeqTagName,
                    SeqType = x.SeqType,
                    SeqName = x.SeqName,
                    NetworkName = "All",
                    SeqMasterStartTime = x.StartTime,
                    IsValid = (bool)x.IsValid,
                    IntervalDays = x.IntervalDays,
                }).ToListAsync();
                // get network, if network is 0 then show 'All' in network name
                foreach (var item in sequences)
                {
                    var network = await _mainDBContext.Network.Where(x => x.NetworkId == item.NetworkId).FirstOrDefaultAsync();
                    if (network != null)
                    {
                        item.NetworkName = network.Name;
                    }

                    var seqweek = await _mainDBContext.NewSequenceWeeklySchedule.Where(x => x.SeqId == item.SeqId).ToListAsync();
                    if (seqweek != null)
                    {
                        item.WeekDays = seqweek.Select(x => (int)x.WeekDayId).ToList();
                    }
                }

                return _mapper.Map<List<SequenceShortViewModel>>(sequences);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetSequenceByProgramId)}]{ex}");
                throw ex;
            }
        }


        /// <summary>
        /// Get sequences by program id
        /// </summary>
        /// <param name="progId"></param>
        /// <returns>List<SequenceShortViewModel></returns>
        public async Task<List<SequenceShortViewModel>> GetSequenceByProgramId(int progId)
        {
            try
            {
                var sequences = await _mainDBContext.Sequence
                    .Where(x => x.PrgId == progId).Include(x => x.SequenceWeeklySchedule)
                    .Join(_mainDBContext.Zone,
                    s => s.ZoneId,
                    z => z.ZoneId,
                    (s, z) => new { s, z })
                    .Join(_mainDBContext.SequenceMasterConfig,
                    x => x.s.SeqId,
                    m => m.SeqId,
                    (x, m) => new SequenceShortViewModel
                    {
                        BasisOfOp = x.s.BasisOfOp,
                        NetworkId = x.s.NetworkId,
                        PrgId = x.s.PrgId,
                        PrjId = x.s.PrjId,
                        SeqEndDate = x.s.SeqEndDate,
                        SeqId = x.s.SeqId,
                        SeqStartDate = x.s.SeqStartDate,
                        SeqTagName = x.s.SeqTagName,
                        SeqType = x.s.SeqType,
                        ZoneId = x.s.ZoneId,
                        ZoneName = x.z.Name,
                        SeqName = x.s.SeqName,
                        NetworkName = "All",
                        SeqMasterStartTime = m.StartTime,
                        IsValid = (bool)x.s.IsValid,
                        IntervalDays = x.s.IntervalDays,
                        WeekDays = x.s.SequenceWeeklySchedule.Select(x => x.WeekDayId).ToList()
                    }).ToListAsync();
                // get network, if network is 0 then show 'All' in network name
                foreach (var item in sequences)
                {
                    var network = await _mainDBContext.Network.Where(x => x.NetworkId == item.NetworkId).FirstOrDefaultAsync();
                    if (network != null)
                    {
                        item.NetworkName = network.Name;
                    }
                }

                return _mapper.Map<List<SequenceShortViewModel>>(sequences);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetSequenceByProgramId)}]{ex}");
                throw ex;
            }
        }

        public async Task<string> DeleteSequenceBySeqId(List<int> lstSeqIds)
        {
            string resultText = String.Empty;
            try
            {

                foreach (var seqId in lstSeqIds)
                {
                    //await DeleteValveTimeSpan(seqId);

                    SequenceViewModel sequenceViewModel = await GetSequenceById(seqId);
                    bool delsequence = true;
                    List<CustomRulesTarget> rt = await _eventDBContext.CustomRulesTarget.Where(n => n.EventObjTypeId == 12 && n.ObjectId == seqId).ToListAsync();
                    List<ManualOverrideElements> mt = await _eventDBContext.ManualOverrideElements.Where(n => n.ElementTypeId == 12 && n.ObjectId == seqId).ToListAsync();
                    if (rt != null)
                    {
                        if (rt.Count > 0)
                        {
                            foreach (CustomRulesTarget rel in rt)
                            {
                                CustomRulesMaster rm = await _eventDBContext.CustomRulesMaster.FirstOrDefaultAsync(x => x.RuleId == rel.RuleId && x.IsDeleted == false && x.RuleStatus == 0);
                                if (rm != null)
                                {
                                    resultText = GlobalConstants.Cannotdeletesequencitusedruletarget;
                                    delsequence = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (mt != null)
                    {
                        if (mt.Count > 0)
                        {
                            foreach (ManualOverrideElements el in mt)
                            {
                                ManualOverrideMaster m = await _eventDBContext.ManualOverrideMaster.FirstOrDefaultAsync(x => x.Moid == el.Moid && x.IsDeleted == false && x.Status == "N");
                                if (m != null)
                                {
                                    resultText = GlobalConstants.Cannotdeletesequenceusedmanualoverrideelement;
                                    delsequence = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (delsequence)
                    {
                        resultText = await DeleteSequece(seqId, sequenceViewModel.PrjId, sequenceViewModel.NetworkId);

                        ProgramIndividual progObj = null;//await _mainDBContext.ProgramIndividual.Where(x => x.ProgId == sequenceViewModel.PrgId).FirstOrDefaultAsync();

                        if (CheckSequenceValidity(sequenceViewModel.PrgId) == false)
                        {
                            progObj.IsLocked = true;
                            await _mainDBContext.SaveChangesAsync();
                            UpdateEventLogForProgramLock(4, "N");

                        }
                        else
                        {
                            if (progObj != null)
                            {
                                progObj.IsLocked = true;
                                await _mainDBContext.SaveChangesAsync();
                                UpdateEventLogForProgramLock(4, "C");

                            }
                        }

                    }
                }

                // await _mainDBContext.Sequence.Where(s1 =>lstSeqIds.Contains(s1.SeqId)).ToListAsync().ForEach(item => await _mainDBContext.Sequence.Remove(item));

                return resultText;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(DeleteSequenceBySeqId)}]{ex}");
                return resultText;
            }
        }



        public async Task<string> DeleteAllSequence()
        {
            string resultText = String.Empty;
            try
            {
                List<NewSequence> seqList = await _mainDBContext.NewSequence.ToListAsync();
                List<NewSequenceValveConfig> seqListValve = await _mainDBContext.NewSequenceValveConfig.ToListAsync();
                List<NewSequenceWeeklySchedule> seqListweeklySchedule = await _mainDBContext.NewSequenceWeeklySchedule.ToListAsync();

                _mainDBContext.NewSequence.RemoveRange(seqList);
                await _mainDBContext.SaveChangesAsync();

                List<NrseqUpids> neSeqList = _mainDBContext.NrseqUpids.ToList();

                foreach (var item in neSeqList)
                {
                    item.UpId = item.UpId + 1;
                }

                _mainDBContext.NrseqUpids.UpdateRange(neSeqList);
                await _mainDBContext.SaveChangesAsync();

                _mainDBContext.NewSequenceValveConfig.RemoveRange(seqListValve);
                await _mainDBContext.SaveChangesAsync();

                _mainDBContext.NewSequenceWeeklySchedule.RemoveRange(seqListweeklySchedule);
                await _mainDBContext.SaveChangesAsync();

                var projectuids = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                if (projectuids != null)
                {
                    projectuids.ProjectUpId = projectuids.ProjectUpId + 1;
                    _mainDBContext.UpdateIdsProject.Update(projectuids);
                    await _mainDBContext.SaveChangesAsync();
                }
                //var gwSch = await _mainDBContext.GatewayMaxSch.ToListAsync();
                //foreach (var item in gwSch)
                //{
                //    item.MaxSchUpId = item.MaxSchUpId + 1;
                //}
                //_mainDBContext.GatewayMaxSch.UpdateRange(gwSch);
                //await _mainDBContext.SaveChangesAsync();

                #region Update Main sequence Id
                var upseqMain = await _mainDBContext.UpdateIdsMainSch.FirstOrDefaultAsync();
                if (upseqMain != null)
                {
                    upseqMain.SeqMaxUpid = upseqMain.SeqMaxUpid + 1;
                    _mainDBContext.UpdateIdsMainSch.Update(upseqMain);
                    await _mainDBContext.SaveChangesAsync();
                }
                else
                {
                    UpdateIdsMainSch updateIdsMainSch = new UpdateIdsMainSch();
                    updateIdsMainSch.SeqMaxUpid = 0;
                    updateIdsMainSch.GwSeqMaxUpId = 0;
                    await _mainDBContext.UpdateIdsMainSch.AddAsync(updateIdsMainSch);
                    await _mainDBContext.SaveChangesAsync();
                }
                #endregion

                return resultText;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(DeleteSequenceBySeqId)}]{ex}");
                return resultText;
            }
        }


        public async Task<string> ResetUploadSeq()
        {
            string resultText = String.Empty;
            try
            {
                var sequpload = _mainDBContext.MultiSequenceUploading.FirstOrDefault();
                sequpload.SeqUploadingFlag = false;
                _mainDBContext.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(ResetUploadSeq)}]{ex}");
                return resultText;
            }
        }
        public async Task<string> DeleteSequenceBySeqIdNew(int seqId)
        {
            string resultText = String.Empty;
            try
            {
                List<NewSequence> seqList = await _mainDBContext.NewSequence.Where(x => x.SeqId == seqId).ToListAsync();
                List<NewSequenceValveConfig> seqListValve = await _mainDBContext.NewSequenceValveConfig.Where(x => x.SeqId == seqId).ToListAsync();
                List<NewSequenceWeeklySchedule> seqListweeklySchedule = await _mainDBContext.NewSequenceWeeklySchedule.Where(x => x.SeqId == seqId).ToListAsync();
                //Channel id to delete
                List<int> channelNamesToDelete = seqListValve.Select(x =>(int) x.ChannelId).ToList();
                List<int> nodeIdsToUpdate = _mainDBContext.Nrvchannels.Where(x => channelNamesToDelete.Contains(x.Id)).Select(x => (int)x.RtuId).ToList();

                _mainDBContext.NewSequence.RemoveRange(seqList);
                await _mainDBContext.SaveChangesAsync();

                List<NrseqUpids> neSeqList = _mainDBContext.NrseqUpids.Where(x=> nodeIdsToUpdate.Contains((int)x.NodeRtuId)).ToList();

                foreach (var item in neSeqList)
                {
                    item.UpId = item.UpId + 1;
                }
                _mainDBContext.NrseqUpids.UpdateRange(neSeqList);
                await _mainDBContext.SaveChangesAsync();

                _mainDBContext.NewSequenceValveConfig.RemoveRange(seqListValve);
                await _mainDBContext.SaveChangesAsync();

                _mainDBContext.NewSequenceWeeklySchedule.RemoveRange(seqListweeklySchedule);
                await _mainDBContext.SaveChangesAsync();

                var projectuids = await _mainDBContext.UpdateIdsProject.FirstOrDefaultAsync();
                if (projectuids != null)
                {
                    projectuids.ProjectUpId = projectuids.ProjectUpId + 1;
                    _mainDBContext.UpdateIdsProject.Update(projectuids);
                    await _mainDBContext.SaveChangesAsync();
                }
                //var gwSch = await _mainDBContext.GatewayMaxSch.ToListAsync();
                //foreach (var item in gwSch)
                //{
                //    item.MaxSchUpId = item.MaxSchUpId + 1;
                //}
                //_mainDBContext.GatewayMaxSch.UpdateRange(gwSch);
                //await _mainDBContext.SaveChangesAsync();

                #region Update Main sequence Id
                var upseqMain = await _mainDBContext.UpdateIdsMainSch.FirstOrDefaultAsync();
                if (upseqMain != null)
                {
                    upseqMain.SeqMaxUpid = upseqMain.SeqMaxUpid + 1;
                    _mainDBContext.UpdateIdsMainSch.Update(upseqMain);
                    await _mainDBContext.SaveChangesAsync();
                }
                else
                {
                    UpdateIdsMainSch updateIdsMainSch = new UpdateIdsMainSch();
                    updateIdsMainSch.SeqMaxUpid = 0;
                    updateIdsMainSch.GwSeqMaxUpId = 0;
                    await _mainDBContext.UpdateIdsMainSch.AddAsync(updateIdsMainSch);
                    await _mainDBContext.SaveChangesAsync();
                }
                #endregion
                return resultText;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(DeleteSequenceBySeqId)}]{ex}");
                return resultText;
            }
        }

        #region Private methods - StoreProcedures
        //SP- DeleteAllValveTimeSpan
        public async Task<bool> DeleteValveTimeSpan(int deleteId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DeletedSeqId", deleteId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteAllValveTimeSpan", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //SP- DeleteAllSequenceValves
        public async Task<bool> DeleteAllSequencesValves(int deleteId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", deleteId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteAllSequenceValves", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //SP- DeleteSeqMasterConfig
        public async Task<bool> DeleteSeqMasterConfig(int deleteId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", deleteId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSeqMasterConfig", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //SP- DeleteSeqMasterConfig
        public async Task<bool> DeleteSeqWeeklySch(int deleteId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", deleteId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSeqWeekly", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //SP- DeleteSequenceValveConfig
        public async Task<bool> DeleteSequenceValveConfig(int seqGrEleId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqGrEleId", seqGrEleId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSequenceValveConfig", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //SP- DeleteSequenceValveConfig
        public async Task<bool> DeleteSequence(int seqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", seqId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSequence", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InsertAllDeleteEvents(int deleteId, DateTime TimeZoneDateTime)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DeletedSeqId", deleteId);
                    parameters.Add("@TimeZoneDateTime", TimeZoneDateTime);
                    var result = await sqlConnection.QueryMultipleAsync("AddDeleteEventsForSequence", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSequenceErrors(int deleteId, bool IsError)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", deleteId);
                    parameters.Add("@IsError", IsError);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSequenceErrors", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSequenceLooping(int deleteId, float startid)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", deleteId);
                    parameters.Add("@StartId", startid);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteSequenceLooping", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> InsertDeletedSequence(int SeqNo, int ProgramId, Boolean reused)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqNo", SeqNo);
                    parameters.Add("@ProgramNo", ProgramId);
                    parameters.Add("@Reused", reused);
                    var result = await sqlConnection.QueryMultipleAsync("InsertDeletedSequence", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        private async Task<string> DeleteSequece(int deleteId, int pid, int nid)
        {
            try
            {
                string resultText = string.Empty;

                //First remove time stamp 
                //Remove time stamp for this valve
                bool result1 = await DeleteValveTimeSpan(deleteId);
                //TODO: PA-
                //int isdeleting = jas.InsertAllDeletedSchedule(deleteId, false);
                //if (isdeleting == 1)
                //{
                //    resultText = "Another User is trying to delete Sequence Or Sequence already Deleted by other User";
                //}            

                bool result2 = await InsertAllDeleteEvents(deleteId, await _zoneTimeService.TimeZoneDateTime());
                bool result3 = await DeleteAllSequencesValves(deleteId);

                //Remove sequence valve config           
                bool result4 = await DeleteSeqMasterConfig(deleteId);

                ////Then remove weekly schedule
                bool result5 = await DeleteSeqWeeklySch(deleteId);

                //Delete error details
                bool result6 = await DeleteSequenceErrors(deleteId, true);
                bool result7 = await DeleteSequenceErrors(deleteId, false);

                //Delete looping entries
                bool result8 = await DeleteSequenceLooping(deleteId, -1);

                //Now delete sequence
                //Insert deleted sequence
                //HD
                //17 Mar 2015
                int sno = 0;
                int prgid = 0;
                Sequence s = await _mainDBContext.Sequence.FirstOrDefaultAsync(s1 => s1.SeqId == deleteId);
                //DeleteSequence - Created new SP 
                var restult9 = await DeleteSequence(deleteId);
                //try
                //{
                //    _mainDBContext.Sequence.Remove(s);
                //    await _mainDBContext.SaveChangesAsync();
                //}
                //catch (DbUpdateConcurrencyException)
                //{

                //}

                //await _mainDBContext.DisposeAsync();
                //List<Sequence> seq = await _mainDBContext.Sequence.Where(s1 => lstSeqIds.Contains(s1.SeqId)).ToListAsync();
                //_mainDBContext.Entry(seq).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                //_mainDBContext.Sequence.RemoveRange(seq);
                //await _mainDBContext.SaveChangesAsync();
                if (s != null)
                {
                    if (string.IsNullOrEmpty(s.SeqNo.ToString()) == false)
                    {
                        sno = Convert.ToInt32(s.SeqNo);
                        prgid = s.PrgId;
                    }
                }

                if (sno != 0)
                {
                    await InsertDeletedSequence(sno, prgid, false);
                }
                return "Success";
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        /// <summary>
        /// Get sequence by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>SequenceViewModel</returns>
        public async Task<SequenceViewModel> GetSequenceById(int id)
        {
            try
            {

                var sequence = _mainDBContext.Sequence.Where(x => x.SeqId == id)
                    .Include(x => x.SequenceErrDetails)
                    .Include(x => x.SequenceMasterConfig)
                    .Include(x => x.SequenceValveConfig)
                    .Include(x => x.SequenceWeeklySchedule)
                    .Include(x => x.PrjType);

                int prgID = sequence.FirstOrDefault().PrgId;
                ProgramIndividual progObj = null;//await _mainDBContext.ProgramIndividual.Where(x => x.ProgId == prgID).FirstOrDefaultAsync();


                if (progObj != null)
                {
                    progObj.IsLocked = false;
                    await _mainDBContext.SaveChangesAsync();
                    UpdateEventLogForProgramLock(4, "N");

                }


                return _mapper.Map<SequenceViewModel>(await sequence.FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetSequenceById)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get operation types
        /// </summary>
        /// <returns>List<OperationTypeViewModel></returns>
        public async Task<List<OperationTypeViewModel>> GetOperationTypes()
        {
            try
            {
                var operationTypes = await _mainDBContext.OperationType.ToListAsync();
                return _mapper.Map<List<OperationTypeViewModel>>(operationTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetOperationTypes)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get unused deleted sequence number by program id
        /// </summary>
        /// <param name="progId"></param>
        /// <returns></returns>
        public async Task<int> GetUnusedDeletedSeqNoByProgNo(int progNo)
        {
            try
            {
                // here progId is inserted in column programno while deleting seq
                var deletedSeq = await _mainDBContext.DeletedSequence.Where(x => x.ProgramNo == progNo && x.Reused == false).FirstOrDefaultAsync();
                if (deletedSeq != null)
                {
                    return (int)deletedSeq.SeqNo;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetUnusedDeletedSeqNoByProgNo)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get sequence max number by program id
        /// </summary>
        /// <param name="progId"></param>
        /// <returns>int</returns>
        public async Task<int> GetSeqMaxNoByProgId(int progId)
        {
            try
            {
                var sequence = await _mainDBContext.Sequence.Where(x => x.PrgId == progId).ToListAsync();
                if (sequence.Count > 0)
                {
                    return ((int)sequence.Max(x => x.SeqNo) + 1);
                }
                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetSeqMaxNoByProgId)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Validate Sequence
        /// </summary>
        /// <param name="seqId"></param>
        /// <returns></returns>
        public async Task<List<SequenceErrDetailsViewModel>> ValidateSequenceById(int progId)
        {
            List<string> warningmsg = new List<string>();
            string errormsg = "";
            ErrorMsg errorMsg = new ErrorMsg();
            List<SequenceErrDetailsViewModel> errseq = new List<SequenceErrDetailsViewModel>();
            try
            {
                bool isValidSequence = false;
                //We will validate all sequences in current program starting with current sequence
                //Hence we are sorting sequence id in desc order to validate most recently added sequence first

                List<Sequence> lstAllSeqInProg = await _mainDBContext.Sequence.Where(n => n.PrgId == progId).OrderBy(n => n.SeqId).ToListAsync();
                if (lstAllSeqInProg != null)
                {
                    if (lstAllSeqInProg.Count > 0)
                    {
                        foreach (Sequence seq in lstAllSeqInProg)
                        {
                            //Validate Sequence start/end dates within program start end dates.
                            errorMsg = await ValidateSeqStartEndDateAsync(seq);// ValidateSeqStartEndDate(s);
                            if (errorMsg.flag)
                            {
                                //Sequence schedule oveflowing beyond zone day end time 
                                errorMsg = await ValidateZoneDayEndTime(seq);// ValidateZoneDayEndTime(s);
                            }


                            if (errorMsg.flag)
                            {
                                //o/p no. ## trying to operate more than 30 times
                                errorMsg = await ValidateValveNetworkZoneRelation(seq);// ValidateValveNetworkZoneRelation(s);
                            }



                            if (errorMsg.flag)
                            {
                                //o/p no. ## trying to operate more than 30 times
                                errorMsg = await ValidateValveForMoreThanThirtyTimesAsync(seq);// ValidateValveForMoreThanThirtyTimes(s);
                            }


                            if (errorMsg.flag)
                            {
                                //group. ## trying to operate more than 25 times
                                errorMsg = await ValidateGroupForMoreThanTwentyFiveTimesAsync(seq);
                            }


                            if (errorMsg.flag)
                            {
                                //Fert duration more than valve duration
                                errorMsg = await ValidateForFertDurMoreThanValveDur(seq);// ValidateForFertDurMoreThanValveDur(s);
                            }


                            if (errorMsg.flag)
                            {
                                //Validate if not start time is saved during importing 
                                errorMsg = await ValidateForStarttimeBlankAsync(seq);// ValidateForFertDurMoreThanValveDur(s);
                            }
                            //Validate for warnings

                            //Check sequence weekly schedule
                            warningmsg = await ValidateForSequenceWeeklySchedule(seq);

                            //errormsg = vs.errormsg;
                            // warningmsg = vs.warningmsg;

                            List<SequenceErrDetailsViewModel> errs = await UpdateSequenceValidityAsync(seq, errorMsg.flag, warningmsg, errorMsg.errors);
                            errseq.AddRange(errs);
                            if (seq.IsValid == true)
                            {
                                //Mark all events as "N"                            
                                await UpdateEventForSequence(Convert.ToInt32(seq.SeqId));
                                //Lock program when sequence is valid 
                                isValidSequence = true;
                            }
                            else
                            {
                                isValidSequence = false;
                            }

                        }
                        if (isValidSequence)
                        {
                            ProgramIndividual progObj = null;// await _mainDBContext.ProgramIndividual.Where(x => x.ProgId == progId).FirstOrDefaultAsync();

                            if (CheckSequenceValidity(progId) == false)
                            {
                                progObj.IsLocked = true;
                                await _mainDBContext.SaveChangesAsync();
                                UpdateEventLogForProgramLock(4, "N");

                            }
                            else
                            {
                                if (progObj != null)
                                {
                                    progObj.IsLocked = true;
                                    await _mainDBContext.SaveChangesAsync();
                                    UpdateEventLogForProgramLock(4, "C");

                                }
                            }


                        }
                    }
                }
                return errseq;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DummySequence)}.{nameof(GetDummySequenceById)}]{ex}");
                return null;
            }
        }

        public async Task<bool> UpdateEventLogForProgramLock(int ProgId, string status)
        {
            try
            {
                DateTime dateTime = await _zoneTimeService.TimeZoneDateTime();
                EventViewModel evt = new EventViewModel();

                evt.ObjName = "Program";
                evt.networkId = 0;
                evt.prjId = 4;
                evt.objIdinDB = ProgId;
                evt.action = "L";
                evt.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();
                int objTypeId = await _eventDBContext.EventObjectTypes.Where(x => x.ObjectName == "Program").Select(x => x.EventObjTypeId).FirstOrDefaultAsync();
                List<Events> dsEvents = await _eventDBContext.Events.Where(x => x.IsSentToBst == false && x.ObjectIdInDb == ProgId && x.ObjTypeId == objTypeId && x.ActionExecuted == "L").ToListAsync();

                if (dsEvents != null)
                {
                    if (dsEvents.Count > 0)
                    {
                        return await UpdateEvent(Convert.ToInt32(dsEvents[0].EventId.ToString()), status, dateTime);
                    }
                    else
                    {
                        return await AdddEvent(evt);
                    }
                }
                else
                {
                    return await AdddEvent(evt);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occure in BindProgrammsData in MaintainProgram page in step = ");
                return false;
                throw ex;
            }
        }

        public bool CheckSequenceValidity(int ProgID)
        {
            bool isValid = true;
            try
            {
                //Check if all sequence under this program are validated. else don't allow user to lock.
                List<Sequence> AllSeq = _mainDBContext.Sequence.Where(e => e.PrgId == ProgID).ToList();


                if (AllSeq != null)
                {
                    if (AllSeq.Count > 0)
                    {
                        foreach (Sequence s in AllSeq)
                        {
                            if (s.ValidationState == false)
                            {
                                isValid = false;
                                break;
                            }
                            else if (s.IsValid == false)
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message;
                //  _log.Error(ErrorMessage);
            }

            return isValid;
        }

        /// <summary>
        /// Get dummy sequence by id
        /// </summary>
        /// <param name="seqId"></param>
        /// <returns></returns>
        public async Task<bool> GetDummySequenceById(int seqId)
        {
            try
            {
                var sequence = await _mainDBContext.DummySequence.Where(x => x.SeqId == seqId).FirstOrDefaultAsync();
                return sequence == null ? false : true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DummySequence)}.{nameof(GetDummySequenceById)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Add sequence
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<SequenceViewModel> Add(SequenceViewModel model)
        {
            try
            {
                Sequence sequence = new Sequence();
                sequence.SeqId = model.SeqId;
                sequence.PrjId = model.PrjId;
                sequence.PrgId = model.PrgId;
                sequence.NetworkId = model.NetworkId;
                sequence.ZoneId = model.ZoneId;
                sequence.SeqStartDate = model.SeqStartDate;
                sequence.SeqEndDate = model.SeqEndDate;
                sequence.IsProgrammable = model.IsProgrammable;
                sequence.BasisOfOp = model.BasisOfOp;
                sequence.IntervalDays = model.IntervalDays;
                sequence.PrjTypeId = model.PrjTypeId;
                sequence.OperationTypeId = model.OperationTypeId;
                sequence.MannualorAutoEt = model.MannualorAutoEt;
                sequence.IsSmartEt = model.IsSmartEt;
                sequence.SeqNo = model.SeqNo;
                sequence.SeqDesc = model.SeqDesc;
                sequence.SeqTagName = model.SeqTagName;
                sequence.SeqType = model.SeqType;
                sequence.IsSent = model.IsSent;
                sequence.SeqName = model.SeqName;
                await _mainDBContext.Sequence.AddAsync(sequence);
                await _mainDBContext.SaveChangesAsync();
                model.SeqId = sequence.SeqId;

                DeletedSequence deletedSequence = await _mainDBContext.DeletedSequence.FirstOrDefaultAsync(n => n.SeqNo == model.SeqNo && n.ProgramNo == model.PrgId && n.Reused == false);
                if (deletedSequence != null)
                {
                    deletedSequence.Reused = true;
                    await _mainDBContext.SaveChangesAsync();
                }
                // save start time in sequence master config
                await SaveStartTime(model.SeqId, model.PrjId, model.PrgId, model.NetworkId, model.ZoneId, model.SeqMasterStartTime);
                if (model.BasisOfOp == "Weekly")
                {
                    // save weekly configuration
                    await SaveWeeklyConfiguration(model.WeekDays, model.PrjId, model.PrgId, model.SeqId);
                }


                // save sequence in loop
                //DateTime prgStDate;
                //DateTime prgEndDate;
                //bool programLoopStatus = false;
                //var program = await _programService.GetProgramDates(model.PrgId);
                //prgStDate = program.prgStDate;
                //prgEndDate = program.prgEndDate;
                //programLoopStatus = program.programLoopStatus;
                //if (programLoopStatus)
                //{
                //    await SaveSequenceInLoopAsync(sequence);
                //}
                model = await GetSequenceById(model.SeqId);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(Add)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Edit sequence
        /// </summary>
        /// <param name="model"></param>
        /// <returns>SequenceViewModel</returns>
        public async Task<SequenceViewModel> Edit(SequenceViewModel model)
        {
            try
            {
                var sequence = _mainDBContext.Sequence.Where(x => x.SeqId == model.SeqId).FirstOrDefault();
                sequence.SeqStartDate = model.SeqStartDate;
                sequence.SeqEndDate = model.SeqEndDate;
                sequence.IsProgrammable = model.IsProgrammable;
                sequence.BasisOfOp = model.BasisOfOp;
                sequence.IntervalDays = model.IntervalDays;
                sequence.PrjTypeId = model.PrjTypeId;
                sequence.OperationTypeId = model.OperationTypeId;
                sequence.MannualorAutoEt = model.MannualorAutoEt;
                sequence.IsSmartEt = model.IsSmartEt;
                sequence.SeqNo = model.SeqNo;
                sequence.SeqDesc = model.SeqDesc;
                sequence.SeqTagName = model.SeqTagName;
                sequence.SeqType = model.SeqType;
                sequence.IsSent = model.IsSent;
                sequence.SeqName = model.SeqName;
                await _mainDBContext.SaveChangesAsync();

                // save start time
                await SaveStartTime(model.SeqId, model.PrjId, model.PrgId, model.NetworkId, model.ZoneId, model.SeqMasterStartTime);
                // save weekly configuration
                await SaveWeeklyConfiguration(model.WeekDays, model.PrjId, model.PrgId, model.SeqId);

                // save sequence in loop   
                DateTime prgStDate;
                DateTime prgEndDate;
                bool programLoopStatus = false;
               
                SequenceMasterConfig sequenceMasterConfig = await _mainDBContext.SequenceMasterConfig.Where(x => x.SeqId == model.SeqId && x.StartId == 1).FirstOrDefaultAsync();
                await UpdateValveStartTime(model, sequenceMasterConfig.MstseqId, 1, model.SeqId, Convert.ToDateTime(model.SeqStartDate), Convert.ToDateTime(model.SeqEndDate));

                model = await GetSequenceById(sequence.SeqId);
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(Edit)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get sequence details of selected sequence for editing
        /// Stored procedure GetSequenceData table[2] converted to this method
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="prjId"></param>
        /// <param name="progId"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public async Task<List<SequenceValveDataViewModel>> GetSequenceValveData(int seqId, int prjId, int progId, int networkId, int zoneId, string seqType)
        {
            try
            {
                var seqValveData = from m in _mainDBContext.SequenceMasterConfig
                                   from v in _mainDBContext.SequenceValveConfig
                                   from s in _mainDBContext.Sequence
                                   from t in _mainDBContext.TimeIntervals
                                   where m.MstseqId == v.MstseqId &&
                                       m.SeqId == v.SeqId &&
                                       s.SeqId == m.SeqId &&
                                       t.StartTime == v.ValveStartTime &&
                                       m.SeqId == seqId &&
                                       m.ProjectId == prjId &&
                                       m.NetworkId == networkId &&
                                       m.ZoneId == zoneId
                                   orderby m.SeqId, m.StartId, m.StartTime, v.HorizGrId, v.IsHorizontal descending
                                   select new
                                   {
                                       m.MstseqId,
                                       m.SeqId,
                                       m.ProjectId,
                                       m.PrgId,
                                       m.NetworkId,
                                       m.ZoneId,
                                       m.StartId,
                                       v.ValveStartTime,
                                       s.SeqNo,
                                       t.TimeSpanId,
                                       v.GroupName,
                                       Valve = "NRV",
                                       v.ChannelId,
                                       v.ValveStartDuration
                                   };

                if (seqType != "NRV")
                {
                    seqValveData = from l in seqValveData
                                   from sub in _mainDBContext.SubBlock
                                   where l.ChannelId == sub.ChannelId
                                   select new
                                   {
                                       l.MstseqId,
                                       l.SeqId,
                                       l.ProjectId,
                                       l.PrgId,
                                       l.NetworkId,
                                       l.ZoneId,
                                       l.StartId,
                                       l.ValveStartTime,
                                       l.SeqNo,
                                       l.TimeSpanId,
                                       l.GroupName,
                                       Valve = sub.Name,
                                       l.ChannelId,
                                       l.ValveStartDuration
                                   };
                }

                return await seqValveData.Select(x => new SequenceValveDataViewModel()
                {
                    MstseqId = x.MstseqId,
                    SeqId = x.SeqId,
                    ProjectId = x.ProjectId,
                    PrgId = x.PrgId,
                    NetworkId = x.NetworkId,
                    ZoneId = x.ZoneId,
                    StartId = x.StartId,
                    ValveStartTime = x.ValveStartTime,
                    SeqNo = x.SeqNo,
                    TimeSpanId = x.TimeSpanId,
                    GroupName = x.GroupName,
                    Valve = x.Valve,
                    ChannelId = x.ChannelId,
                    ValveStartDuration = x.ValveStartDuration
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Get sequence details of selected sequence for editing
        /// Stored procedure GetSequenceData table[2] converted to this method
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="prjId"></param>
        /// <param name="progId"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public async Task<List<SequenceValveDataViewModel>> GetSequenceValveData(int seqId)
        {
            try
            {

                Sequence seq = await _mainDBContext.Sequence.Where(x => x.SeqId == seqId).FirstOrDefaultAsync();
                var seqValveData = from m in _mainDBContext.SequenceMasterConfig
                                   from v in _mainDBContext.SequenceValveConfig
                                   from s in _mainDBContext.Sequence
                                   from t in _mainDBContext.TimeIntervals
                                   where m.MstseqId == v.MstseqId &&
                                       m.SeqId == v.SeqId &&
                                       s.SeqId == m.SeqId &&
                                       t.StartTime == v.ValveStartTime &&
                                       m.SeqId == seqId &&
                                       m.ProjectId == seq.PrjId &&
                                       m.NetworkId == seq.NetworkId &&
                                       m.ZoneId == seq.ZoneId

                                   orderby m.SeqId, m.StartId, m.StartTime, v.HorizGrId, v.IsHorizontal descending
                                   select new
                                   {
                                       m.MstseqId,
                                       m.SeqId,
                                       m.ProjectId,
                                       m.PrgId,
                                       m.NetworkId,
                                       m.ZoneId,
                                       m.StartId,
                                       v.ValveStartTime,
                                       s.SeqNo,
                                       t.TimeSpanId,
                                       v.GroupName,
                                       v.Valve,
                                       //Valve = "NRV",
                                       v.ChannelId,
                                       v.ValveStartDuration,
                                       v.IsFertilizerRelated,
                                       v.IsFlushRelated,
                                       v.ScheduleNo
                                   };

                if (seq.SeqType != "NRV")
                {
                    seqValveData = from l in seqValveData
                                   from sub in _mainDBContext.SubBlock
                                   where l.ChannelId == sub.ChannelId
                                   select new
                                   {
                                       l.MstseqId,
                                       l.SeqId,
                                       l.ProjectId,
                                       l.PrgId,
                                       l.NetworkId,
                                       l.ZoneId,
                                       l.StartId,
                                       l.ValveStartTime,
                                       l.SeqNo,
                                       l.TimeSpanId,
                                       l.GroupName,
                                       Valve = sub.Name,
                                       l.ChannelId,
                                       l.ValveStartDuration,
                                       l.IsFertilizerRelated,
                                       l.IsFlushRelated,
                                       l.ScheduleNo
                                   };
                }

                return await seqValveData.Select(x => new SequenceValveDataViewModel()
                {
                    MstseqId = x.MstseqId,
                    SeqId = x.SeqId,
                    ProjectId = x.ProjectId,
                    PrgId = x.PrgId,
                    NetworkId = x.NetworkId,
                    ZoneId = x.ZoneId,
                    StartId = x.StartId,
                    ValveStartTime = x.ValveStartTime,
                    SeqNo = x.SeqNo,
                    TimeSpanId = x.TimeSpanId,
                    GroupName = x.GroupName,
                    Valve = x.Valve,
                    ChannelId = x.ChannelId,
                    ValveStartDuration = x.ValveStartDuration,
                    IsFertilizerRelated = x.IsFertilizerRelated,
                    IsFlushRelated = x.IsFlushRelated,
                    ScheduleNo = x.ScheduleNo
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Get sequence details of selected sequence for editing
        /// Stored procedure GetSequenceData table[2] converted to this method
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="prjId"></param>
        /// <param name="progId"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public async Task<List<SequenceValveDataViewModel>> GetSequenceValveDataMulti(int seqId)
        {
            try
            {

                NewSequence seq = await _mainDBContext.NewSequence.Where(x => x.SeqId == seqId).FirstOrDefaultAsync();
                var seqValveData = from v in _mainDBContext.NewSequenceValveConfig
                                   from s in _mainDBContext.NewSequence
                                   where s.SeqId == v.SeqId && s.SeqId == seqId
                                   orderby s.SeqId, v.HorizGrId, v.IsHorizontal descending
                                   select new
                                   {
                                       s.SeqId,
                                       v.ValveStartTime,
                                       s.SeqNo,
                                       v.Valve,
                                       v.ChannelId,
                                       v.ValveStartDuration,
                                       v.IsFertilizerRelated,
                                       v.IsFlushRelated,
                                       v.ScheduleNo
                                   };

                return await seqValveData.Select(x => new SequenceValveDataViewModel()
                {
                    SeqId = x.SeqId,
                    ValveStartTime = x.ValveStartTime,
                    SeqNo = x.SeqNo,
                    Valve = x.Valve,
                    ChannelId = x.ChannelId,
                    ValveStartDuration = x.ValveStartDuration,
                    IsFertilizerRelated = x.IsFertilizerRelated,
                    IsFlushRelated = x.IsFlushRelated,
                    ScheduleNo = x.ScheduleNo
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get sequence valves and groups to configure valve element
        /// </summary>
        /// <param name="seqType"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <returns>SeqValveGroupViewModel</returns>
        public async Task<SeqValveGroupViewModel> GetSeqValveGroup(string seqType, int networkId, int zoneId)
        {
            try
            {
                SeqValveGroupViewModel seqValveGroupViewModel = new SeqValveGroupViewModel();
                // get valves
                if (seqType == "NRV")
                {
                    var channelList = _mainDBContext.Channel.Where(c => c.EqpTypeId == 4 && c.TypeId == 1 && c.IsEnabled == true)
                    .Select(x => new { x.Rtuid, x.ChannelId, x.ChannelName, x.TagName });
                    if (networkId != 0)
                    {
                        channelList = channelList.Join(
                            _mainDBContext.Rtu,
                            c => c.Rtuid,
                            r => r.Rtuid,
                            (c, r) => new { c, r })
                            .Where(x => x.r.Active == true
                            && x.r.NetworkId == networkId)
                            .Select(x => new { x.c.Rtuid, x.c.ChannelId, x.c.ChannelName, x.c.TagName });
                    }

                    List<KeyValuePairViewModel> SequenceValves = await channelList.Select(x => new KeyValuePairViewModel()
                    {
                        Value = x.ChannelId,
                        Text = x.ChannelName,
                        TagName = x.TagName,
                        IsFertilizerRelated = true,
                        IsFlushRelated = false
                    }).ToListAsync();


                    int MSTFertPumpId = 0;
                    string FilterGrNo = "";
                    foreach (var item in SequenceValves)
                    {
                        List<FertGr> FertGrValve = await GetFertGroup(item.Value, networkId, zoneId);
                        List<FilterGr> flterValve = await GetFilterGroup(item.Value);

                        if (FertGrValve != null)
                        {
                            if (FertGrValve.Count > 0)
                            {
                                if (Convert.IsDBNull(FertGrValve[0].MSTFertPumpId) == false)
                                {
                                    MSTFertPumpId = Convert.ToInt32(FertGrValve[0].MSTFertPumpId.ToString());
                                }
                                else
                                {
                                    MSTFertPumpId = 0;
                                }
                            }
                            else
                            {
                                MSTFertPumpId = 0;
                            }
                        }
                        else
                        {
                            MSTFertPumpId = 0;
                        }


                        if (flterValve != null)
                        {
                            if (flterValve.Count > 0)
                            {
                                FilterGrNo = flterValve[0].FieldValue.ToString();
                            }
                            else
                            {
                                FilterGrNo = "";
                            }
                        }
                        else
                        {
                            FilterGrNo = "";
                        }



                        if (MSTFertPumpId == 0)
                        {
                            item.IsFertilizerRelated = false;
                        }
                        else
                        {

                            List<FertValveGroupSettingsConfig> lstNoOfSettings = _mainDBContext.FertValveGroupSettingsConfig.Where(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == zoneId).ToList();
                            if (lstNoOfSettings.Count >= 15)
                            {
                                item.IsFertilizerRelated = false;
                            }
                            else
                            {
                                //Enabled true 
                                item.IsFertilizerRelated = true;
                            }
                        }
                        if (FilterGrNo == "")
                        {
                            //enabled false
                            item.IsFlushRelated = false;
                        }
                        else
                        {
                            //Enabled true 
                            item.IsFlushRelated = true;
                        }
                    }
                    seqValveGroupViewModel.SequenceValves = SequenceValves;
                }
                else if (seqType == "ZBS")
                {
                    seqValveGroupViewModel.SequenceValves = await _mainDBContext.SubBlock
                        .Select(x => new KeyValuePairViewModel()
                        {
                            Value = x.SubblockId,
                            Text = x.Name,
                            TagName = x.TagName,
                            IsFertilizerRelated = true,
                            IsFlushRelated = false
                        }).ToListAsync();
                }
                // get groups
                seqValveGroupViewModel.SequenceGroups = await _mainDBContext.GroupDetails
                    .Where(x => x.OpGroupTypeId == 1 && x.ZoneId == zoneId)
                    .Select(x => new KeyValuePairGroupViewModel()
                    {
                        Value = x.GrpId,
                        Text = x.GroupName,
                        TagName = x.TagName == null ? "-" : x.TagName,
                        Channels = null
                    }).ToListAsync();

                List<SeqGroupValves> SequenecGroupsValves = new List<SeqGroupValves>();

                //Get Group Valves
                foreach (var item in seqValveGroupViewModel.SequenceGroups)
                {
                    //SeqGroupValves seqGroupValves = new SeqGroupValves();
                    List<int> valveconfigids = await _mainDBContext.ValveGroupConfig.Where(x => x.GroupName == item.Text).Select(x => x.ValveConfigId).ToListAsync();
                    var channelList = await _mainDBContext.Channel.Join(
                             _mainDBContext.ValveGroupElementConfig,
                             c => c.ChannelId,
                             r => r.ChannelId,
                             (c, r) => new { c, r })
                             .Where(x => valveconfigids.Contains(x.r.ValveConfigId))
                             .Select(x => new KeyValuePairViewModel()
                             {
                                 Value = x.c.ChannelId,
                                 Text = x.c.ChannelName,
                                 TagName = x.c.TagName,
                                 IsFertilizerRelated = true,
                                 IsFlushRelated = false
                             }).ToListAsync();


                    int MSTFertPumpId = 0;
                    string FilterGrNo = "";
                    foreach (var itemc in channelList)
                    {
                        List<FertGr> FertGrValve = await GetFertGroup(itemc.Value, networkId, zoneId);
                        List<FilterGr> flterValve = await GetFilterGroup(itemc.Value);

                        if (FertGrValve != null)
                        {
                            if (FertGrValve.Count > 0)
                            {
                                if (Convert.IsDBNull(FertGrValve[0].MSTFertPumpId) == false)
                                {
                                    MSTFertPumpId = Convert.ToInt32(FertGrValve[0].MSTFertPumpId.ToString());
                                }
                                else
                                {
                                    MSTFertPumpId = 0;
                                }
                            }
                            else
                            {
                                MSTFertPumpId = 0;
                            }
                        }
                        else
                        {
                            MSTFertPumpId = 0;
                        }


                        if (flterValve != null)
                        {
                            if (flterValve.Count > 0)
                            {
                                FilterGrNo = flterValve[0].FieldValue.ToString();
                            }
                            else
                            {
                                FilterGrNo = "";
                            }
                        }
                        else
                        {
                            FilterGrNo = "";
                        }



                        if (MSTFertPumpId == 0)
                        {
                            itemc.IsFertilizerRelated = false;
                        }
                        else
                        {

                            List<FertValveGroupSettingsConfig> lstNoOfSettings = _mainDBContext.FertValveGroupSettingsConfig.Where(n => n.MstfertPumpId == MSTFertPumpId && n.ZoneId == zoneId).ToList();
                            if (lstNoOfSettings.Count >= 15)
                            {
                                itemc.IsFertilizerRelated = false;
                            }
                            else
                            {
                                //Enabled true 
                                itemc.IsFertilizerRelated = true;
                            }
                        }
                        if (FilterGrNo == "")
                        {
                            //enabled false
                            itemc.IsFlushRelated = false;
                        }
                        else
                        {
                            //Enabled true 
                            itemc.IsFlushRelated = true;
                        }
                    }
                    item.Channels = channelList;
                    //seqGroupValves.GroupName = item.Text;
                    //seqGroupValves.SequenceValves = channelList;
                    //SequenecGroupsValves.Add(seqGroupValves);
                }
                //seqValveGroupViewModel.SequenecGroupsValves = SequenecGroupsValves;
                return seqValveGroupViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(GetSeqValveGroup)}]{ex}");
                throw ex;
            }
        }

        #region Private Methods
        /// <summary>
        /// Get sequence master config
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="startId"></param>
        /// <param name="mSTSeqId"></param>
        /// <returns></returns>
        private async Task<List<SequenceMasterConfig>> GetSequenceMaster(int seqId, int startId, int mSTSeqId)
        {
            try
            {
                List<SequenceMasterConfig> sequenceMasterConfigs = new List<SequenceMasterConfig>();
                if (mSTSeqId == 0)
                {
                    sequenceMasterConfigs = await _mainDBContext.SequenceMasterConfig.Where(x => x.SeqId == seqId && x.StartId == startId).ToListAsync();
                }
                else
                {
                    sequenceMasterConfigs = await _mainDBContext.SequenceMasterConfig.Where(x => x.SeqId == seqId && x.StartId == startId && x.MstseqId == mSTSeqId).ToListAsync();
                }
                return sequenceMasterConfigs;

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceMasterConfig)}.{nameof(GetSequenceMaster)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Save weekly configuration
        /// </summary>
        /// <param name="weekDays"></param>
        /// <param name="prjId"></param>
        /// <param name="programId"></param>
        /// <param name="seqId"></param>
        /// <returns></returns>
        private async Task<bool> SaveWeeklyConfiguration(List<int> weekDays, int prjId, int programId, int seqId)
        {
            try
            {
                var weeklySchedule = _mainDBContext.SequenceWeeklySchedule
                     .Where(x => x.SeqId == seqId && x.PrgId == programId && x.PrjId == prjId);

                if (weeklySchedule.Count() > 0)
                {
                    _mainDBContext.SequenceWeeklySchedule.RemoveRange(weeklySchedule);
                    await _mainDBContext.SaveChangesAsync();
                }
                List<SequenceWeeklySchedule> seqWeekList = new List<SequenceWeeklySchedule>();
                foreach (var dayofweek in weekDays)
                {
                    SequenceWeeklySchedule s1 = new SequenceWeeklySchedule();
                    s1.WeekDayId = dayofweek;
                    s1.PrjId = prjId;
                    s1.PrgId = programId;
                    s1.SeqId = seqId;
                    seqWeekList.Add(s1);
                    //await _mainDBContext.SequenceWeeklySchedule.AddAsync(s1);
                    //await _mainDBContext.SaveChangesAsync();
                }
                if (seqWeekList.Count > 0)
                {
                    _mainDBContext.AddRange(seqWeekList); // don't remember exaxct syntaxt but this should be faster way
                    await _mainDBContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SaveWeeklyConfiguration)}]{ex}");

                throw ex;
            }
        }

        /// <summary>
        /// Save start time in sequence master config
        /// </summary>
        /// <param name="seqid"></param>
        /// <param name="ProjectId"></param>
        /// <param name="ProgId"></param>
        /// <param name="NetworkId"></param>
        /// <param name="ZoneId"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private async Task<bool> SaveStartTime(int seqid, int ProjectId, int ProgId, int NetworkId, int ZoneId, string startTime)
        {
            try
            {
                //SP GetSequenceMaster is converted to following code
                List<SequenceMasterConfig> sequenceMasterConfigs = await GetSequenceMaster(seqid, 1, 0);
                if (sequenceMasterConfigs.Count == 0)
                {
                    SequenceMasterConfig m = new SequenceMasterConfig();
                    m.SeqId = seqid;
                    m.ProjectId = ProjectId;
                    m.PrgId = ProgId;
                    m.NetworkId = NetworkId;
                    m.ZoneId = ZoneId;
                    m.StartId = 1;
                    m.StartTime = startTime;
                    await _mainDBContext.SequenceMasterConfig.AddAsync(m);
                    await _mainDBContext.SaveChangesAsync();
                }
                else
                {
                    SequenceMasterConfig m = await _mainDBContext.SequenceMasterConfig.FirstOrDefaultAsync(m1 => m1.SeqId == seqid && m1.StartId == 1);
                    if (m != null)
                    {
                        m.StartTime = startTime;
                        await _mainDBContext.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(DummySequence)}.{nameof(SaveStartTime)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Delete sequence looping
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        private async Task<bool> DeleteSequenceLooping(int seqId, int startId)
        {
            if (startId == -1)
            {
                List<SequenceLooping> sequenceLooping = await _mainDBContext.SequenceLooping
                    .Where(x => x.SeqId == seqId).ToListAsync();
                if (sequenceLooping != null)
                {
                    _mainDBContext.SequenceLooping.RemoveRange(sequenceLooping);
                }
            }
            else
            {
                List<SequenceLooping> sequenceLooping = await _mainDBContext.SequenceLooping
                    .Where(x => x.SeqId == seqId && x.StartId == startId).ToListAsync();
                if (sequenceLooping != null)
                {
                    _mainDBContext.SequenceLooping.RemoveRange(sequenceLooping);
                }
            }
            _mainDBContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Delete all existing sequence looping and then create new
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private async Task SaveSequenceInLoopAsync(Sequence sequence)
        {
            try
            {
                ProgramLooping p = null;// await _mainDBContext.ProgramLooping.FirstOrDefaultAsync(p1 => p1.PrgId == sequence.PrgId);
                if (p != null)
                {
                    // delete all sequence looping
                    await DeleteSequenceLooping(Convert.ToInt32(sequence.SeqId), -1);

                    DateTime progstdt = Convert.ToDateTime(p.LoopStartDate);

                    DateTime seqstdate = Convert.ToDateTime(sequence.SeqStartDate);

                    DateTime seqenddate = Convert.ToDateTime(sequence.SeqEndDate);

                    //Get sequence start offset in minutes
                    TimeSpan span = seqstdate.Subtract(progstdt);

                    double seqstoffset = span.TotalMinutes;

                    TimeSpan span1 = seqenddate.Subtract(seqstdate);
                    double seqdays = span1.TotalDays;

                    var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
                    using (var sqlConnection = new SqlConnection(dbConnectionString))
                    {
                        await sqlConnection.OpenAsync();
                        var parameters = new DynamicParameters();
                        parameters.Add("@SeqId", sequence.SeqId);
                        parameters.Add("@SeqOffSet", seqstoffset);
                        parameters.Add("@SeqDays", seqdays);
                        var resultSP = await sqlConnection.QueryMultipleAsync("BulkInsertSequenceInLoop", parameters, null, null, CommandType.StoredProcedure);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceService)}.{nameof(SaveSequenceInLoopAsync)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<FertGr>> GetFertGroup(int channelId, int networkID, int zoneId)
        {
            var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
            using (var sqlConnection = new SqlConnection(dbConnectionString))
            {
                await sqlConnection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@ChannelId", channelId);
                parameters.Add("@NetworkId", networkID);
                parameters.Add("@ZoneId", zoneId);
                var resultSP = await sqlConnection.QueryMultipleAsync("GetFertGroupName", parameters, null, null, CommandType.StoredProcedure);
                var resultIntervals = await resultSP.ReadAsync();
                List<FertGr> list = resultIntervals.Select(x => new FertGr { MSTFertPumpId = x.MSTFertPumpId }).ToList();
                return list;
            }
        }

        public async Task<List<FilterGr>> GetFilterGroup(int channelId)
        {
            var dbConnectionString = DbManager.GetDbConnectionString(DbManager.SiteName, "Main");
            using (var sqlConnection = new SqlConnection(dbConnectionString))
            {
                await sqlConnection.OpenAsync();
                var parameters = new DynamicParameters();
                parameters.Add("@ChannelId", channelId);
                var resultSP = await sqlConnection.QueryMultipleAsync("GetFilterGroupName", parameters, null, null, CommandType.StoredProcedure);
                var resultIntervals = await resultSP.ReadAsync();
                List<FilterGr> list = resultIntervals.Select(x => new FilterGr { FieldName = x.FieldName, FieldValue = x.FieldValue }).ToList();
                return list;

            }
        }
        public async Task<bool> UpdateValveStartTime(SequenceViewModel model, int MSTSeqId, int StartId, int editSeqId, DateTime SeqStDt, DateTime SeqEndDt)
        {
            try
            {
                List<DatesToConfigure> dsSeqDatesToConfigure = await GetDatesToConfigure(model.SeqId);
                if (dsSeqDatesToConfigure != null)
                {
                    bool ProceedUpdate = true;

                    List<SequenceValveConfig> dsAllValvesInOtherStTimes = new List<SequenceValveConfig>();

                    dsAllValvesInOtherStTimes = await GetAllValvesInOtherStTimes(model.SeqId, (int)StartId);

                    //Delete all time stamp in this sequence
                    DeleteAllTimeStamp(editSeqId);


                    List<SequenceValveConfig> dsAllValves = new List<SequenceValveConfig>();
                    dsAllValves = await _mainDBContext.SequenceValveConfig.Where(z => z.SeqId == model.SeqId && z.StartId == StartId).OrderBy(z => z.HorizGrId).OrderBy(z => z.IsHorizontal).ToListAsync();

                    if (dsAllValves != null)
                    {
                        if (dsAllValves.Count > 0)
                        {
                            await UpdateAllValves(dsAllValves, model.SeqMasterStartTime, dsSeqDatesToConfigure, model.PrgId, model.PrjId);

                        }
                    }

                    if (dsAllValvesInOtherStTimes != null)
                    {
                        if (dsAllValvesInOtherStTimes.Count > 0)
                        {
                            await UpdateAllValves(dsAllValvesInOtherStTimes, model.SeqMasterStartTime, dsSeqDatesToConfigure, model.PrgId, model.PrjId);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error occured in UpdateValveStartTime in MaintainSequence.aspx";

                return false;
            }
        }

        public async Task<List<SequenceValveConfig>> GetAllValvesInOtherStTimes(int SeqId, int StartId)
        {
            List<SequenceValveConfig> sequenceValveConfigs = new List<SequenceValveConfig>();
            try
            {
                sequenceValveConfigs = await _mainDBContext.SequenceValveConfig.Where(z => z.SeqId == SeqId && z.StartId != StartId).OrderBy(z => z.HorizGrId).OrderBy(z => z.IsHorizontal).ToListAsync();
                return sequenceValveConfigs;
            }
            catch (Exception ex)
            {

                return sequenceValveConfigs;
            }
        }

        public async Task<bool> UpdateAllValves(List<SequenceValveConfig> tblValves, string NewStartTime, List<DatesToConfigure> SeqDates, int programId, int projectId)
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
                            //Update valve
                            SequenceValveConfig vStep = await _mainDBContext.SequenceValveConfig.FirstOrDefaultAsync(n => n.SeqGrEleId == SeqGrEleId);
                            if (vStep.StartId == 1)
                            {
                                if (vStep != null)
                                {
                                    vStep.ValveStartTime = v1StartTime.Hours.ToString("00") + ":" + v1StartTime.Minutes.ToString("00");
                                    await _mainDBContext.SaveChangesAsync();
                                    await _valveService.AddEventForSequences(Convert.ToInt32(vStep.SeqId), Convert.ToInt32(vStep.ChannelId), Convert.ToInt32(vStep.ScheduleNo), "U", programId, await _zoneTimeService.TimeZoneDateTime(), projectId);
                                }
                            }
                            await _valveService.BulkInsertValveTimeStamp(SeqGrEleId, Convert.ToInt32(vStep.SeqId), startid);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error occured in UpdateAllValves() method in MaintainSequence.aspx";
                return false;
            }
        }

        public async void DeleteAllTimeStamp(int SeqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@DeletedSeqId", SeqId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteAllValveTimeSpan", parameters, null, null, CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw;
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


        public async Task<ErrorMsg> ValidateSeqStartEndDateAsync(Sequence s)
        {
            ErrorMsg errorMsg = new ErrorMsg();
            List<DatesToConfigure> dsSeqDatesToConfigure = await GetDatesToConfigure(s.SeqId);
            ProgramIndividual program = null;// await _mainDBContext.ProgramIndividual.Where(x => x.ProgId == s.PrgId).FirstOrDefaultAsync();
            string errormsg = "";
            try
            {
                if (dsSeqDatesToConfigure == null)
                {
                    errormsg = GlobalConstants.InvalidSeqSTDate;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                //Sequence start date shall not be before programme start date (Error)
                if (Convert.ToDateTime(dsSeqDatesToConfigure[0].date) < program.StartDate)
                {
                    if (GlobalConstants.ProgramLoopStatus == false)
                        errormsg = GlobalConstants.ProgStDtErrMsg.Replace("var1", s.SeqName);
                    else
                        errormsg = GlobalConstants.ProgLpStDtErrMsg.Replace("var1", s.SeqName);
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                //Sequence end date shall not be beyond programme end date 
                if ((Convert.ToDateTime(dsSeqDatesToConfigure[dsSeqDatesToConfigure.Count - 1].date) > program.EndDate))
                {
                    if (GlobalConstants.ProgramLoopStatus == false)
                        errormsg = GlobalConstants.ProgEndDtErrMsg.Replace("var1", s.SeqName);
                    else
                        errormsg = GlobalConstants.ProgLpEndDtErrMsg.Replace("var1", s.SeqName);
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                //Validate if seq start time before program start time
                if (s.SeqStartDate == null)
                {
                    errormsg = GlobalConstants.InvalidSeqSTDate;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                if (s.SeqEndDate == null)
                {
                    errormsg = GlobalConstants.InvalidSeqEndDate;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                //Validate if seq start time before program start time
                if (string.IsNullOrEmpty(program.StartTime) == true)
                {
                    errormsg = GlobalConstants.ProgStTimeNotDef;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                if (string.IsNullOrEmpty(program.EndTime) == true)
                {
                    errormsg = GlobalConstants.ProgEndTimeNotDef;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                string[] pstart = program.StartTime.Split(':');
                string[] pend = program.EndTime.Split(':');

                TimeSpan PStTimespan = new TimeSpan();
                if (pstart.Length == 2)
                    PStTimespan = new TimeSpan(Convert.ToInt32(pstart[0]), Convert.ToInt32(pstart[1]), 0);

                TimeSpan PEndTimespan = new TimeSpan();
                if (pend.Length == 2)
                    PEndTimespan = new TimeSpan(Convert.ToInt32(pend[0]), Convert.ToInt32(pend[1]), 0);

                TimeSpan SeqEndTime = new TimeSpan();
                List<SequenceMasterConfig> mlist = await _mainDBContext.SequenceMasterConfig.Where(m => m.SeqId == s.SeqId).OrderBy(n => n.StartId).ToListAsync();
                foreach (SequenceMasterConfig m in mlist)
                {
                    string[] vstart = m.StartTime.Split(':');
                    TimeSpan VStTimespan = new TimeSpan();
                    if (vstart.Length == 2)
                        VStTimespan = new TimeSpan(Convert.ToInt32(vstart[0]), Convert.ToInt32(vstart[1]), 0);
                    if (Convert.ToDateTime(dsSeqDatesToConfigure[0].date) == program.StartDate)
                    {
                        if (VStTimespan < PStTimespan)
                        {
                            if (GlobalConstants.ProgramLoopStatus == false)
                                errormsg = GlobalConstants.ProgStTimeErrMsg.Replace("var1", s.SeqName);
                            else
                                errormsg = GlobalConstants.ProgLpStTimeErrMsg.Replace("var1", s.SeqName);

                            errorMsg.flag = false;
                            errorMsg.errors.Add(errormsg);
                            return errorMsg;
                        }
                    }

                    if (Convert.ToDateTime(dsSeqDatesToConfigure[dsSeqDatesToConfigure.Count - 1].date) == program.EndDate)
                    {
                        if (VStTimespan > PEndTimespan)
                        {
                            if (GlobalConstants.ProgramLoopStatus == false)
                                errormsg = GlobalConstants.ProgStTimeErrMsg1.Replace("var1", s.SeqName);
                            else
                                errormsg = GlobalConstants.ProgLpStTimeErrMsg1.Replace("var1", s.SeqName);
                            errorMsg.flag = false;
                            errorMsg.errors.Add(errormsg);
                            return errorMsg;
                        }
                    }

                    if (Convert.ToDateTime(dsSeqDatesToConfigure[dsSeqDatesToConfigure.Count - 1].date) == program.EndDate)
                    {
                        var vlist = await _mainDBContext.SequenceValveConfig.Where(v => v.MstseqId == m.MstseqId && v.IsHorizontal == true).Select(v => new { v.ValveStartDuration }).Distinct().ToListAsync();
                        foreach (var v in vlist)
                        {
                            if (v.ValveStartDuration != "") //Added by PA to check if record is empty 17-3-2017
                            {
                                string[] vdur = v.ValveStartDuration.Trim().Split(':');
                                TimeSpan VDuration = new TimeSpan();
                                if (vdur.Length == 2)
                                    VDuration = new TimeSpan(Convert.ToInt32(vdur[0]), Convert.ToInt32(vdur[1]), 0);
                                SeqEndTime = VStTimespan.Add(VDuration);
                            }
                            else
                            {
                                errorMsg.flag = false;
                                errorMsg.errors.Add(errormsg);
                                return errorMsg;
                            }

                        }
                    }

                    if (Convert.ToDateTime(dsSeqDatesToConfigure[dsSeqDatesToConfigure.Count - 1].date) == program.EndDate)
                    {
                        var vlist = await _mainDBContext.SequenceValveConfig.Where(v => v.MstseqId == m.MstseqId && v.IsHorizontal == true).Select(v => new { v.ValveStartTime, v.ValveStartDuration, v.Valve }).Distinct().ToListAsync();
                        foreach (var v in vlist)
                        {
                            if (v.ValveStartTime != "" && v.ValveStartDuration != "" && v.Valve != "")//Added by PA to check if record is empty 17-3-2017
                            {

                            }
                            else
                            {
                                errorMsg.flag = false;
                                errorMsg.errors.Add(errormsg);
                                return errorMsg;
                            }

                        }
                    }
                    if (SeqEndTime > PEndTimespan)
                    {
                        if (GlobalConstants.ProgramLoopStatus == false)
                            errormsg = GlobalConstants.ProgEndTimeErrMsg.Replace("var1", s.SeqName);
                        else
                            errormsg = GlobalConstants.ProgLpEndTimeErrMsg.Replace("var1", s.SeqName);

                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsg);
                        return errorMsg;
                    }
                }
                errorMsg.flag = true;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            catch (Exception ex)
            {
                string ErrorMessage = "Error while validating seq start end date for sequence " + s.SeqId.ToString();
                //_log.Error(ErrorMessage, ex);
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            finally
            {

            }
            //return errormsg;
        }

        private async Task<ErrorMsg> ValidateZoneDayEndTime(Sequence s)
        {
            string errormsg = "";
            string errormsgForRes = "";
            ErrorMsg errorMsg = new ErrorMsg();
            errorMsg.flag = true;
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
                                        errorMsg.flag = false;
                                        errorMsg.errors.Add(errormsgForRes);
                                        return errorMsg;
                                    }

                                    List<SequenceValveConfig> dsAllValves = new List<SequenceValveConfig>();
                                    dsAllValves = await _valveService.GetAllValves(s.SeqId, Convert.ToInt32(mList[cnt].StartId));

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
                                            if (await CheckIfChannelsConfiguredInOtherStTimes(dsAllValesInOtherStTimes, starttime.Hours.ToString("00") + ":" + starttime.Minutes.ToString("00"), stRow, endRow, s.SeqId, dsSeqDatesToConfigure) == true)
                                            {
                                                errormsg = GlobalConstants.ElementTimeErrMsg.Replace("var1", s.SeqName);
                                                errorMsg.flag = false;
                                                errorMsg.errors.Add(errormsg);
                                                return errorMsg;
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", mList[cnt].StartId.ToString()).Replace("var2", s.SeqName.ToString());
                                    errormsgForRes = GlobalConstants.dhs66 + " " + "var1".Replace("var1", mList[cnt].StartId.ToString());
                                    errormsgForRes = errormsgForRes + " " + GlobalConstants.forr + " " + "var2" + GlobalConstants.dhs67.Replace("var2", s.SeqName.ToString());
                                    errorMsg.flag = false;
                                    errorMsg.errors.Add(errormsgForRes);
                                    return errorMsg;
                                }
                            }
                        }
                        else
                        {
                            errormsg = GlobalConstants.NullElementErrMsg.Replace("var1", s.SeqName);
                            errormsgForRes = GlobalConstants.dhs68 + "var1" + GlobalConstants.dhs69.Replace("var1", s.SeqName);
                            errorMsg.flag = false;
                            errorMsg.errors.Add(errormsgForRes);
                            return errorMsg;
                        }
                    }
                    else
                    {
                        errormsg = GlobalConstants.DayEndErrMsg;
                        errormsgForRes = GlobalConstants.dhs70;
                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsgForRes);
                        return errorMsg; ;
                    }
                }
            }
            catch (Exception ex)
            {
                errormsg = "Error while validating zone end time for sequence " + s.SeqId.ToString();
                errormsgForRes = GlobalConstants.dhs72 + " " + s.SeqId.ToString();
                //_log.Error(errormsg, ex);
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsgForRes);
                return errorMsg;
            }
            finally
            {

            }

            return errorMsg;
        }

        public async Task<ErrorMsg> ValidateValveNetworkZoneRelation(Sequence s)
        {
            ErrorMsg errorMsg = new ErrorMsg();
            errorMsg.flag = true;
            string errormsg = "";
            try
            {
                if (s.NetworkId == -1)
                {
                    errormsg = GlobalConstants.InvalidNetwork;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.ZoneId == -1)
                {
                    errormsg = GlobalConstants.InvalidZone;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.SeqStartDate == null)
                {
                    errormsg = GlobalConstants.InvalidSeqSTDate;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.SeqEndDate == null)
                {
                    errormsg = GlobalConstants.InvalidSeqEndDate;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.PrjTypeId == null)
                {
                    errormsg = GlobalConstants.InvalidAppOfSeq;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.OperationTypeId == -1)
                {
                    errormsg = GlobalConstants.InvalidTypeofOP;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
                if (s.BasisOfOp == string.Empty)
                {
                    errormsg = GlobalConstants.InvalidbasisofOP;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }

                SequenceMasterConfig m = await _mainDBContext.SequenceMasterConfig.FirstOrDefaultAsync(n => n.SeqId == s.SeqId);
                if (m != null)
                {
                    var valves = await _mainDBContext.SequenceValveConfig.Where(n => n.SeqId == s.SeqId && n.MstseqId == m.MstseqId).Select(n => new { n.ChannelId, n.Valve }).Distinct().ToListAsync();
                    if (valves != null)
                    {
                        if (valves.Count > 0)
                        {
                            for (int i = 0; i < valves.Count; i++)
                            {

                                if (s.NetworkId != -1)
                                {
                                    Channel ch = await _mainDBContext.Channel.FirstOrDefaultAsync(r => r.ChannelId == Convert.ToInt32(valves[i].ChannelId));

                                    if (ch != null)
                                    {
                                        Rtu rtu = await _mainDBContext.Rtu.FirstOrDefaultAsync(r => r.Rtuid == ch.Rtuid);


                                        if (rtu != null)
                                        {
                                            if (s.NetworkId != 0)
                                            {
                                                if (rtu.NetworkId != s.NetworkId)
                                                {
                                                    Network n = await _mainDBContext.Network.FirstOrDefaultAsync(nr => nr.NetworkId == s.NetworkId);

                                                    if (errormsg == "")
                                                        errormsg = GlobalConstants.ValveNetworkMismatch.Replace("var1", ch.ChannelName.ToString()).Replace("var2", n.Name);
                                                    else
                                                        errormsg += GlobalConstants.ValveNetworkMismatch.Replace("var1", ch.ChannelName).Replace("var2", n.Name);

                                                    errorMsg.errors.Add(errormsg);
                                                }
                                            }
                                        }

                                        if (s.ZoneId != -1)
                                        {
                                            Block bl = await _mainDBContext.Block.FirstOrDefaultAsync(b => b.BlockId == rtu.BlockId);
                                            if (bl != null)
                                            {

                                                if (bl.ZoneId != s.ZoneId)
                                                {
                                                    Zone z = await _mainDBContext.Zone.FirstOrDefaultAsync(zr => zr.ZoneId == s.ZoneId);

                                                    if (errormsg == "")
                                                        errormsg = GlobalConstants.ValveZoneMismatch.Replace("var1", ch.ChannelName.ToString()).Replace("var2", z.Name);
                                                    else
                                                        errormsg += GlobalConstants.ValveZoneMismatch.Replace("var1", ch.ChannelName).Replace("var2", z.Name);
                                                    errorMsg.errors.Add(errormsg);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (errormsg != "")
                                    errormsg += "-";
                            }
                        }
                        else
                        {
                            errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", m.StartId.ToString()).Replace("var2", s.SeqName.ToString());
                            errorMsg.flag = false;
                            errorMsg.errors.Add(errormsg);
                            return errorMsg;
                        }
                    }
                    else
                    {
                        errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", m.StartId.ToString()).Replace("var2", s.SeqName.ToString());
                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsg);
                        return errorMsg;
                    }
                }
                else
                {
                    errormsg = GlobalConstants.NullElementErrMsg.Replace("var1", s.SeqName);
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
            }
            catch (Exception ex)
            {
                errormsg = "Error while validating valve network zone relation for sequence " + s.SeqId.ToString();
                //_log.Error(errormsg, ex);
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            finally
            {

            }
            if (errormsg != "")
            {
                if (errormsg.IndexOf("-") >= 0)
                {
                    errormsg = errormsg.Remove(errormsg.LastIndexOf("-"));
                }
            }

            if (errormsg == "")
            {
                errorMsg.flag = true;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            else
            {
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
        }

        public async Task<ErrorMsg> ValidateValveForMoreThanThirtyTimesAsync(Sequence s)
        {
            string errormsg = "";
            ErrorMsg errorMsg = new ErrorMsg();
            try
            {

                SequenceMasterConfig m = await _mainDBContext.SequenceMasterConfig.FirstOrDefaultAsync(n => n.SeqId == s.SeqId);
                if (m != null)
                {
                    var valves = await _mainDBContext.SequenceValveConfig.Where(n => n.SeqId == s.SeqId && n.MstseqId == m.MstseqId).Select(n => new { n.ChannelId, n.Valve }).Distinct().ToListAsync();
                    if (valves != null)
                    {
                        for (int i = 0; i < valves.Count; i++)
                        {
                            List<int> mstIds = await _mainDBContext.SequenceMasterConfig.Where(x => x.PrgId == s.PrgId).Select(x => x.MstseqId).Distinct().ToListAsync();
                            var ValveNoLst = await _mainDBContext.SequenceValveConfig.Where(x => mstIds.Contains((int)x.MstseqId) && x.ChannelId == valves[i].ChannelId).ToListAsync();
                            if (ValveNoLst != null)
                            {
                                if (ValveNoLst.Count > 0)
                                {
                                    int valvecnt = Convert.ToInt32(ValveNoLst.Count);
                                    if (valvecnt > GlobalConstants.MaxValvesAllowed)
                                    {
                                        if (errormsg == "")
                                            errormsg = GlobalConstants.ValveOpMoreThanThirtyTime.Replace("var1", valves[i].Valve).Replace("var2", GlobalConstants.MaxValvesAllowed.ToString());
                                        else
                                            errormsg += GlobalConstants.ValveOpMoreThanThirtyTime.Replace("var1", valves[i].Valve).Replace("var2", GlobalConstants.MaxValvesAllowed.ToString());
                                        errorMsg.flag = false;
                                        errorMsg.errors.Add(errormsg);
                                        return errorMsg;
                                    }
                                }
                                else
                                {
                                    if (errormsg == "")
                                        errormsg = GlobalConstants.NullValveCnt.Replace("var1", valves[i].Valve);
                                    else
                                        errormsg += GlobalConstants.NullValveCnt.Replace("var1", valves[i].Valve);
                                    errorMsg.flag = false;
                                    errorMsg.errors.Add(errormsg);
                                    return errorMsg;
                                }
                            }
                            else
                            {
                                if (errormsg == "")
                                    errormsg = GlobalConstants.NullValveCnt.Replace("var1", valves[i].Valve);
                                else
                                    errormsg += GlobalConstants.NullValveCnt.Replace("var1", valves[i].Valve);
                                errorMsg.flag = false;
                                errorMsg.errors.Add(errormsg);
                                return errorMsg;
                            }
                            if (errormsg != "")
                                errormsg += "-";
                        }

                    }
                    else
                    {
                        errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", m.StartId.ToString()).Replace("var2", s.SeqName.ToString());
                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsg);
                        return errorMsg;
                    }
                }
                else
                {
                    errormsg = GlobalConstants.NullElementErrMsg.Replace("var1", s.SeqName);
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
            }
            catch (Exception ex)
            {
                errormsg = "Error while validating zone end time for sequence " + s.SeqId.ToString();
                ///_log.Error(errormsg, ex);
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            finally
            {

            }
            if (errormsg != "")
            {
                if (errormsg.IndexOf("-") >= 0)
                {
                    errormsg = errormsg.Remove(errormsg.LastIndexOf("-"));
                }
            }

            if (errormsg == "")
            {
                errorMsg.flag = true;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            else
            {
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
        }

        public async Task<ErrorMsg> ValidateGroupForMoreThanTwentyFiveTimesAsync(Sequence s)
        {
            ErrorMsg errorMsg = new ErrorMsg();
            string errormsg = "";
            try
            {
                SequenceMasterConfig m = await _mainDBContext.SequenceMasterConfig.FirstOrDefaultAsync(n => n.SeqId == s.SeqId);
                if (m != null)
                {
                    var valves = await _mainDBContext.SequenceValveConfig.Where(n => n.SeqId == s.SeqId && n.GroupName != "N/A").Select(n => new { n.GroupName, n.HorizGrId, n.StartId }).Distinct().ToListAsync();
                    if (valves != null)
                    {

                        if (valves.Count > GlobalConstants.MaxGroupsAllowed)
                        {
                            errormsg = GlobalConstants.GroupMoreThanTwentyFiveTime.Replace("var1", s.SeqName).Replace("var3", GlobalConstants.MaxGroupsAllowed.ToString());
                            errorMsg.flag = false;
                            errorMsg.errors.Add(errormsg);
                            return errorMsg;
                        }
                    }
                    else
                    {
                        errormsg = GlobalConstants.NullValveErrMsg.Replace("var1", m.StartId.ToString()).Replace("var2", s.SeqName.ToString());
                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsg);
                        return errorMsg;
                    }
                }
                else
                {
                    errormsg = GlobalConstants.NullElementErrMsg.Replace("var1", s.SeqName);
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
            }
            catch (Exception ex)
            {
                errormsg = "Error while validating zone end time for sequence " + s.SeqId.ToString();
                // _log.Error(errormsg, ex);
                errorMsg.flag = false;
                errorMsg.errors.Add(errormsg);
                return errorMsg;
            }
            finally
            {

            }
            errorMsg.flag = true;
            errorMsg.errors.Add(errormsg);
            return errorMsg;
        }

        public async Task<ErrorMsg> ValidateForFertDurMoreThanValveDur(Sequence s)
        {
            string errormsg = "";
            ErrorMsg errorMsg = new ErrorMsg();
            var vlist = await _mainDBContext.SequenceValveConfig.Where(v => v.SeqId == s.SeqId && v.IsFertilizerRelated == true).Select(v => new { v.Valve, v.ValveStartDuration, v.DurationOfFert }).ToListAsync();

            foreach (var v in vlist)
            {
                string[] vdur = v.ValveStartDuration.Trim().Split(':');

                TimeSpan VDuration = new TimeSpan();
                if (vdur.Length == 2)
                    VDuration = new TimeSpan(Convert.ToInt32(vdur[0]), Convert.ToInt32(vdur[1]), 0);

                if (string.IsNullOrEmpty(v.DurationOfFert.ToString()) == false)
                {
                    int FertDuration = Convert.ToInt32(v.DurationOfFert);
                    if (FertDuration > VDuration.TotalMinutes)
                    {
                        errormsg = GlobalConstants.FertDurErrMsg.Replace("var1", v.Valve).Replace("var2", s.SeqName);
                        errorMsg.flag = false;
                        errorMsg.errors.Add(errormsg);
                        return errorMsg;
                    }
                }
            }
            errorMsg.flag = true;
            errorMsg.errors.Add(errormsg);
            return errorMsg;
        }

        public async Task<List<string>> ValidateForSequenceWeeklySchedule(Sequence s)
        {
            string errormsg = "";
            List<string> warningmsg = new List<string>();

            DateTime SeqStDate = await _zoneTimeService.TimeZoneDateTime();
            DateTime SeqEndDate = await _zoneTimeService.TimeZoneDateTime();
            int noofdaysconf = 0;
            List<int> WeekDays = await _mainDBContext.SequenceWeeklySchedule.Where(s1 => s1.SeqId == s.SeqId && s1.PrjId == s.PrjId && s1.PrgId == s.PrgId).Select(s1 => s1.WeekDayId).ToListAsync();
            try
            {
                if (s.BasisOfOp == "Weekly")
                {
                    for (DateTime dt = Convert.ToDateTime(s.SeqStartDate); dt <= Convert.ToDateTime(s.SeqEndDate); dt = dt.AddDays(1))
                    {
                        if (WeekDays.Contains(Convert.ToInt32(dt.DayOfWeek) + 1))
                        {
                            noofdaysconf += 1;
                        }
                    }
                }
                else
                {
                    int interval = Convert.ToInt32(s.IntervalDays);
                    if (interval == 0)
                        interval = 1;

                    for (DateTime dt = Convert.ToDateTime(s.SeqStartDate); dt <= Convert.ToDateTime(s.SeqEndDate); dt = dt.AddDays(interval + 1))
                    {
                        noofdaysconf += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in getting sequence details in displayhorizontalsequence.aspx page load");
                //_log.Error(ErrorMessage, ex);
            }

            if (noofdaysconf == 0)
            {
                if (s.BasisOfOp == "Weekly")
                    warningmsg.Add(GlobalConstants.SeqWeekdaysWarning.Replace("var1", s.SeqName));
                else
                    warningmsg.Add(GlobalConstants.SeqIntervaldaysWarning.Replace("var1", s.SeqName));
            }
            return warningmsg;
        }
        public async Task<ErrorMsg> ValidateForStarttimeBlankAsync(Sequence s)
        {
            string errormsg = "";
            ErrorMsg errorMsg = new ErrorMsg();
            var vlist = await _mainDBContext.SequenceValveConfig.Where(v => v.SeqId == s.SeqId).Select(v => new { v.ValveStartTime, v.ValveStartDuration }).ToListAsync();

            foreach (var v in vlist)
            {
                if (v.ValveStartTime == "" || v.ValveStartDuration == "")
                {
                    errormsg = GlobalConstants.ValveDurationblank;
                    errorMsg.flag = false;
                    errorMsg.errors.Add(errormsg);
                    return errorMsg;
                }
            }
            errorMsg.flag = true;
            errorMsg.errors.Add(errormsg);
            return errorMsg;
        }

        private async Task<List<SequenceErrDetailsViewModel>> UpdateSequenceValidityAsync(Sequence s, bool isValidSequence, List<string> warningmsg, List<string> errors)
        {
            string errormsg = "";

            s.ValidationState = true;
            s.IsValid = isValidSequence;
            await _mainDBContext.SaveChangesAsync();

            if (isValidSequence == false)
            {
                string[] errmsglist = null;
                await DeleteSequenceErrors(Convert.ToInt32(s.SeqId), true);
                if (errors.Count > 0)
                {
                    errmsglist = errors[0].Split('-');
                    if (errmsglist == null)
                    {
                        errmsglist[0] = errors[0];
                    }

                }

                if (errmsglist != null)
                {
                    for (int i = 0; i < errmsglist.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(errmsglist[i]) == false)
                        {
                            //SequenceErrDetail err = jdc.SequenceErrDetails.FirstOrDefault(e => e.SeqId == s.SeqId && e.PrjId == s.PrjId && e.PrgId == s.PrgId && e.NetworkId == s.NetworkId && e.ZoneId == s.ZoneId && e.ErrorDetail == errmsglist[i] && e.IsError == true);
                            SequenceErrDetails err = await _mainDBContext.SequenceErrDetails.FirstOrDefaultAsync(e => e.SeqId == s.SeqId && e.ErrorDetail == errmsglist[i] && e.IsError == true);

                            if (err == null)
                            {
                                err = new SequenceErrDetails();
                                err.SeqId = s.SeqId;
                                err.PrgId = s.PrgId;
                                err.PrjId = s.PrjId;
                                err.NetworkId = s.NetworkId;
                                err.ZoneId = s.ZoneId;
                                err.ErrorDetail = errmsglist[i];
                                err.IsError = true;
                                await _mainDBContext.SequenceErrDetails.AddAsync(err);
                                await _mainDBContext.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            else
            {
                await DeleteSequenceErrors(Convert.ToInt32(s.SeqId), true);
            }

            if (warningmsg.Count > 0)
            {
                await DeleteSequenceErrors(Convert.ToInt32(s.SeqId), false);

                for (int i = 0; i < warningmsg.Count; i++)
                {
                    //SequenceErrDetail err = jdc.SequenceErrDetails.FirstOrDefault(e => e.SeqId == s.SeqId && e.PrjId == s.PrjId && e.PrgId == s.PrgId && e.NetworkId == s.NetworkId && e.ZoneId == s.ZoneId && e.ErrorDetail == warningmsg[i] && e.IsError == false);
                    SequenceErrDetails err = await _mainDBContext.SequenceErrDetails.FirstOrDefaultAsync(e => e.SeqId == s.SeqId && e.ErrorDetail == warningmsg[i] && e.IsError == false);
                    if (err == null)
                    {
                        err = new SequenceErrDetails();
                        err.SeqId = s.SeqId;
                        err.PrgId = s.PrgId;
                        err.PrjId = s.PrjId;
                        err.NetworkId = s.NetworkId;
                        err.ZoneId = s.ZoneId;
                        err.ErrorDetail = warningmsg[i];
                        err.IsError = false;
                        await _mainDBContext.SequenceErrDetails.AddAsync(err);
                        await _mainDBContext.SaveChangesAsync();
                    }
                }
            }
            else
            {

                await DeleteSequenceErrors(Convert.ToInt32(s.SeqId), false);
            }

            List<SequenceErrDetails> errseq = await _mainDBContext.SequenceErrDetails.Where(e => e.SeqId == s.SeqId).ToListAsync();
            List<SequenceErrDetailsViewModel> errse = _mapper.Map<List<SequenceErrDetailsViewModel>>(errseq);
            return errse;
        }
        #region commented
        protected async Task<bool> CheckIfChannelsConfiguredInOtherStTimes(List<SequenceValveConfig> tblValves, string NewStartTime, int start, int end, int newseqid, List<DatesToConfigure> SeqDates)
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
                                    //HD
                                    //6 May 2015
                                    //Call stored procedure to check if channel already configured
                                    if (stRow > endRow)
                                        endRow = endRow + GlobalConstants.LastTimespanEntry;

                                    List<ChannelConfiguredModel> dsTimeSpanEntries = await _valveService.CheckIfValveConfigured(channelid, stRow, endRow, newseqid);

                                    if (dsTimeSpanEntries != null)
                                    {
                                        if (dsTimeSpanEntries.Count > 0)
                                        {
                                            for (int x = 0; x < SeqDates.Count; x++)
                                            {
                                                var date = Convert.ToDateTime(SeqDates[x].date.ToString());
                                                var dt = dsTimeSpanEntries.Where(x => x.SeqDate == date).ToList();
                                                if (dt != null)
                                                {
                                                    if (dt.Count > 1)
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
            }
            return false;
        }

        #endregion
        public async Task<bool> UpdateEventForSequence(int seqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ValidSeqId", seqId);
                    var result = await sqlConnection.QueryMultipleAsync("UpdateAllValidSequenceEvents", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEvent(int EventId, string status, DateTime TimeZoneDateTime)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@EventId", EventId);
                    parameters.Add("@Status", status);
                    parameters.Add("@TimeZoneDateTime", TimeZoneDateTime);
                    var result = await sqlConnection.QueryMultipleAsync("UpdateEventsForProjectStatus", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AdddEvent(EventViewModel ev)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@objectName", ev.ObjName);
                    parameters.Add("@action", ev.action);
                    parameters.Add("@PrjId", ev.TimeZoneDateTime);
                    parameters.Add("@NetworkId", ev.networkId);
                    parameters.Add("@ObjectIdInDB", ev.objIdinDB);
                    parameters.Add("@TimeZoneDateTime", ev.TimeZoneDateTime);
                    var result = await sqlConnection.QueryMultipleAsync("ExecuteSPEvents", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool ValidateDayStartTime(TimeSpan DayStart, TimeSpan DayEnd, TimeSpan ValveStart, TimeSpan ValveEnd)
        {
            if (ValveStart < DayStart && ValveEnd <= DayStart)
            {
                return true;
            }
            else if (ValveStart >= DayStart && ValveEnd >= DayStart && ValveEnd <= DayEnd)
            {
                return true;
            }
            else if (ValveStart >= DayStart && ValveEnd <= DayEnd)
            {
                return true;
            }
            else if (ValveStart < DayStart && ValveEnd > DayStart)
            {
                return false;
            }
            return false;
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
        #endregion

    }
}