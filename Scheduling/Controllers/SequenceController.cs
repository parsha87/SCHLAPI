using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        private readonly ILogger<SequenceController> _logger;
        private ISequenceService _sequenceService;
        private MainDBContext _mainDBContext;
        private IZoneTimeService _zoneTimeService;
        private IProjectService _projectService;

        public SequenceController(IZoneTimeService zoneTimeService,
            MainDBContext mainDBContext,
            ILogger<SequenceController> logger,
            ISequenceService sequenceService,
            IProjectService projectService)
        {
            _logger = logger;
            _mainDBContext = mainDBContext;
            _sequenceService = sequenceService;
            _zoneTimeService = zoneTimeService;
            _projectService = projectService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="credentials">CredentialsViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SequenceViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                //TODO::Check for program lock is not implemented
                // validate model
                if (!await IsModelValid(model))
                {
                    List<string> errLst = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errLst.Add(error.ErrorMessage);
                        }
                    }
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", errLst, 5));

                }

                // get unused deleted sequence by program id
                int delSeqNo = await _sequenceService.GetUnusedDeletedSeqNoByProgNo(model.PrgId);

                // get sequence max number by program id
                int maxSeqNo = await _sequenceService.GetSeqMaxNoByProgId(model.PrgId);
                int newSeqNo = 0;

                // 
                if (delSeqNo != 0)
                {
                    if (maxSeqNo < delSeqNo)
                        newSeqNo = maxSeqNo;
                    else
                        newSeqNo = delSeqNo;
                }
                else
                    newSeqNo = maxSeqNo;

                model.IsProgrammable = true;
                // default project type is agricultural irrigarion
                model.PrjTypeId = GlobalConstants.ProjectTypeId;
                model.ValidationState = false;
                model.IsValid = false;
                model.SeqNo = newSeqNo;
                model.SeqName = $"Sequence{newSeqNo}";

                SequenceViewModel savedSequence = await _sequenceService.Add(model);

                return Ok(CustomResponse.CreateResponse(true, "Saved", savedSequence, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{ nameof(Post) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while adding sequence.", "", 1));
            }

        }


        /// <summary>
        /// Login
        /// </summary>
        /// <param name="credentials">CredentialsViewModel</param>
        /// <returns></returns>
        [HttpPost("PostSequenceToCopy")]
        public async Task<IActionResult> PostSequenceToCopy([FromBody] SequenceViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                //TODO::Check for program lock is not implemented
                // validate model
                if (!await IsModelValid(model))
                {
                    List<string> errLst = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errLst.Add(error.ErrorMessage);
                        }
                    }
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", errLst, 5));

                }

                // get unused deleted sequence by program id
                int delSeqNo = await _sequenceService.GetUnusedDeletedSeqNoByProgNo(model.PrgId);

                // get sequence max number by program id
                int maxSeqNo = await _sequenceService.GetSeqMaxNoByProgId(model.PrgId);
                int newSeqNo = 0;

                // 
                if (delSeqNo != 0)
                {
                    if (maxSeqNo < delSeqNo)
                        newSeqNo = maxSeqNo;
                    else
                        newSeqNo = delSeqNo;
                }
                else
                    newSeqNo = maxSeqNo;

                model.IsProgrammable = true;
                // default project type is agricultural irrigarion
                model.PrjTypeId = GlobalConstants.ProjectTypeId;
                model.ValidationState = false;
                model.IsValid = false;
                model.SeqNo = newSeqNo;
                model.SeqName = $"Sequence{newSeqNo}";
                model.SeqId = 0;
                SequenceViewModel savedSequence = await _sequenceService.Add(model);

                return Ok(CustomResponse.CreateResponse(true, "Saved", savedSequence, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{ nameof(Post) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while adding sequence.", "", 1));
            }

        }
        /// <summary>
        /// edit program, no valves configured for this sequence
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] SequenceViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                //TODO::Check for program lock is not implemented
                // validate model
                if (!await IsModelValid(model))
                {
                    List<string> errLst = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errLst.Add(error.ErrorMessage);
                        }
                    }
                    return Ok(CustomResponse.CreateResponse(false, "Invalid model.", errLst, 5));
                }
                // return Ok(CustomResponse.CreateResponse(false, "Invalid model.", ModelState, 5));

                SequenceViewModel existingSequence = await _sequenceService.GetSequenceById(model.SeqId);
                if (existingSequence == null)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Sequence not found.", ModelState, 6));
                }

                //TODO: To check later PA
                //var sequenceData = await _sequenceService.GetSequenceValveData(model.SeqId, model.PrjId, model.PrgId, model.NetworkId, model.ZoneId, existingSequence.SeqType);
                //if(sequenceData.Count > 0)
                //{
                //    // valve already configured for this sequence, can not edit 
                //    return Ok(CustomResponse.CreateResponse(false, "Sequence already has valve configured, can not update.", ModelState, 6));
                //}
                model.IsProgrammable = true;
                // default project type is agricultural irrigarion
                model.PrjTypeId = GlobalConstants.ProjectTypeId;
                model.ValidationState = false;
                model.IsValid = false;
                existingSequence = await _sequenceService.Edit(model);

                return Ok(CustomResponse.CreateResponse(true, "Saved", existingSequence, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError("[" + nameof(SequenceController) + "." + nameof(Put) + "]" + ex);
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while editing sequence.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("DeleteSequenceBySeqId")]
        public async Task<ActionResult> DeleteSequenceBySeqId([FromBody] List<int> seqIds)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                string models = await _sequenceService.DeleteSequenceBySeqId(seqIds);


                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteSequenceBySeqIdNew/{seqId}")]
        public async Task<ActionResult> DeleteSequenceBySeqIdNew(int seqId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                string models = await _sequenceService.DeleteSequenceBySeqIdNew(seqId);


                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ResetUploadSeq/")]
        public async Task<ActionResult> ResetUploadSeq()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                string models = await _sequenceService.ResetUploadSeq();


                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(SequenceController)}.{nameof(GetSequenceByProgramId)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteAllSequence")]
        public async Task<ActionResult> DeleteAllSequence(int seqId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                string models = await _sequenceService.DeleteAllSequence();


                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }


        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [HttpGet("GetSeqValData/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSeqValData(int id)
        {
            try
            {
                string result = "Success";
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var sequenceData = await _sequenceService.GetSequenceValveData(id);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, sequenceData, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSeqValData) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        [HttpGet("GetSeqValDataMulti/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSeqValDataMulti(int id)
        {
            try
            {
                string result = "Success";
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var sequenceData = await _sequenceService.GetSequenceValveDataMulti(id);
                return Ok(sequenceData);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSeqValData) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="programId"></param>
        /// <returns></returns>
        [HttpGet("ValidateSequenceByProgramId")]
        public async Task<ActionResult> ValidateSequenceByProgramId(int id)
        {
            try
            {
                string result = "Success";
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<SequenceErrDetailsViewModel> resulta = await _sequenceService.ValidateSequenceById(id);

                return Ok(CustomResponse.CreateResponse(true, string.Empty, resulta, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(ValidateSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetSequenceByProgramId")]
        public async Task<ActionResult> GetSequenceByProgramId(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<SequenceShortViewModel> models = await _sequenceService.GetSequenceByProgramIdMulti(id);
                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetSequenceByProgramIdForMultimaster")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSequenceByProgramIdForMultimaster(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<SequenceShortViewModel> models = await _sequenceService.GetSequenceByProgramId(id);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequences by programId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetRtuWatermeterCounterValue")]
        [AllowAnonymous]
        public async Task<ActionResult> GetRtuWatermeterCounterValue(int id)
        {
            try
            {
                DbManager.SiteName = "JainPort92";

                Network network = await _mainDBContext.Network.Where(x => x.NetworkNo == id).FirstOrDefaultAsync();
                List<StatusDataRtulineDetails> statusdata = new List<StatusDataRtulineDetails>();
                var ss = _mainDBContext.StatusDataRtulineDetails.AsEnumerable();
                statusdata = ss.Where(x => x.NetworkId == network.NetworkId && x.AorDorV!="V" && x.RtutimeStamp > DateTime.Now.AddDays(-1)).ToList();
                return Ok(statusdata);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceByProgramId) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequence by seq Id, get with SequenceErrDetails, SequenceMasterConfig, 
        /// SequenceValveConfig, SequenceWeeklySchedule, PrjType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetSequenceById")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSequenceById(int id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                SequenceViewModel model = await _sequenceService.GetSequenceById(id);
                model.SeqMasterStartTime = model.SequenceMasterConfig.Select(x => x.StartTime).FirstOrDefault().Trim();
                List<int> weekdays = new List<int>();
                foreach (var item in model.SequenceWeeklySchedule)
                {
                    weekdays.Add(item.WeekDayId);
                }
                model.WeekDays = weekdays;
                return Ok(CustomResponse.CreateResponse(true, string.Empty, model, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSequenceById) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get operation types
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOperationTypes")]
        public async Task<ActionResult> GetOperationTypes()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<OperationTypeViewModel> models = await _sequenceService.GetOperationTypes();
                return Ok(CustomResponse.CreateResponse(true, string.Empty, models, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetOperationTypes) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Get sequence valves and groups to configure valve element
        /// </summary>
        /// <param name="seqType"></param>
        /// <param name="networkId"></param>
        /// <param name="zoneId"></param>
        /// <returns>SeqValveGroupViewModel</returns>
        [HttpGet("GetSeqValveGroup")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSeqValveGroup(string seqType, int networkId, int zoneId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //DbManager.SiteName = "JainPort76";
                SeqValveGroupViewModel model = await _sequenceService.GetSeqValveGroup(seqType, networkId, zoneId);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, model, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceController) }.{ nameof(GetSeqValveGroup) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        #region private methods

        /// <summary>
        /// validate sequence model
        /// </summary>
        /// <param name="model">SequenceViewModel</param>
        /// <returns>error string</returns>
        private async Task<bool> IsModelValid(SequenceViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }
            if (!ModelState.IsValid)
            {
                return false;
            }

            DateTime prgStDate;
            DateTime prgEndDate;
            bool programLoopStatus = false;
           
           

            if (model.SeqEndDate < model.SeqStartDate)
            {
                ModelState.AddModelError("Error", "Sequence start date should be before sequence end date.");
            }

            if (model.BasisOfOp == "Weekly")
            {
                if (model.WeekDays.Count == 0)
                {
                    ModelState.AddModelError("Error", "Please select weekdays for which you want to create sequence.");
                }
                int noofdaysconf = 0;
                //Check if sequence will execute at least once for given period
                for (DateTime dt = Convert.ToDateTime(model.SeqStartDate); dt <= Convert.ToDateTime(model.SeqEndDate); dt = dt.AddDays(1))
                {
                    if (model.WeekDays.Contains(Convert.ToInt32(dt.DayOfWeek)))
                    {
                        noofdaysconf += 1;
                    }
                }

                if (noofdaysconf == 0)
                {
                    ModelState.AddModelError("Error", "The configured sequence will not be executed as selected weekdays are not present within sequence date range.");
                }
            }
            else if (model.BasisOfOp == "Interval")
            {
                if (model.SeqStartDate == model.SeqEndDate)
                {
                    if ((int)model.IntervalDays > 0)
                    {
                        ModelState.AddModelError("Error", "Days Interval exceed number of days in sequence.");
                    }
                }

            }

            if (model.ZoneId == 0)
            {
                ModelState.AddModelError("Error", "Please select zone.");
            }

            //check validation for interval time
            if (model.BasisOfOp == "Interval")
            {
                TimeSpan TSpan = Convert.ToDateTime(model.SeqEndDate) - Convert.ToDateTime(model.SeqStartDate);
                if (model.IntervalDays > TSpan.Days)
                {
                    ModelState.AddModelError("Error", GlobalConstants.msq49);
                }
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

        #endregion
    }
}
