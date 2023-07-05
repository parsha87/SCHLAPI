using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IFilterService
    {
        Task<List<FilterZone>> GetMetaData(string priviledges, string userId);
        Task<List<GroupDetailsViewModel>> GetGroupDetailsByZoneId(int zoneId, string priviledges, string userId);
        Task<FilterValveGroupConfigViewModel> GetGroupDetailsById(int grpId, string priviledges, string userId);
        Task<bool> AddUpdate(FilterValveGroupConfig model, string UserId);
        Task<string> CreateGroup(int zoneId, string priviledges, string userId);

    }

    public class FilterService : IFilterService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private EventDBContext _eventDBContext;
        private IZoneTimeService _zoneTimeService;

        private readonly ILogger<FilterService> _logger;
        public FilterService(ILogger<FilterService> logger,
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

        public async Task<List<FilterZone>> GetMetaData(string priviledges, string userId)
        {
            try
            {
                List<FilterZone> filterZones = new List<FilterZone>();
                List<KeyValueViewModel> rtus = new List<KeyValueViewModel>();
                List<int> networkids = new List<int>();
                if (priviledges == "All")
                {
                    var zones = await _mainDBContext.Zone.Select(x => new ZoneShortViewModel
                    {
                        ZoneId = x.ZoneId,
                        ZoneName = x.Name
                    }).ToListAsync();
                    foreach (var itemZones in zones)
                    {
                        FilterZone moZone = new FilterZone();
                        moZone.ZoneId = itemZones.ZoneId;
                        moZone.ZoneName = itemZones.ZoneName;
                        List<KeyValueViewModel> zoneGroups = new List<KeyValueViewModel>();
                        var szoneGroups = await _mainDBContext.GroupDetails.Where(r => r.ZoneId == itemZones.ZoneId && r.OpGroupTypeId == 2).Select(r => new KeyValueViewModel { Text = r.GroupName, Value = r.GrpId, TagName = "" }).ToArrayAsync();
                        moZone.Groups = szoneGroups.ToList();

                        networkids = await _mainDBContext.Block.Where(x => x.ZoneId == itemZones.ZoneId).Select(x => (int)x.NetworkId).ToListAsync();
                        rtus = await _mainDBContext.Rtu.Where(x => networkids.Contains(x.NetworkId) && x.Active == true).Select(x => new KeyValueViewModel { Text = x.Rtuname, Value = x.Rtuid }).ToListAsync();
                        moZone.Rtus = rtus;
                        filterZones.Add(moZone);
                    }
                }
                else
                {
                    string[] pvResult = priviledges.Split(',');
                    if (pvResult.Contains("1"))
                    {
                        IEnumerable<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();




                        var zoneName = await _mainDBContext.Zone.OrderBy(z => z.ZoneNo).Select(x => new
                        {
                            ZoneId = x.ZoneId,
                            ZoneName = x.Name
                        }).ToListAsync();

                        if (apObj.Where(a => a.AllZones == true).ToList().Count > 0)
                        {
                            zoneName = await _mainDBContext.Zone.OrderBy(z => z.ZoneNo).Select(x => new
                            {
                                ZoneId = x.ZoneId,
                                ZoneName = x.Name
                            }).ToListAsync();
                        }
                        else
                        {
                            List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
                            zoneName = zoneName.Where(s => zoneObj.Contains(s.ZoneId)).ToList();
                        }
                        if (zoneName.Count() > 0)
                        {
                            foreach (var itemZones in zoneName)
                            {
                                FilterZone moZone = new FilterZone();
                                moZone.ZoneId = itemZones.ZoneId;
                                moZone.ZoneName = itemZones.ZoneName;
                                List<KeyValueViewModel> zoneGroups = new List<KeyValueViewModel>();
                                var szoneGroups = await _mainDBContext.GroupDetails.Where(r => r.ZoneId == itemZones.ZoneId && r.OpGroupTypeId == 2).Select(r => new KeyValueViewModel { Text = r.GroupName, Value = r.GrpId, TagName = "" }).ToArrayAsync();
                                moZone.Groups = szoneGroups.ToList();
                                networkids = await _mainDBContext.Block.Where(x => x.ZoneId == itemZones.ZoneId).Select(x => (int)x.NetworkId).ToListAsync();
                                if (apObj.Where(a => a.AllNetworks == true).ToList().Count > 0)
                                {
                                    rtus = await _mainDBContext.Rtu.Where(x => networkids.Contains(x.NetworkId) && x.Active == true).Select(x => new KeyValueViewModel { Text = x.Rtuname, Value = x.Rtuid }).ToListAsync();
                                }
                                else
                                {
                                    List<int> networkObj = apObj.Where(a => a.Network != 0).Select(n => (int)n.Network).ToList();
                                    rtus = await _mainDBContext.Rtu.Where(x => networkObj.Contains(x.NetworkId) && x.Active == true).Select(x => new KeyValueViewModel { Text = x.Rtuname, Value = x.Rtuid }).ToListAsync();
                                }
                                moZone.Rtus = rtus;
                                filterZones.Add(moZone);
                            }
                        }
                    }
                }
                return filterZones;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<GroupDetailsViewModel>> GetGroupDetailsByZoneId(int zoneId, string priviledges, string userId)
        {
            List<GroupDetails> groupDetails = new List<GroupDetails>();
            List<GroupDetailsViewModel> groupDet = new List<GroupDetailsViewModel>();

            try
            {
                groupDetails = await _mainDBContext.GroupDetails.Where(x => x.OpGroupTypeId == 2).ToListAsync();
                if (zoneId != 0)
                {
                    groupDetails = await _mainDBContext.GroupDetails.Where(x => x.OpGroupTypeId == 2 && x.ZoneId == zoneId).ToListAsync();
                }
                else
                {
                    if (priviledges != "All")
                    {
                        string[] pvResult = priviledges.Split(',');
                        if (pvResult.Contains("1"))
                        {
                            IEnumerable<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();
                            List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
                            groupDetails = groupDetails.Where(p => zoneObj.Contains(Convert.ToInt32(p.ZoneId)) && (p.OpGroupTypeId == 2)).ToList();

                        }
                        //groupDet = _mapper.Map<List<GroupDetailsViewModel>>(groupDetails);


                    }
                }
       
                return groupDet;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<FilterValveGroupConfigViewModel> GetGroupDetailsById(int grpId, string priviledges, string userId)
        {
            FilterValveGroupConfigViewModel groupDetails = new FilterValveGroupConfigViewModel();
            try
            {
                FilterValveGroupConfig groupDetail = await _mainDBContext.FilterValveGroupConfig.Include(x => x.FilterValveGroupElementsConfig).Where(x => x.GrpId == grpId).FirstOrDefaultAsync();

                List<FilterValveGroupElementsConfig> ss = await _mainDBContext.FilterValveGroupElementsConfig.Where(x => x.MstfilterGroupId == groupDetail.MstfilterGroupId).ToListAsync();
                //groupDetails.

                List<FilterValveGroupElementsConfigViewModel> sss = _mapper.Map<List<FilterValveGroupElementsConfigViewModel>>(ss);
                foreach (var item in sss)
                {
                    item.ChannelName = await _mainDBContext.Channel.Where(x => x.ChannelId == item.ChannelId).Select(x => x.ChannelName).FirstOrDefaultAsync();

                    item.PressSustainingOpChannelName = await _mainDBContext.Channel.Where(x => x.ChannelId == item.PressSustainingOpNo).Select(x => x.ChannelName).FirstOrDefaultAsync();
                }



                groupDetails = _mapper.Map<FilterValveGroupConfigViewModel>(groupDetail);
                groupDetails.FilterValveGroupElementsConfig = sss;
                return groupDetails;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<bool> AddUpdate(FilterValveGroupConfig model, string UserId)
        {
            FilterValveGroupConfig filterMaster = new FilterValveGroupConfig();
            try
            {
                var projectID = await _mainDBContext.Project.Select(x => x.PrjId).FirstOrDefaultAsync();

                if (model.MstfilterGroupId == 0)
                {
                    //Add MO

                    filterMaster.ProjectId = projectID;
                    filterMaster.NetworkId = 0;
                    filterMaster.ZoneId = model.ZoneId;
                    filterMaster.Rtuid = model.Rtuid;
                    filterMaster.OpGroupTypeId = model.OpGroupTypeId;
                    filterMaster.GroupName = model.GroupName;
                    filterMaster.OperationType = model.OperationType;
                    filterMaster.PauseWhileFlush = model.PauseWhileFlush;
                    filterMaster.OffsetForFilterFlushinMin = model.OffsetForFilterFlushinMin;
                    filterMaster.FlushTimeinMin = model.FlushTimeinMin;
                    filterMaster.DelayBetweenFlushinSec = model.DelayBetweenFlushinSec;
                    filterMaster.StartSustainingBeforeFlush = model.StartSustainingBeforeFlush;
                    filterMaster.GrpId = model.GrpId;
                    filterMaster.TagName = model.TagName;
                    await _mainDBContext.FilterValveGroupConfig.AddAsync(filterMaster);
                    await _mainDBContext.SaveChangesAsync();
                    int MstfilterGroupId = filterMaster.MstfilterGroupId;


                    var rtu = await _mainDBContext.Rtu.FirstAsync(s => s.Rtuid == model.Rtuid);
                    int NId = rtu.NetworkId;
                    GroupDetails objGroupDetail = await _mainDBContext.GroupDetails.FirstAsync(n => n.GrpId == model.GrpId);
                    EventViewModel evt = new EventViewModel();
                    evt.ObjName = "Filter Group";
                    evt.networkId = 0;
                    evt.prjId = projectID;
                    evt.objIdinDB = (int)objGroupDetail.GrpNoinNwzone;
                    evt.action = "U";
                    evt.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();
                    await AddEventForFilter(evt);



                }
                else
                {
                    //Edit
                    FilterValveGroupConfig filterValveGroupConfig = await _mainDBContext.FilterValveGroupConfig.Where(x => x.MstfilterGroupId == model.MstfilterGroupId).FirstOrDefaultAsync();
                    filterValveGroupConfig.Rtuid = model.Rtuid;
                    filterValveGroupConfig.OpGroupTypeId = model.OpGroupTypeId;
                    filterValveGroupConfig.PauseWhileFlush = model.PauseWhileFlush;
                    filterValveGroupConfig.OffsetForFilterFlushinMin = model.OffsetForFilterFlushinMin;
                    filterValveGroupConfig.FlushTimeinMin = model.FlushTimeinMin;
                    filterValveGroupConfig.DelayBetweenFlushinSec = model.DelayBetweenFlushinSec;
                    filterValveGroupConfig.StartSustainingBeforeFlush = model.StartSustainingBeforeFlush;
                    filterValveGroupConfig.TagName = model.TagName;
                    await _mainDBContext.SaveChangesAsync();

                    var rtu = await _mainDBContext.Rtu.FirstAsync(s => s.Rtuid == model.Rtuid);
                    int NId = rtu.NetworkId;
                    GroupDetails objGroupDetail = await _mainDBContext.GroupDetails.FirstAsync(n => n.GrpId == model.GrpId);
                    EventViewModel evt = new EventViewModel();
                    evt.ObjName = "Filter Group";
                    evt.networkId = 0;
                    evt.prjId = projectID;
                    evt.objIdinDB = (int)objGroupDetail.GrpNoinNwzone;
                    evt.action = "U";
                    evt.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();
                    await AddEventForFilter(evt);
                }

                return true;
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in save filter in filter serveice page.");
                return false;
                // _log.Error(ErrorMessage, ex);
            }
        }


        public async Task<string> CreateGroup(int zoneId, string priviledges, string userId)
        {
            try
            {
                string result = "";
                GroupDetails record = new GroupDetails();
                var zoneObj = await _mainDBContext.Zone.Where(z => z.ZoneId == zoneId).Select(s => s.NoOfFilterStations).FirstOrDefaultAsync();
                if (zoneObj != null)
                {
                    var totalGrp = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 2 && g.ZoneId == zoneId).ToListAsync();
                    if (zoneObj.Value > totalGrp.Count())
                    {
                        record.GrpId = 0;
                        record.OpGroupTypeId = 2;

                        record.ZoneId = zoneId;
                        record.NetworkId = 0;
                        await _mainDBContext.GroupDetails.AddAsync(record);
                        await _mainDBContext.SaveChangesAsync();
                        //NetworkId = 0;

                        using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                        {
                            await sqlConnection.OpenAsync();
                            var parameters = new DynamicParameters();
                            parameters.Add("@networkId", 0);
                            parameters.Add("@zoneId", zoneId);
                            parameters.Add("@OpGroupTypeId", record.OpGroupTypeId);
                            parameters.Add("@groupName", dbType: DbType.String, direction: ParameterDirection.ReturnValue);
                            var resultsp = await sqlConnection.QueryMultipleAsync("GetOPGroupName", parameters, null, null, CommandType.StoredProcedure);
                            var resultIntervals = await resultsp.ReadAsync();
                        }
                        // record.GroupName = jas.GetOutputGroupName(0, ZoneId, Convert.ToInt32(record.OpGroupTypeId));
                        return result = GlobalConstants.GroupcreatedsuccessfullyMV;
                    }
                    else if (zoneObj.Value == 0)
                    {
                        return result = GlobalConstants.resultFI;
                    }
                    else
                    {
                        return result = GlobalConstants.resultFI1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<bool> AddEventForFilter(EventViewModel evt)
        {
            try
            {
                var timezone = await _zoneTimeService.TimeZoneDateTime();
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@objectName", evt.ObjName);
                    parameters.Add("@action", evt.action);
                    parameters.Add("@PrjId", evt.prjId);
                    parameters.Add("@NetworkId", evt.networkId);
                    parameters.Add("@ObjectIdInDB", evt.objIdinDB);
                    parameters.Add("@TimeZoneDateTime", timezone);
                    var result = await sqlConnection.QueryMultipleAsync("ExecuteSPEvents", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
