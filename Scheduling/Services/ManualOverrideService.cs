using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.EventEntities;
using Scheduling.Helpers;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IManualOverrideService
    {
        Task<MOMetaDataViewModel> GetMOMetaData(string priviledges, string userId);
        Task<bool> AddUpdate(ManualOverrideMasterViewModel model, string UserId);
        Task<List<MOGetDataModel>> GetMo(string priviledges, string UserId);
        Task<ManualOverrideMasterViewModel> GetMoById(int id, string priviledges, string userId);
        Task<List<ElementNo>> GetOutputValves(int networkId, int zoneId, int blockId, string overridefor, string priviledges, string userId);
        Task<bool> DeleteSequenceBySeqId(List<int> moids);
        Task<List<ValveScheduleForMO>> GetValveScheduleForElements(int moId,string elem, string overridefor);
    }
    public class ManualOverrideService : IManualOverrideService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private EventDBContext _eventDBContext;
        private IZoneTimeService _zoneTimeService;

        private readonly ILogger<ManualOverrideService> _logger;
        public ManualOverrideService(ILogger<ManualOverrideService> logger,
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

        public async Task<MOMetaDataViewModel> GetMOMetaData(string priviledges, string userId)
        {
            MOMetaDataViewModel model = new MOMetaDataViewModel();
            try
            {
                List<MOTypeViewModel> Motypes = new List<MOTypeViewModel>();
                List<Motype> motypes = await _eventDBContext.Motype.ToListAsync();
                Motypes = _mapper.Map<List<MOTypeViewModel>>(motypes);

                List<ActionTypesViewModel> ActionTypes = new List<ActionTypesViewModel>();
                List<ActionTypes> actionTypes = await _eventDBContext.ActionTypes.ToListAsync();
                ActionTypes = _mapper.Map<List<ActionTypesViewModel>>(actionTypes);

                List<AlarmLevelsViewModel> AlarmLevels = new List<AlarmLevelsViewModel>();
                List<AlarmLevels> alarmlevels = await _mainDBContext.AlarmLevels.ToListAsync();
                AlarmLevels = _mapper.Map<List<AlarmLevelsViewModel>>(alarmlevels);

                List<RuleElementsMetadataViewModel> ElementToOverride = new List<RuleElementsMetadataViewModel>();
                List<RuleElementsMetadata> ruleElementsMetadatas = await _eventDBContext.RuleElementsMetadata.Where(x => x.IsActiveForMo == true).OrderBy(x => x.ElementName).ToListAsync();
                ElementToOverride = _mapper.Map<List<RuleElementsMetadataViewModel>>(ruleElementsMetadatas);


                //Networks - Zones
                List<MoNetwork> monetworks = new List<MoNetwork>();
                List<Network> lstnetwork = await _mainDBContext.Network.ToListAsync();
                foreach (var item in lstnetwork)
                {
                    MoNetwork moNetwork = new MoNetwork();
                    moNetwork.NetworkId = item.NetworkId;
                    moNetwork.NetworkName = item.Name;
                    moNetwork.NetworkNo = (int)item.NetworkNo;

                    if (priviledges == "All")
                    {
                        //Get zones in network
                        List<int> zoneids = _mainDBContext.ZoneInNetwork.Where(b => b.NetworkId == item.NetworkId).Select(s => (int)s.ZoneId).ToList();
                        if (zoneids.Count == 0)
                            zoneids = _mainDBContext.Block.Where(b => b.NetworkId == item.NetworkId).Select(s => (int)s.ZoneId).ToList();
                        List<MoZone> ZoneLst = await _mainDBContext.Zone.Where(z => zoneids.Contains(z.ZoneId)).Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();
                        foreach (var itemZones in ZoneLst)
                        {
                            List<KeyValueViewModel> zoneBlock = new List<KeyValueViewModel>();
                            zoneBlock = await _mainDBContext.Block.Where(z => z.ZoneId == itemZones.ZoneId).Select(x => new KeyValueViewModel
                            {
                                Value = x.BlockId,
                                Text = x.Name
                            }).ToListAsync();
                            itemZones.Blocks = zoneBlock;
                        }
                        moNetwork.Zones = ZoneLst;
                    }
                    else
                    {
                        string[] pvResult = priviledges.ToString().Split(',');
                        if (pvResult.Contains("2"))
                        {
                            List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();
                            List<int> zoneids = await _mainDBContext.ZoneInNetwork.Where(b => b.NetworkId == item.NetworkId).Select(s => (int)s.ZoneId).ToListAsync();
                            if (zoneids.Count == 0)
                                zoneids = await _mainDBContext.Block.Where(b => b.NetworkId == item.NetworkId).Select(s => (int)s.ZoneId).ToListAsync();

                            List<MoZone> ZoneLst = await _mainDBContext.Zone.Where(z => zoneids.Contains(z.ZoneId)).Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();
                            if (apObj.Where(a => a.AllZones == true).ToList().Count > 0)
                            {
                                ZoneLst = await _mainDBContext.Zone.Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();
                            }
                            else
                            {
                                List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
                                ZoneLst = ZoneLst.Where(s => zoneObj.Contains(s.ZoneId)).ToList();
                            }
                            foreach (var itemZones in ZoneLst)
                            {
                                List<KeyValueViewModel> zoneBlock = new List<KeyValueViewModel>();
                                zoneBlock = await _mainDBContext.Block.Where(z => z.ZoneId == itemZones.ZoneId).Select(x => new KeyValueViewModel
                                {
                                    Value = x.BlockId,
                                    Text = x.Name
                                }).ToListAsync();
                                itemZones.Blocks = zoneBlock;
                            }
                            moNetwork.Zones = ZoneLst;
                        }
                        else
                        {
                            //Unauthorized
                        }
                    }

                    monetworks.Add(moNetwork);
                }



                List<MoZone> Zones = new List<MoZone>();
                var zones = await _mainDBContext.Zone.Select(x => new ZoneShortViewModel
                {
                    ZoneId = x.ZoneId,
                    ZoneName = x.Name
                }).ToListAsync();
                foreach (var itemZones in zones)
                {
                    MoZone moZone = new MoZone();
                    moZone.ZoneId = itemZones.ZoneId;
                    moZone.ZoneName = itemZones.ZoneName;
                    List<KeyValueViewModel> zoneBlock = new List<KeyValueViewModel>();
                    zoneBlock = await _mainDBContext.Block.Where(z => z.ZoneId == itemZones.ZoneId).Select(x => new KeyValueViewModel
                    {
                        Value = x.BlockId,
                        Text = x.Name
                    }).ToListAsync();
                    moZone.Blocks = zoneBlock;
                    Zones.Add(moZone);
                }


                List<KeyValueViewModel> Blocks = new List<KeyValueViewModel>();
                Blocks = await _mainDBContext.Block.Select(x => new KeyValueViewModel
                {
                    Value = x.BlockId,
                    Text = x.Name
                }).ToListAsync();


                model.Motypes = Motypes;
                model.AlarmLevels = AlarmLevels;
                model.ElementToOverride = ElementToOverride;
                model.ActionTypes = ActionTypes;
                model.Networks = monetworks;
                model.Zones = Zones;
                model.Blocks = Blocks;

                return model;
            }
            catch (Exception ex)
            {

                return model;
            }
        }

        public async Task<bool> AddUpdate(ManualOverrideMasterViewModel model, string UserId)
        {
            try
            {
                var projectID = await _mainDBContext.Project.Select(x => x.PrjId).FirstOrDefaultAsync();
                string overridefor = await CheckUserAccessArea(model, UserId);

                if (overridefor != "")
                {
                    if (model.Moid == 0)
                    {
                        //Add MO
                        ManualOverrideMaster MOMaster = new ManualOverrideMaster();
                        using (var context = new EventDBContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    if (overridefor == "Op")
                                        overridefor = "DO";
                                    else if (overridefor == "G")
                                        overridefor = "Valve Group";
                                    else if (overridefor == "Sq")
                                        overridefor = "Sequence";
                                    else if (overridefor == "Rule")
                                        overridefor = "Rules";
                                    else if (overridefor == "FertGr")
                                        overridefor = "Fert Group";
                                    else if (overridefor == "FiltGr")
                                        overridefor = "Filter Group";
                                    else if (overridefor == "MValve")
                                        overridefor = "Master Valve";
                                    else if (overridefor == "MPump")
                                        overridefor = "Pump Station";
                                    int overrideid = await _eventDBContext.EventObjectTypes.Where(x => x.ObjectName == overridefor).Select(x => x.EventObjTypeId).FirstOrDefaultAsync();
                                    MOMaster.Motypeid = model.Motypeid;
                                    MOMaster.UserName = UserId;
                                    MOMaster.MocreatedDate = await _zoneTimeService.TimeZoneDateTime();
                                    MOMaster.ActionTypeId = model.ActionTypeId;
                                    MOMaster.AlarmLevel = model.AlarmLevel;
                                    MOMaster.OverrideForId = overrideid;
                                    MOMaster.NetworkId = model.NetworkId;
                                    MOMaster.ZoneId = model.ZoneId;
                                    MOMaster.BlockId = model.BlockId;
                                    MOMaster.MostartDateTime = model.MostartDateTime;
                                    MOMaster.MoendDateTime = model.MoendDateTime;
                                    MOMaster.TagName = model.TagName;
                                    MOMaster.IsDeleted = false;
                                    MOMaster.MasterValveOpEnabled = model.MasterValveOpEnabled;
                                    await _eventDBContext.ManualOverrideMaster.AddAsync(MOMaster);
                                    await _eventDBContext.SaveChangesAsync();
                                    int Moid = MOMaster.Moid;

                                    MOMaster.Moname = "MO" + Moid.ToString();
                                    await _eventDBContext.SaveChangesAsync();

                                    if (MOMaster.Moid != 0)
                                    {
                                        if (model.Elements.Count > 0)
                                        {
                                            foreach (var item in model.Elements)
                                            {
                                                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                                                {
                                                    await sqlConnection.OpenAsync();
                                                    var parameters = new DynamicParameters();
                                                    parameters.Add("@MOId", MOMaster.Moid);
                                                    parameters.Add("@ElementType", overridefor);
                                                    parameters.Add("@ObjectId", item.Id);
                                                    var result = await sqlConnection.QueryMultipleAsync("AddManualOverrideElements", parameters, null, null, CommandType.StoredProcedure);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                                            {
                                                await sqlConnection.OpenAsync();
                                                var parameters = new DynamicParameters();
                                                parameters.Add("@MOId", Moid);
                                                if (model.NetworkId != 0 && model.ZoneId == 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                                {
                                                    //Pause the entire network
                                                    parameters.Add("@ElementType", "Network");
                                                    parameters.Add("@ObjectId", model.NetworkId);
                                                }
                                                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                                {
                                                    //Pause the entire zone
                                                    parameters.Add("@ElementType", "Zone");
                                                    parameters.Add("@ObjectId", model.ZoneId);
                                                }
                                                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                                {
                                                    //Pause the entire block
                                                    parameters.Add("@ElementType", "Block");
                                                    parameters.Add("@ObjectId", model.BlockId);
                                                }
                                                else if (model.NetworkId != 0 && model.ZoneId == 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                                {
                                                    parameters.Add("@ElementType", "Block");
                                                    parameters.Add("@ObjectId", model.BlockId);
                                                }
                                                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                                {
                                                    parameters.Add("@ElementType", "Block");
                                                    parameters.Add("@ObjectId", model.BlockId);
                                                }
                                                var result = await sqlConnection.QueryMultipleAsync("AddManualOverrideElements", parameters, null, null, CommandType.StoredProcedure);
                                            }
                                        }


                                        //cn.Close();
                                        #region Add Event
                                        EventViewModel ev = new EventViewModel();
                                        ev.networkId = 0;
                                        ev.objIdinDB = MOMaster.Moid;
                                        ev.ObjName = "MO";
                                        ev.action = "A";
                                        ev.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();

                                        ev.prjId = projectID;
                                        await AddEventForMO(ev);
                                        #endregion
                                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "MSG", "alert('" + GlobalConstants.ManualOverridecreatedsuccessfully + "'); RedirectToViewPage();", true);
                                    }
                                    else
                                    {
                                        await dbContextTransaction.RollbackAsync();
                                        //cn.Close();
                                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "MSG", "alert('" + GlobalConstants.MOerror17 + "'); ", true);
                                    }

                                    await dbContextTransaction.CommitAsync();
                                }
                                catch (Exception ex)
                                {
                                    //_log.Error("error in transaction while saving Manual Override.." + ex.ToString());
                                    await dbContextTransaction.RollbackAsync();
                                    //cn.Close();
                                }
                            }
                        }

                    }
                    else
                    {
                        ManualOverrideMaster MOMaster = await _eventDBContext.ManualOverrideMaster.Where(x => x.Moid == model.Moid).FirstOrDefaultAsync();
                        using (var context = new EventDBContext())
                        {
                            using (var dbContextTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    //update mo moster

                                    if (overridefor == "Op")
                                        overridefor = "DO";
                                    else if (overridefor == "G")
                                        overridefor = "Valve Group";
                                    else if (overridefor == "Sq")
                                        overridefor = "Sequence";
                                    else if (overridefor == "Rule")
                                        overridefor = "Rules";
                                    else if (overridefor == "FertGr")
                                        overridefor = "Fert Group";
                                    else if (overridefor == "FiltGr")
                                        overridefor = "Filter Group";
                                    else if (overridefor == "MValve")
                                        overridefor = "Master Valve";
                                    else if (overridefor == "MPump")
                                        overridefor = "Pump Station";
                                    int overrideid = await _eventDBContext.EventObjectTypes.Where(x => x.ObjectName == overridefor).Select(x => x.EventObjTypeId).FirstOrDefaultAsync();
                                    MOMaster.Motypeid = model.Motypeid;
                                    MOMaster.UserName = UserId;
                                    MOMaster.MocreatedDate = await _zoneTimeService.TimeZoneDateTime();
                                    MOMaster.ActionTypeId = model.ActionTypeId;
                                    MOMaster.AlarmLevel = model.AlarmLevel;
                                    MOMaster.OverrideForId = overrideid;
                                    MOMaster.NetworkId = model.NetworkId;
                                    MOMaster.ZoneId = model.ZoneId;
                                    MOMaster.BlockId = model.BlockId;
                                    MOMaster.MostartDateTime = model.MostartDateTime;
                                    MOMaster.MoendDateTime = model.MoendDateTime;
                                    MOMaster.TagName = model.TagName;
                                    MOMaster.Status = "N";
                                    MOMaster.ExecutionDatetime = null;
                                    MOMaster.MasterValveOpEnabled = model.MasterValveOpEnabled;
                                    await _eventDBContext.SaveChangesAsync();

                                    //delete mo elements
                                    await DeleteMOElements(model.Moid);
                                    //add mo elements
                                    if (model.Elements.Count > 0)
                                    {
                                        foreach (var item in model.Elements)
                                        {
                                            using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                                            {
                                                await sqlConnection.OpenAsync();
                                                var parameters = new DynamicParameters();
                                                parameters.Add("@MOId", MOMaster.Moid);
                                                parameters.Add("@ElementType", overridefor);
                                                parameters.Add("@ObjectId", item.Id);
                                                var result = await sqlConnection.QueryMultipleAsync("AddManualOverrideElements", parameters, null, null, CommandType.StoredProcedure);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                                        {
                                            await sqlConnection.OpenAsync();
                                            var parameters = new DynamicParameters();
                                            parameters.Add("@MOId", model.Moid);
                                            if (model.NetworkId != 0 && model.ZoneId == 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                            {
                                                //Pause the entire network
                                                parameters.Add("@ElementType", "Network");
                                                parameters.Add("@ObjectId", model.NetworkId);
                                            }
                                            else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                            {
                                                //Pause the entire zone
                                                parameters.Add("@ElementType", "Zone");
                                                parameters.Add("@ObjectId", model.ZoneId);
                                            }
                                            else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                            {
                                                //Pause the entire block
                                                parameters.Add("@ElementType", "Block");
                                                parameters.Add("@ObjectId", model.BlockId);
                                            }
                                            else if (model.NetworkId != 0 && model.ZoneId == 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                            {
                                                parameters.Add("@ElementType", "Block");
                                                parameters.Add("@ObjectId", model.BlockId);
                                            }
                                            else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                                            {
                                                parameters.Add("@ElementType", "Block");
                                                parameters.Add("@ObjectId", model.BlockId);
                                            }
                                            var result = await sqlConnection.QueryMultipleAsync("AddManualOverrideElements", parameters, null, null, CommandType.StoredProcedure);
                                        }
                                    }                                    //update event                              
                                    #region Add Event
                                    EventViewModel ev = new EventViewModel();
                                    ev.networkId = 0;
                                    ev.objIdinDB = MOMaster.Moid;
                                    ev.ObjName = "MO";
                                    ev.action = "U";
                                    ev.prjId = projectID;
                                    ev.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();
                                    await AddEventForMO(ev);
                                    #endregion
                                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "MSG", "alert('" + GlobalConstants.ManualOverridecreatedsuccessfully + "');RedirectToViewPage(); ", true);
                                    await dbContextTransaction.CommitAsync();
                                }
                                catch (Exception ex)
                                {
                                    //_log.Error("error in transaction while saving Manual Override.." + ex.ToString());
                                    await dbContextTransaction.RollbackAsync();
                                }

                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in save manual override in managemanualoverride.aspx page.");
                return false;
                // _log.Error(ErrorMessage, ex);
            }
        }
        public async Task<bool> DeleteMOElements(int MOId)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MOId", MOId);
                    var result = await sqlConnection.QueryMultipleAsync("DeleteManualOverrideElements", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in delete mo elements in managemanualoverride.aspx page.");
                //_log.Error(ErrorMessage, ex);
                return false;
            }
        }

        public async Task<bool> AddEventForMO(EventViewModel evt)
        {
            try
            {
                var timezone = await _zoneTimeService.TimeZoneDateTime();
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@MOId", evt.objIdinDB);
                    parameters.Add("@action", evt.action);
                    parameters.Add("@TimeZoneDateTime", timezone);
                    var result = await sqlConnection.QueryMultipleAsync("AddMOEvents", parameters, null, null, CommandType.StoredProcedure);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task<string> CheckUserAccessArea(ManualOverrideMasterViewModel model, string UserId)
        {
            List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == UserId).ToListAsync();

            string overridefor = "";
            if (model.OverrideForId != 0)
            {
                overridefor = model.OverrideForText;
            }
            else
            {
                if (model.NetworkId != 0)
                {
                    overridefor = "Network";
                }
                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                {
                    overridefor = "Zone";
                }
                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId == 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                {
                    overridefor = "Zone";
                }
                else if (model.NetworkId != 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                {
                    overridefor = "Block";
                }
                else if (model.NetworkId != 0 && model.ZoneId == 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                {
                    overridefor = "Block";
                }
                else if (model.NetworkId == 0 && model.ZoneId != 0 && model.BlockId != 0 && model.OverrideForId == 0 && model.Elements.Count == 0)
                {
                    overridefor = "Block";
                }
            }

            //Commented below code as user can have access to zone to create MO for blocks under that zone
            //HD
            //5 Feb 2016
            if (apObj.Where(a => a.SubBlock != 0).ToList().Count > 0 || apObj.Where(a => a.Block != 0).ToList().Count > 0)
            {
                if (overridefor != "Block" && overridefor != "RTU" && overridefor != "Op")
                {
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "Msg", "alert('" + GlobalConstants.MOAccessRightsError1 + "');", true);
                    return "";
                }
            }
            return overridefor;
        }

        public async Task<List<MOGetDataModel>> GetMo(string priviledges, string UserId)
        {
            List<MOGetDataModel> modata = new List<MOGetDataModel>();
            try
            {
                string nwstring = "";
                List<NetworkViewModel> NetworkName = await _mainDBContext.Network.Select(r => new NetworkViewModel { NetworkId = r.NetworkId }).ToListAsync();
                List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == UserId).ToListAsync();
                if (priviledges == "All")
                {
                    NetworkName = await _mainDBContext.Network.OrderBy(a => a.NetworkNo).Select(r => new NetworkViewModel { NetworkId = r.NetworkId }).ToListAsync();
                }
                else
                {
                    List<int> nwObj = apObj.Where(a => a.Network != 0).Select(n => (int)n.Network).ToList();
                    NetworkName = NetworkName.Where(s => nwObj.Contains(s.NetworkId)).Select(r => new NetworkViewModel { NetworkId = r.NetworkId }).ToList();
                }
                int counter = 0;
                foreach (var item in NetworkName)
                {
                    if (counter == 0)
                        nwstring += item.NetworkId;
                    else
                        nwstring += "," + item.NetworkId;
                    counter++;
                }

                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@nwstring", nwstring);
                    var result = await sqlConnection.QueryMultipleAsync("GetManualOverrideMaster", parameters, null, null, CommandType.StoredProcedure);
                    modata = result.Read<MOGetDataModel>().ToList();
                }

                //foreach (var item in modata)
                //{
                //    List<Elements> attachedElements = await _eventDBContext.ManualOverrideElements.Where(x => x.Moid == item.MOId).Select(x => new Elements { ElementName = "", ElementNo = (int)x.ObjectId }).ToListAsync();
                //    manualOverrideMasterViewModels.Elements = attachedElements;
                //}
                return modata;
            }
            catch (Exception ex)
            {
                return modata;
            }



        }

        public async Task<ManualOverrideMasterViewModel> GetMoById(int id, string priviledges, string userId)
        {
            try
            {
                ManualOverrideMasterViewModel manualOverrideMasterViewModels = new ManualOverrideMasterViewModel();
                ManualOverrideMaster model = await _eventDBContext.ManualOverrideMaster.Where(x => x.Moid == id).FirstOrDefaultAsync();
                manualOverrideMasterViewModels = _mapper.Map<ManualOverrideMasterViewModel>(model);
                List<int> attachedElements = await _eventDBContext.ManualOverrideElements.Where(x => x.Moid == id).Select(x => (int)x.ObjectId).ToListAsync();
                if (manualOverrideMasterViewModels.OverrideForId == 6)
                    manualOverrideMasterViewModels.OverrideForText = "Op";
                if (manualOverrideMasterViewModels.OverrideForId == 14)
                    manualOverrideMasterViewModels.OverrideForText = "G";
                if (manualOverrideMasterViewModels.OverrideForId == 5)
                    manualOverrideMasterViewModels.OverrideForText = "RTU";
                if (manualOverrideMasterViewModels.OverrideForId == 12)
                    manualOverrideMasterViewModels.OverrideForText = "Sq";
                if (manualOverrideMasterViewModels.OverrideForId == 13)
                    manualOverrideMasterViewModels.OverrideForText = "FertGr";
                if (manualOverrideMasterViewModels.OverrideForId == 7)
                    manualOverrideMasterViewModels.OverrideForText = "FiltGr";
                if (manualOverrideMasterViewModels.OverrideForId == 15)
                    manualOverrideMasterViewModels.OverrideForText = "MValve";
                if (manualOverrideMasterViewModels.OverrideForId == 16)
                    manualOverrideMasterViewModels.OverrideForText = "MPump";
                if (manualOverrideMasterViewModels.OverrideForId == 18)
                {
                    manualOverrideMasterViewModels.OverrideForText = "MPump";
                }


                List<ElementNo> mastereleNo = await BindElementNo((int)manualOverrideMasterViewModels.NetworkId, (int)manualOverrideMasterViewModels.ZoneId, (int)manualOverrideMasterViewModels.BlockId, manualOverrideMasterViewModels.OverrideForText, priviledges, userId);
                manualOverrideMasterViewModels.Elements = mastereleNo.Where(x => attachedElements.Contains(x.Id)).ToList();


                return manualOverrideMasterViewModels;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteSequenceBySeqId(List<int> moids)
        {
            try
            {
                var projectID = await _mainDBContext.Project.Select(x => x.PrjId).FirstOrDefaultAsync();

                foreach (var moid in moids)
                {
                    using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Events")))
                    {
                        await sqlConnection.OpenAsync();
                        var parameters = new DynamicParameters();
                        parameters.Add("@MOId", moid);
                        parameters.Add("@ErrorCode", 0);
                        var result = await sqlConnection.QueryMultipleAsync("DeleteManualOverride", parameters, null, null, CommandType.StoredProcedure);
                        int ErrorCode = parameters.Get<int>("@ErrorCode");
                    }
                    #region Add Event
                    EventViewModel ev = new EventViewModel();
                    ev.networkId = 0;
                    ev.objIdinDB = moid;
                    ev.ObjName = "MO";
                    ev.action = "D";
                    ev.prjId = projectID;
                    ev.TimeZoneDateTime = await _zoneTimeService.TimeZoneDateTime();
                    await AddEventForMO(ev);
                    #endregion
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ElementNo>> GetOutputValves(int networkId, int zoneId, int blockId, string overridefor, string priviledges, string userId)
        {
            try
            {
                List<ElementNo> elementNos = new List<ElementNo>();

                elementNos = await BindElementNo(networkId, zoneId, blockId, overridefor, priviledges, userId);
                //GenerateConfirmationText();
                return elementNos;
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in get output valves in managemanualoverride.aspx page.");
                //_log.Error(ErrorMessage, ex);
                return null;
            }
        }

        public async Task<List<ElementNo>> BindElementNo(int NetworkId, int ZoneId, int BlockId, string overrideFor, string priviledges, string userId)
        {
            List<ElementNo> dt = new List<ElementNo>();
            try
            {
                if (overrideFor != "Sq" && overrideFor != "Rule")
                {
                    int EventObjtypeid = 0;
                    if (overrideFor == "Op")
                        EventObjtypeid = 6;
                    else if (overrideFor == "G")
                        EventObjtypeid = 14;
                    else if (overrideFor == "RTU")
                        EventObjtypeid = 5;
                    //HD
                    //16 Nov 2015
                    //New targets in MO
                    else if (overrideFor == "FertGr")
                        EventObjtypeid = 13;
                    else if (overrideFor == "FiltGr")
                        EventObjtypeid = 7;
                    else if (overrideFor == "MValve")
                        EventObjtypeid = 15;
                    else if (overrideFor == "MPump")
                        EventObjtypeid = 16;// By soham 20.06.2016

                    dt = await GetChannelNamesForMO(NetworkId, ZoneId, BlockId, 4, 1, EventObjtypeid);
                    //check admin privileges here
                    string[] pvResult = priviledges.Split(',');
                    if (pvResult.Contains("2") && priviledges != "All")
                    {
                        List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();
                        var subblockList = apObj.Where(a => a.SubBlock != 0).Select(s => s.SubBlock).ToList();
                        var blockList = apObj.Where(a => a.Block != 0).Select(s => s.Block).ToList();
                        var networkList = apObj.Where(a => a.Network != 0).Select(s => s.Network).ToList();
                        var zoneList = apObj.Where(a => a.Zone != 0).Select(s => s.Zone).ToList();
                        var zoneAllList = apObj.Where(a => a.AllZones == true).ToList();
                        //check for subblock access
                        if (subblockList.Count > 0)
                        {
                            if (overrideFor == "Op")
                            {
                                string channelStr = "";
                                List<int> ChannelIds = await _mainDBContext.SubBlock.Where(s => subblockList.Contains(s.SubblockId)).Select(s => (int)s.ChannelId).ToListAsync();
                                List<int> channelObj = new List<int>();

                                channelObj = await _mainDBContext.Channel.Where(c => ChannelIds.Contains(c.ChannelId)).Select(c => c.ChannelId).ToListAsync();
                                dt = dt.Where(c => channelObj.Contains(c.Id)).ToList();
                            }
                            else
                            {
                                dt = null;
                            }
                            return dt;
                        }
                        else if (blockList.Count > 0)//check for block access
                        {
                            if (overrideFor == "Op")
                            {
                                string channelStr = "";
                                List<int> ChannelIds = await _mainDBContext.SubBlock.Where(s => blockList.Contains(s.BlockId)).Select(c => (int)c.ChannelId).ToListAsync();
                                List<int> channelObj = new List<int>();
                                channelObj = await _mainDBContext.Channel.Where(c => ChannelIds.Contains(c.ChannelId)).Select(c => c.ChannelId).ToListAsync();
                                dt = dt.Where(c => channelObj.Contains(c.Id)).ToList();
                            }
                            else
                            {
                                dt = null;
                            }
                            return dt;

                        }
                        //check for networks access
                        else if (networkList.Count > 0)
                        {
                            string IdStr = "";

                            if (overrideFor == "Op")
                            {//check for channels
                                List<int> blocks = await _mainDBContext.Block.Where(b => networkList.Contains(b.NetworkId) && zoneList.Contains(b.ZoneId)).Select(s => (int)s.BlockId).ToListAsync();
                                if (blocks.Count > 0)
                                {
                                    List<int> rtus = await _mainDBContext.Rtu.Where(r => blocks.Contains((int)r.BlockId) && r.Active == true).Select(s => s.Rtuid).ToListAsync();
                                    List<int> channels = await _mainDBContext.Channel.Where(s => rtus.Contains((int)s.Rtuid)).Select(s => s.ChannelId).ToListAsync();
                                    List<int> channelObj = new List<int>();
                                    channelObj = await _mainDBContext.Channel.Where(c => channels.Contains(c.ChannelId)).Select(c => c.ChannelId).ToListAsync();
                                    dt = dt.Where(c => channelObj.Contains(c.Id)).ToList();
                                }
                                return dt;

                            }
                            else if (overrideFor == "G")
                            {//check for output groups
                                List<int> groupDetails = new List<int>();
                                if (zoneAllList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 1).Select(s => s.GrpId).ToListAsync();
                                }
                                else if (zoneAllList.Count == 0 && zoneList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 1 && zoneList.Contains(g.ZoneId)).Select(s => s.GrpId).ToListAsync();
                                }
                                dt = dt.Where(c => groupDetails.Contains(c.Id)).ToList();
                                return dt;

                            }
                            else if (overrideFor == "RTU")
                            {//check for RTUs
                                List<int> rtus = new List<int>();
                                List<int> blocks = await _mainDBContext.Block.Where(b => networkList.Contains(b.NetworkId) && zoneList.Contains(b.ZoneId)).Select(s => (int)s.BlockId).ToListAsync();
                                if (blocks.Count > 0)
                                {
                                    rtus = await _mainDBContext.Rtu.Where(r => blocks.Contains((int)r.BlockId) && r.Active == true).Select(s => s.Rtuid).ToListAsync();
                                }
                                dt = dt.Where(c => rtus.Contains(c.Id)).ToList();
                                return dt;

                            }
                            else if (overrideFor == "FertGr")
                            {//check for fert group
                                List<int> groupDetails = new List<int>();
                                if (zoneAllList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 3).Select(s => s.GrpId).ToListAsync();
                                }
                                else if (zoneAllList.Count == 0 && zoneList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 3 && zoneList.Contains(g.ZoneId)).Select(s => s.GrpId).ToListAsync();
                                }
                                dt = dt.Where(c => groupDetails.Contains(c.Id)).ToList();
                                return dt;

                            }
                            else if (overrideFor == "FiltGr")
                            {//check for filter group
                                List<int> groupDetails = new List<int>();
                                if (zoneAllList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 2).Select(s => s.GrpId).ToListAsync();

                                }
                                else if (zoneAllList.Count == 0 && zoneList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 2 && zoneList.Contains(g.ZoneId)).Select(s => s.GrpId).ToListAsync();

                                }
                                dt = dt.Where(c => groupDetails.Contains(c.Id)).ToList();
                                return dt;

                            }
                            else if (overrideFor == "MValve")
                            {//check for master valve group
                                List<int> groupDetails = new List<int>();
                                if (zoneAllList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 17).Select(s => s.GrpId).ToListAsync();
                                    foreach (var item in groupDetails)
                                    {
                                        IdStr += item + ",";
                                    }
                                }
                                else if (zoneAllList.Count == 0 && zoneList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 17 && zoneList.Contains(g.ZoneId)).Select(s => s.GrpId).ToListAsync();
                                    foreach (var item in groupDetails)
                                    {
                                        IdStr += item + ",";
                                    }
                                }
                                dt = dt.Where(c => groupDetails.Contains(c.Id)).ToList();
                                return dt;

                            }
                            else if (overrideFor == "MPump")
                            {//check for master Pump group  by soham 20.06.2016
                                List<int> groupDetails = new List<int>();
                                if (zoneAllList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 19).Select(s => s.GrpId).ToListAsync();

                                }
                                else if (zoneAllList.Count == 0 && zoneList.Count > 0)
                                {
                                    groupDetails = await _mainDBContext.GroupDetails.Where(g => g.OpGroupTypeId == 19 && zoneList.Contains(g.ZoneId)).Select(s => s.GrpId).ToListAsync();

                                }
                                dt = dt.Where(c => groupDetails.Contains(c.Id)).ToList();
                                return dt;

                            }
                        }
                    }
                    return dt;
                }
                else if (overrideFor == "Sq")
                {
                    dt = await GetSequenceNamesForMO(NetworkId, ZoneId, BlockId);
                    return dt;
                }
                else if (overrideFor == "Rule")
                {
                    dt = new List<ElementNo>();
                    if (NetworkId == 0)
                    {
                        dt = await _eventDBContext.CustomRulesMaster.Where(x => x.RuleExeFrom == "S" && x.IsDeleted == false).Select(x => new ElementNo { Id = x.RuleId, Name = x.RuleName }).OrderBy(x => x.Id).ToListAsync();
                    }
                    else
                    {
                        dt = await _eventDBContext.CustomRulesMaster.Where(x => x.NetwrokNo == NetworkId && x.IsDeleted == false).Select(x => new ElementNo { Id = x.RuleId, Name = x.RuleName }).OrderBy(x => x.Id).ToListAsync();
                    }
                    return dt;
                }
                // BindElementInDll();
                return dt;
            }
            catch (Exception ex)
            {
                string ErrorMessage = string.Format("Error occured in Bind Element no in managemanualoverride.aspx page.");
                return null;
                //_log.Error(ErrorMessage, ex);
            }
        }

        public async Task<List<ValveScheduleForMO>> GetValveScheduleForMO(string Elemets, string sdt, string edt)
        {
            List<ValveScheduleForMO> listele = null;
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ChannelId", Elemets);
                    parameters.Add("@StartDateTime", sdt);
                    parameters.Add("@EndDateTime", edt);
                    var result = await sqlConnection.QueryMultipleAsync("GetValveScheduleForMO", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    listele = resultIntervals.Select(x =>
                    new ValveScheduleForMO
                    {
                        SeqStDate = x.SeqStDate,
                        SeqEndDate = x.SeqEndDate,
                        SeqId = x.SeqId,
                        ChannelId = x.ChannelId,
                        valve = x.valve,
                        StartTimeSpanId = x.StartTimeSpanId,
                        StartTime = x.StartTime,
                        EndTimeSpanId = x.EndTimeSpanId,
                        EndTime = x.EndTime
                    }).ToList();
                    return listele;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ElementNo>> GetSequenceNamesForMO(int NetworkId, int ZoneId, int BlockId)
        {
            List<SequenceForMO> list = null;
            List<ElementNo> listele = null;
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@networkid", NetworkId);
                    parameters.Add("@zoneid", ZoneId);
                    parameters.Add("@blkid", BlockId);
                    var result = await sqlConnection.QueryMultipleAsync("GetSequenceForMO", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    list = resultIntervals.Select(x => new SequenceForMO { SeqId = x.SeqId, SeqName = x.SeqName }).ToList();
                    listele = list.Select(x => new ElementNo { Id = x.SeqId, Name = x.SeqName }).ToList();
                    return listele;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ElementNo>> GetChannelNamesForMO(int NetworkId, int ZoneId, int BlockId, int EqpTypeId, int SubTypeId, int EventObjtypeid)
        {
            List<ElementNo> list = null;
            try
            {
                using (var sqlConnection = new SqlConnection(DbManager.GetDbConnectionString(DbManager.SiteName, "Main")))
                {
                    await sqlConnection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@networkid", NetworkId);
                    parameters.Add("@zoneid", ZoneId);
                    parameters.Add("@blkid", BlockId);
                    parameters.Add("@EqpTypeID", EqpTypeId);
                    parameters.Add("@SubTypeId", SubTypeId);
                    parameters.Add("@EventObjTypeId", EventObjtypeid);
                    var result = await sqlConnection.QueryMultipleAsync("GetChannelDetailsForManualOverride", parameters, null, null, CommandType.StoredProcedure);
                    var resultIntervals = await result.ReadAsync();
                    if (EventObjtypeid == 6)
                    {
                        list = resultIntervals.Select(x => new ElementNo { Id = x.Id, Name = x.Name, RTUId = x.RTUId }).ToList();

                    }
                    else
                    {
                        list = resultIntervals.Select(x => new ElementNo { Id = x.Id, Name = x.Name }).ToList();

                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                return list;
            }
        }
        //private void GenerateConfirmationText()
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        if (ddlAction.SelectedValue != "Select")
        //        {
        //            if (ddlElementToOverride.SelectedValue != "Select")
        //            {
        //                int flag = 0;
        //                foreach (ListItem lst in ddlElementNo.Items)
        //                {
        //                    if (lst.Selected)
        //                    {
        //                        flag = 1;
        //                        break;
        //                    }
        //                    else
        //                    {
        //                        flag = 0;
        //                    }
        //                }
        //                if (flag == 1)
        //                {
        //                    if (ddlElementToOverride.SelectedValue != "Sq")
        //                    {
        //                        sb.Append(ddlElementToOverride.SelectedItem.Text + " ");
        //                        foreach (ListItem item in ddlElementNo.Items)
        //                        {
        //                            if (item.Selected)
        //                            {
        //                                sb.Append(item.Text + ",");
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        foreach (ListItem item in ddlElementNo.Items)
        //                        {
        //                            if (item.Selected)
        //                            {
        //                                sb.Append(item.Text + ",");
        //                            }
        //                        }
        //                    }
        //                    sb.Remove(sb.Length - 1, 1);
        //                    sb.Append(" " + GlobalConstants.willbe + " " + ddlAction.SelectedItem.Text);
        //                    hdnMOFor.Value = sb.ToString();
        //                }
        //                else
        //                {
        //                    hdnMOFor.Value = "Cs";
        //                }

        //            }
        //            else
        //            {
        //                if (ddlNetworkNo.SelectedValue != "Select" && ddlZoneNo.SelectedValue == "Select" && ddlBlockNo.SelectedValue == "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause network
        //                    sb.Append(GlobalConstants.Network + " " + ddlNetworkNo.SelectedItem.Text);
        //                }
        //                else if (ddlNetworkNo.SelectedValue != "Select" && ddlZoneNo.SelectedValue != "Select" && ddlBlockNo.SelectedValue == "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause network
        //                    sb.Append(GlobalConstants.Zone + " " + ddlZoneNo.SelectedItem.Text);
        //                }
        //                else if (ddlNetworkNo.SelectedValue != "Select" && ddlZoneNo.SelectedValue != "Select" && ddlBlockNo.SelectedValue == "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause zone
        //                    sb.Append(GlobalConstants.Zone + " " + ddlZoneNo.SelectedItem.Text);
        //                }
        //                else if (ddlNetworkNo.SelectedValue != "Select" && ddlZoneNo.SelectedValue == "Select" && ddlBlockNo.SelectedValue != "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause block
        //                    sb.Append(GlobalConstants.Block + " " + ddlBlockNo.SelectedItem.Text);
        //                }
        //                else if (ddlNetworkNo.SelectedValue == "Select" && ddlZoneNo.SelectedValue != "Select" && ddlBlockNo.SelectedValue != "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause block
        //                    sb.Append(GlobalConstants.Block + " " + ddlBlockNo.SelectedItem.Text);
        //                }
        //                else if (ddlNetworkNo.SelectedValue != "Select" && ddlZoneNo.SelectedValue != "Select" && ddlBlockNo.SelectedValue != "Select" && ddlElementToOverride.SelectedValue == "Select" && ddlElementNo.SelectedValue == "")
        //                {
        //                    //Pause block
        //                    sb.Append(GlobalConstants.Block + " " + ddlBlockNo.SelectedItem.Text);
        //                }
        //                sb.Append(" " + GlobalConstants.willbe + " " + ddlAction.SelectedItem.Text);

        //                hdnMOFor.Value = sb.ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrorMessage = string.Format("Error occured generate confirmation text in managemanualoverride.aspx page.");
        //        //_log.Error(ErrorMessage, ex);
        //    }

        //}

        public async Task<List<ValveScheduleForMO>> GetValveScheduleForElements(int moId, string Elemets, string overridefor)
        {
            try
            {
                var moMaster = await _eventDBContext.ManualOverrideMaster.Where(x => x.Moid == moId).FirstOrDefaultAsync();
                List<ValveScheduleForMO> ValveSchedule = new List<ValveScheduleForMO>();
                DateTime dt = await _zoneTimeService.TimeZoneDateTime();
                if (overridefor == "Op")
                {
                    DateTime st = Convert.ToDateTime(moMaster.MostartDateTime);
                    DateTime et = Convert.ToDateTime(moMaster.MoendDateTime);

                    ValveSchedule = await GetValveScheduleForMO(Elemets, st.ToString("MM/dd/yyyy"), et.ToString("MM/dd/yyyy"));
                }
                else if (overridefor == "G")
                {
                    List<string> GrNames = new List<string>();

                    GrNames = Elemets.Split(",").ToList();
                    var vlist = await _mainDBContext.SequenceValveConfig.Where(v => GrNames.Contains(v.GroupName)).Select(v => new { v.ChannelId }).Distinct().ToListAsync();
                    foreach (var v in vlist)
                    {
                        Elemets += v.ChannelId + ",";
                    }
                    if (Elemets != "")
                    {
                        if (Elemets.IndexOf(",") >= 0)
                        {
                            Elemets = Elemets.Remove(Elemets.LastIndexOf(","));
                        }
                    }
                    ValveSchedule = await GetValveScheduleForMO(Elemets, dt.ToString("MM/dd/yyyy"), dt.ToString("MM/dd/yyyy"));
                }
                else if (overridefor == "Sq")
                {
                    List<int> SeqId = new List<int>();

                    SeqId = Elemets.Split(",").ToList().ConvertAll(int.Parse);

                    var vlist = await _mainDBContext.SequenceValveConfig.Where(v => SeqId.Contains(Convert.ToInt32(v.SeqId))).Select(v => new { v.ChannelId }).Distinct().ToListAsync();
                    foreach (var v in vlist)
                    {
                        Elemets += v.ChannelId + ",";
                    }
                    if (Elemets != "")
                    {
                        if (Elemets.IndexOf(",") >= 0)
                        {
                            Elemets = Elemets.Remove(Elemets.LastIndexOf(","));
                        }
                    }
                    ValveSchedule = await GetValveScheduleForMO(Elemets, dt.ToString("MM/dd/yyyy"), dt.ToString("MM/dd/yyyy"));
                }
                else if (overridefor == "RTU")
                {
                    List<int> rtuids = new List<int>();

                    rtuids = Elemets.Split(",").ToList().ConvertAll(int.Parse);
                    foreach (var item in rtuids)
                    {
                        List<int> ds = await GetRTUDetails(item);

                        if (ds != null)
                        {
                            if (ds.Count > 0)
                            {
                                foreach (var cnt in ds)
                                {
                                    Elemets += cnt.ToString() + ",";
                                }
                            }
                        }
                    }
                    ValveSchedule = await GetValveScheduleForMO(Elemets, dt.ToString("MM/dd/yyyy"), dt.ToString("MM/dd/yyyy"));
                }

                //HD
                //16 Nov 2015
                //Show valveschedule only if applicable
                if (ValveSchedule != null)
                {
                    if (ValveSchedule.Count > 0)
                    {
                        if (overridefor == "Sq" && overridefor != "")
                        {
                            return ValveSchedule.Where(x => x.SeqId == Convert.ToInt32(Elemets)).ToList();
                        }
                        else
                        {
                            return ValveSchedule;
                        }

                    }
                }
                return ValveSchedule;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<int>> GetRTUDetails(int rtuId)
        {
            try
            {
                List<int> lst1 = await (from c in _mainDBContext.Channel
                                        join a in _mainDBContext.AddressDetails on c.SlotIdInRtu equals a.AnalogIp
                                        join d in _mainDBContext.AnalogSensorType on c.TypeId equals d.AnalogSensorTypeId
                                        where (c.Rtuid == rtuId && c.IsExpansionCardSlot == false && a.IsExpansionCard == false && c.EqpTypeId == 1)
                                        select c.ChannelId).ToListAsync();

                List<int> lst2 = await (from c in _mainDBContext.Channel
                                        join a in _mainDBContext.AddressDetails on c.SlotIdInRtu equals a.AnalogIp
                                        join d in _mainDBContext.DigitalOutput on c.TypeId equals d.Doid
                                        where (c.Rtuid == rtuId && c.IsExpansionCardSlot == false && a.IsExpansionCard == false && c.EqpTypeId == 4)
                                        select c.ChannelId).ToListAsync();

                List<int> lst3 = await (from c in _mainDBContext.Channel
                                        join a in _mainDBContext.AddressDetails on c.SlotIdInRtu equals a.AnalogIp
                                        join d in _mainDBContext.DigitalCounter on c.SubTypeId equals d.DcntId
                                        where (c.Rtuid == rtuId && c.IsExpansionCardSlot == false && a.IsExpansionCard == false && c.EqpTypeId == 3)
                                        select c.ChannelId).ToListAsync();

                List<int> lst4 = await (from c in _mainDBContext.Channel
                                        join a in _mainDBContext.AddressDetails on c.SlotIdInRtu equals a.AnalogIp
                                        join d in _mainDBContext.DigitalNonc on c.SlotIdInRtu equals d.DnoncId
                                        where (c.Rtuid == rtuId && c.IsExpansionCardSlot == false && a.IsExpansionCard == false && c.EqpTypeId == 2)
                                        select c.ChannelId).ToListAsync();

                List<int> finalLst = new List<int>();
                finalLst.AddRange(lst1);
                finalLst.AddRange(lst2);
                finalLst.AddRange(lst3);
                finalLst.AddRange(lst4);
                return finalLst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
