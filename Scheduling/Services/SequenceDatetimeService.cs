using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface ISequenceDatetimeService
    {
        Task<List<DateTime>> GetDatesToConfigure(int seqId);
        Task<List<DateTime>> GetDatesToConfigureInLoop(int seqId, DateTime startDate, DateTime endDate);
        Task<List<DateTime>> GetDatesToConfigureBeforeUpdate(DateTime startDate, DateTime endDate, string basisOfOp, int interval, int isSun, int isMon, int isTue, int isWed, int isThu, int isFri, int isSat);
    }

    public class SequenceDatetimeService : ISequenceDatetimeService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SequenceDatetimeService> _logger;

        public SequenceDatetimeService(ILogger<SequenceDatetimeService> logger,
                MainDBContext uciDBContext,
                IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// get dates to confugure (sp GetSequenceDatesToConfigure)
        /// </summary>
        /// <param name="seqId"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> GetDatesToConfigure(int seqId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", seqId);
                    var result = await sqlConnection.QueryMultipleAsync("GetSequenceDatesToConfigure", parameters, null, null, CommandType.StoredProcedure);
                    var resultDates = await result.ReadAsync();
                    var list = resultDates.Select(x=> new { x.date }).ToList();
                    List<DateTime> listDateTime = new List<DateTime>();
                    foreach (var item in list)
                    {
                        listDateTime.Add(Convert.ToDateTime(item.date));
                    }
                    return listDateTime;
                }                    
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceDatetimeService) }.{ nameof(GetDatesToConfigure) }]{ ex }");
                throw ex;
            }
        }

        /// <summary>
        /// get sequence dates in loop (sp GetSeqDatesInLoop)
        /// </summary>
        /// <param name="seqId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> GetDatesToConfigureInLoop(int seqId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@SeqId", seqId);
                    parameters.Add("@stdate", startDate);
                    parameters.Add("@enddate", endDate);
                    var result = await sqlConnection.QueryMultipleAsync("GetSeqDatesInLoop", parameters, null, null, CommandType.StoredProcedure);
                    var resultDates = await result.ReadAsync();
                    var list = resultDates.Select(x => new { x.date }).ToList();
                    List<DateTime> listDateTime = new List<DateTime>();
                    foreach (var item in list)
                    {
                        listDateTime.Add(Convert.ToDateTime(item.date));
                    }
                    return listDateTime;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceDatetimeService) }.{ nameof(GetDatesToConfigureInLoop) }]{ ex }");
                throw ex;
            }
        }

        /// <summary>
        /// get dates to configure before update
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="basisOfOp"></param>
        /// <param name="interval"></param>
        /// <param name="isSun"></param>
        /// <param name="isMon"></param>
        /// <param name="isTue"></param>
        /// <param name="isWed"></param>
        /// <param name="isThu"></param>
        /// <param name="isFri"></param>
        /// <param name="isSat"></param>
        /// <returns></returns>
        public async Task<List<DateTime>> GetDatesToConfigureBeforeUpdate(DateTime startDate, DateTime endDate, string basisOfOp, int interval, int isSun, int isMon, int isTue, int isWed, int isThu, int isFri, int isSat)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@stdate", startDate);
                    parameters.Add("@enddate", endDate);
                    parameters.Add("@BasisOfOP", basisOfOp);
                    parameters.Add("@IntervalDays", interval);
                    parameters.Add("@IsSun", isSun);
                    parameters.Add("@IsMon", isMon);
                    parameters.Add("@IsTue", isTue);
                    parameters.Add("@IsWed", isWed);
                    parameters.Add("@IsThu", isThu);
                    parameters.Add("@IsFri", isFri);
                    parameters.Add("@IsSat", isSat);
                    var result = await sqlConnection.QueryMultipleAsync("GetDatesToConfigure", parameters, null, null, CommandType.StoredProcedure);
                    var resultDates = await result.ReadAsync();
                    var list = resultDates.Select(x => new { x.date }).ToList();
                    List<DateTime> listDateTime = new List<DateTime>();
                    foreach (var item in list)
                    {
                        listDateTime.Add(Convert.ToDateTime(item.date));
                    }
                    return listDateTime;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(SequenceDatetimeService) }.{ nameof(GetDatesToConfigureBeforeUpdate) }]{ ex }");
                throw ex;
            }

        }

    }
}
