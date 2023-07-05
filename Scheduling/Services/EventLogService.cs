using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IEventLogService
    {
        Task<EventlogDDlViewModel> GetDDLList(string priviledges, string userId);
       // Task<EventlogDDlViewModel> GetDDLListByZoneId(string priviledges, string userId, int networkId);

        Task<dynamic> GetValEvents(int zoneId, int networkId, int blockId, int rtuId);
    }
    public class EventLogService : IEventLogService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private EventDBContext _eventDBContext;
        private IZoneTimeService _zoneTimeService;

        private readonly ILogger<EventLogService> _logger;
        public EventLogService(ILogger<EventLogService> logger,
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
        public async Task<EventlogDDlViewModel> GetDDLList(string priviledges, string userId)
        {

            try
            {
                EventlogDDlViewModel model = new EventlogDDlViewModel();
                List<MoNetwork> networkLst = new List<MoNetwork>();
                List<Network> lstnetwork = await _mainDBContext.Network.ToListAsync();
                List<MoZone> ZoneLst = new List<MoZone>();
                List<KeyValueViewModel> Blocks = new List<KeyValueViewModel>();

                if (priviledges == "All" || priviledges.Contains("57") || priviledges.Contains("58") || priviledges.Contains("59") || priviledges.Contains("60") || priviledges.Contains("61") || priviledges.Contains("62") || priviledges.Contains("63") || priviledges.Contains("64") || priviledges.Contains("65") || priviledges.Contains("88") || priviledges.Contains("89") || priviledges.Contains("90") || priviledges.Contains("91"))
                {
                    if (priviledges == "All")
                    {
                        ZoneLst = await _mainDBContext.Zone.OrderBy(z => z.ZoneNo).Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();
                        networkLst = await _mainDBContext.Network.OrderBy(z => z.NetworkNo).Select(r => new MoNetwork { NetworkName = r.Name, NetworkId = r.NetworkId, NetworkNo = r.NetworkNo }).ToListAsync();
                        Blocks = await _mainDBContext.Block.Select(x => new KeyValueViewModel { Value = x.BlockId, Text = x.Name }).ToListAsync();
                    }
                    else
                    {
                        ZoneLst = await _mainDBContext.Zone.Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();
                        networkLst = await _mainDBContext.Network.OrderBy(z => z.NetworkNo).Select(r => new MoNetwork { NetworkName = r.Name, NetworkId = r.NetworkId, NetworkNo = r.NetworkNo }).ToListAsync();
                        Blocks = await _mainDBContext.Block.Select(x => new KeyValueViewModel { Value = x.BlockId, Text = x.Name }).ToListAsync();
                        List<AdminPrivileges> apObj = await _mainDBContext.AdminPrivileges.Where(ap => ap.UserId == userId).ToListAsync();
                        //Network
                        if (apObj.Where(a => a.AllNetworks == true).ToList().Count > 0)
                        {
                            networkLst = await _mainDBContext.Network.OrderBy(z => z.NetworkNo).Select(r => new MoNetwork { NetworkName = r.Name, NetworkId = r.NetworkId, NetworkNo = r.NetworkNo }).ToListAsync(); ;
                        }
                        else
                        {
                            List<int> networkObj = apObj.Where(a => a.Network != 0).Select(n => (int)n.Network).ToList();
                            networkLst = networkLst.OrderBy(a => a.NetworkNo).Where(s => networkObj.Contains(s.NetworkId)).Select(r => new MoNetwork { NetworkName = r.NetworkName, NetworkId = r.NetworkId, NetworkNo = r.NetworkNo }).ToList();
                        }

                        //Zone
                        if (apObj.Where(a => a.AllZones == true).ToList().Count > 0)
                        {
                            ZoneLst = await _mainDBContext.Zone.OrderBy(s => s.ZoneNo).Select(r => new MoZone { ZoneName = r.Name, ZoneId = r.ZoneId }).ToListAsync();

                        }
                        else
                        {
                            List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
                            List<int> blocksObj = apObj.Where(a => a.Block != 0).Select(n => (int)n.Block).ToList();
                            List<int> zonesids = await _mainDBContext.Block.Where(z => blocksObj.Contains(z.BlockId)).Select(s => (int)s.ZoneId).ToListAsync();
                            ZoneLst = (List<MoZone>)ZoneLst.Where(s => zoneObj.Contains(s.ZoneId) || zonesids.Contains(s.ZoneId)).Distinct();
                        }

                        if (apObj.Where(a => a.AllBlocks == true).ToList().Count > 0)
                        {
                            Blocks = await _mainDBContext.Block.Select(x => new KeyValueViewModel { Value = x.BlockId, Text = x.Name }).ToListAsync();
                        }
                        else
                        {
                            List<int> BLOCKObj = apObj.Where(a => a.Block != 0).Select(n => (int)n.Block).ToList();
                            Blocks = Blocks.OrderBy(a => a.Value).Where(s => BLOCKObj.Contains(s.Value)).Select(r => new KeyValueViewModel { Text = r.Text, Value = r.Value }).ToList();
                        }

                    }
                }
                else
                {
                    return null;
                }

                model.Networks = networkLst;
                model.Zones = ZoneLst;
                model.Blocks = Blocks;

                return model;
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        public Task<dynamic> GetValEvents(int zoneId, int networkId, int blockId, int rtuId)
        {
            throw new NotImplementedException();
        }


        #region Commented
        //public async Task<EventlogDDlViewModel> GetDDLListByZoneId(string priviledges, string userId, int zoneId)
        //{
        //    try
        //    {
        //        EventlogDDlViewModel model = new EventlogDDlViewModel();
        //        List<MoNetwork> networkLst = new List<MoNetwork>();
        //        List<MoZone> ZoneLst = new List<MoZone>();
        //        List<KeyValueViewModel> Blocks = new List<KeyValueViewModel>();
        //        if (priviledges == "All")
        //        {

        //            IEnumerable<object> nwLst = null;
        //            if (zoneId != 0)
        //            {
        //                //Get networks in zone
        //                List<int> nwids = await _mainDBContext.ZoneInNetwork.Where(b => b.ZoneId == zoneId).Select(s => (int)s.NetworkId).ToListAsync();
        //                if (nwids.Count == 0)
        //                {
        //                    nwids = await _mainDBContext.Block.Where(b => b.ZoneId == zoneId).Select(s => (int)s.NetworkId).ToListAsync();
        //                }
        //                else
        //                {
        //                    nwLst = await _mainDBContext.Network.OrderBy(a => a.NetworkNo).Where(z => nwids.Contains(z.NetworkId)).Select(r => new { DisplayText = r.Name, Value = r.NetworkId }).ToListAsync();
        //                }
        //            }
        //            else
        //            {
        //                return null;
        //            }

        //            if (nwLst.ToList().Count > 0)
        //            {
        //                DDLNetwork.DataSource = nwLst;
        //                DDLNetwork.DataValueField = "Value";
        //                DDLNetwork.DataTextField = "DisplayText";
        //                DDLNetwork.DataBind();
        //                DDLNetwork.Items.Insert(0, new ListItem("Select", "0"));
        //                DDLNetwork.SelectedValue = "0";
        //            }
        //        }
        //        else
        //        {

        //            var NwTypes = jdc.Networks.OrderBy(a => a.NetworkNo).Select(r => new { DisplayText = r.Name, Value = r.NetworkId });
        //            IEnumerable<AdminPrivilege> apObj = jdc.AdminPrivileges.Where(ap => ap.UserId == HttpContext.Current.Session["UserId"].ToString());
        //            if (apObj.Where(a => a.AllNetworks == true).ToList().Count > 0)
        //            {
        //                NwTypes = jdc.Networks.OrderBy(a => a.NetworkNo).Select(r => new { DisplayText = r.Name, Value = r.NetworkId });
        //            }
        //            else
        //            {
        //                List<int> networkObj = apObj.Where(a => a.Network != 0).Select(n => (int)n.Network).ToList();
        //                NwTypes = NwTypes.Where(s => networkObj.Contains(s.Value));

        //                //int ZN = Convert.ToInt32(DDLZone.SelectedValue);
        //                //var networkId = jdc.Blocks.OrderBy(b => b.BlockNo).Where(b => b.ZoneId == Convert.ToInt32(DDLZone.SelectedValue)).Select(s => s.NetworkId).Distinct().ToArray();
        //                //NwTypes = jdc.Networks.OrderBy(c => c.NetworkNo).Where(n => networkId.Contains(n.NetworkId) && networkObj.Contains(n.Value)).Select(r => new { DisplayText = r.Name, Value = r.NetworkId });
        //            }

        //            if (NwTypes.ToList().Count > 0)
        //            {

        //                DDLNetwork.DataSource = NwTypes;
        //                DDLNetwork.DataValueField = "Value";
        //                DDLNetwork.DataTextField = "DisplayText";
        //                DDLNetwork.DataBind();
        //                DDLNetwork.Items.Insert(0, new ListItem("Select", "0"));
        //                DDLNetwork.SelectedValue = "0";
        //                if (Request.Form["NetworkId"] != null)
        //                {
        //                    DDLNetwork.SelectedValue = Request.Form["NetworkId"];
        //                }
        //            }
        //            else
        //            {

        //                DDLNetwork.Items.Insert(0, new ListItem("Select", "Select"));

        //            }

        //        }


        //        BindZoneNwBlock();
        //        BindZoneNwRtu();
        //        BindZoneNwChannels();

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
        //public void BindZoneNwRtu()
        //{
        //    try
        //    {
        //        if (DDLBlock.SelectedValue != "Select")
        //        {
        //            if (hfEventSensoDataVal.Value == "11" || hfEventVal.Value == "4" || hfEventSensoDataVal.Value == "16")
        //            {

        //                if (Session["Privileges"] != null && Session["UserId"] != null)
        //                {
        //                    if (Session["Privileges"].ToString() == "All")
        //                    {
        //                        //var ZoneNwRtu = jdc.RTUs.Where(r => r.BlockId == Convert.ToInt32(DDLBlock1.SelectedValue) && r.Active == true).OrderBy(s => s.RTUNo).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                        var ZoneNwRtu = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                         .Where(r => r.Active == true && r.BlockId == Convert.ToInt32(DDLBlock.SelectedValue) && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                        DDLRtu.DataSource = ZoneNwRtu;
        //                        DDLRtu.DataValueField = "RTUId";
        //                        DDLRtu.DataTextField = "RTUName";
        //                        DDLRtu.DataBind();
        //                    }
        //                    else
        //                    {
        //                        string[] pvResult = Session["Privileges"].ToString().Split(',');
        //                        if (pvResult.Contains("1"))
        //                        {
        //                            JainH2OAdminServices.JainH2OAdminService jas = new JainH2OAdminServices.JainH2OAdminService();
        //                            DataSet ds = jas.GetNZBRListForSensorAndOutputConfig(Session["UserId"].ToString(), 'R', 'B', Convert.ToInt32(DDLBlock.SelectedValue));
        //                            //for only ai di rtus
        //                            var ZoneNwRtu = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                                            .Where(r => r.Active == true && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                            DataView dvFilter = ds.Tables[0].DefaultView;
        //                            DataTable dtTemp = new DataTable();
        //                            List<int> listRTUIds = ZoneNwRtu.AsEnumerable().Select(s => s.RTUId).ToList();
        //                            string result = string.Join<int>(", ", listRTUIds);
        //                            dvFilter.RowFilter = "Value in (" + result + ")";
        //                            dtTemp = dvFilter.ToTable();
        //                            DDLRtu.DataSource = dtTemp;

        //                            DDLRtu.DataValueField = "Value";
        //                            DDLRtu.DataTextField = "DisplayText";
        //                            DDLRtu.DataBind();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var ZoneNwRtu = jdc.RTUs.OrderBy(s => s.RTUNo).Where(r => r.BlockId == Convert.ToInt32(DDLBlock.SelectedValue) && r.Active == true).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId, Tagname = r.TagName });
        //                DDLRtu.DataSource = ZoneNwRtu;

        //                DDLRtu.DataValueField = "Value";
        //                DDLRtu.DataTextField = "DisplayText";
        //                DDLRtu.DataBind();
        //            }


        //        }
        //        else if (DDLNetwork.SelectedValue != "Select")
        //        {
        //            if (hfEventSensoDataVal.Value == "11" || hfEventVal.Value == "4" || hfEventSensoDataVal.Value == "16")
        //            {
        //                if (Session["Privileges"] != null && Session["UserId"] != null)
        //                {
        //                    if (Session["Privileges"].ToString() == "All")
        //                    {
        //                        //var ZoneNwRtu = jdc.RTUs.Where(r => r.NetworkId == Convert.ToInt32(DDLNetwork1.SelectedValue) && r.Active == true).OrderBy(s => s.RTUNo).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                        var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                         .Where(r => r.Active == true && r.NetworkId == Convert.ToInt32(DDLNetwork.SelectedValue) && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                        DDLRtu.DataSource = ZoneNwRtu2;
        //                        DDLRtu.DataValueField = "RTUId";
        //                        DDLRtu.DataTextField = "RTUName";
        //                        DDLRtu.DataBind();
        //                    }
        //                    else
        //                    {
        //                        string[] pvResult = Session["Privileges"].ToString().Split(',');
        //                        if (pvResult.Contains("1"))
        //                        {
        //                            JainH2OAdminServices.JainH2OAdminService jas = new JainH2OAdminServices.JainH2OAdminService();
        //                            DataSet ds = jas.GetNZBRListForSensorAndOutputConfig(Session["UserId"].ToString(), 'R', 'N', Convert.ToInt32(DDLNetwork.SelectedValue));

        //                            //for only ai di rtus
        //                            var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                                            .Where(r => r.Active == true && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                            DataView dvFilter = ds.Tables[0].DefaultView;
        //                            DataTable dtTemp = new DataTable();
        //                            List<int> listRTUIds = ZoneNwRtu2.AsEnumerable().Select(s => s.RTUId).ToList();
        //                            string result = string.Join<int>(", ", listRTUIds);
        //                            dvFilter.RowFilter = "Value in (" + result + ")";
        //                            dtTemp = dvFilter.ToTable();

        //                            DDLRtu.DataSource = dtTemp;
        //                            DDLRtu.DataValueField = "Value";
        //                            DDLRtu.DataTextField = "DisplayText";
        //                            DDLRtu.DataBind();

        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var ZoneNwRtu = jdc.RTUs.OrderBy(s => s.RTUNo).Where(r => r.NetworkId == Convert.ToInt32(DDLNetwork.SelectedValue) && r.Active == true).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                DDLRtu.DataSource = ZoneNwRtu;

        //                DDLRtu.DataValueField = "Value";
        //                DDLRtu.DataTextField = "DisplayText";
        //                DDLRtu.DataBind();
        //            }



        //        }
        //        else if (DDLZone.SelectedValue != "Select")
        //        {

        //            if (hfEventSensoDataVal.Value == "11" || hfEventVal.Value == "4" || hfEventSensoDataVal.Value == "16")
        //            {
        //                if (Session["Privileges"] != null && Session["UserId"] != null)
        //                {
        //                    if (Session["Privileges"].ToString() == "All")
        //                    {
        //                        var blockIds = jdc.Blocks.Where(b => b.ZoneId == Convert.ToInt32(DDLZone.SelectedValue)).Select(b => b.BlockId).ToList();
        //                        //var ZoneNwRtu = jdc.RTUs.Where(r => blockIds.Contains(Convert.ToInt32(r.BlockId)) && r.Active == true).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                        var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                         .Where(r => r.Active == true && blockIds.Contains(Convert.ToInt32(r.BlockId)) && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                        DDLRtu.DataSource = ZoneNwRtu2;

        //                        DDLRtu.DataValueField = "RTUId";
        //                        DDLRtu.DataTextField = "RTUName";
        //                        DDLRtu.DataBind();
        //                    }
        //                    else
        //                    {
        //                        string[] pvResult = Session["Privileges"].ToString().Split(',');
        //                        if (pvResult.Contains("1"))
        //                        {
        //                            JainH2OAdminServices.JainH2OAdminService jas = new JainH2OAdminServices.JainH2OAdminService();
        //                            DataSet ds = jas.GetNZBRListForSensorAndOutputConfig(Session["UserId"].ToString(), 'R', 'Z', Convert.ToInt32(DDLZone.SelectedValue));
        //                            //for only ai di rtus
        //                            var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                                            .Where(r => r.Active == true && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                            DataView dvFilter = ds.Tables[0].DefaultView;
        //                            DataTable dtTemp = new DataTable();
        //                            List<int> listRTUIds = ZoneNwRtu2.AsEnumerable().Select(s => s.RTUId).ToList();
        //                            string result = string.Join<int>(", ", listRTUIds);
        //                            dvFilter.RowFilter = "Value in (" + result + ")";
        //                            dtTemp = dvFilter.ToTable();

        //                            DDLRtu.DataSource = dtTemp;

        //                            DDLRtu.DataValueField = "Value";
        //                            DDLRtu.DataTextField = "DisplayText";
        //                            DDLRtu.DataBind();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var blockIds = jdc.Blocks.OrderBy(b => b.BlockNo).Where(b => b.ZoneId == Convert.ToInt32(DDLZone.SelectedValue)).Select(b => b.BlockId).ToList();
        //                var ZoneNwRtu = jdc.RTUs.OrderBy(s => s.RTUNo).Where(r => blockIds.Contains(Convert.ToInt32(r.BlockId)) && r.Active == true).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                DDLRtu.DataSource = ZoneNwRtu;

        //                DDLRtu.DataValueField = "Value";
        //                DDLRtu.DataTextField = "DisplayText";
        //                DDLRtu.DataBind();
        //            }


        //        }
        //        else
        //        {
        //            if (hfEventSensoDataVal.Value == "11" || hfEventVal.Value == "4" || hfEventSensoDataVal.Value == "16")
        //            {
        //                if (Session["Privileges"] != null && Session["UserId"] != null)
        //                {
        //                    if (Session["Privileges"].ToString() == "All")
        //                    {
        //                        //var ZoneNwRtu = jdc.RTUs.Where(r => r.Active == true).OrderBy(s => s.NetworkId).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                        var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                            .Where(r => r.Active == true && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                        DDLRtu.DataSource = ZoneNwRtu2;
        //                        DDLRtu.DataValueField = "RTUId";
        //                        DDLRtu.DataTextField = "RTUName";
        //                        DDLRtu.DataBind();
        //                    }
        //                    else
        //                    {
        //                        string[] pvResult = Session["Privileges"].ToString().Split(',');
        //                        if (pvResult.Contains("1"))
        //                        {

        //                            JainH2OAdminServices.JainH2OAdminService jas = new JainH2OAdminServices.JainH2OAdminService();
        //                            DataSet ds = jas.GetNZBRListForSensorAndOutputConfig(Session["UserId"].ToString(), 'R', 'R', 0);
        //                            //for only ai di rtus
        //                            var ZoneNwRtu2 = jdc.RTUs.Join(jdc.RTUModels, r => r.RTUModelId, m => m.RTUModelId, (r, m) => new { r.RTUName, r.RTUId, r.BlockId, r.Active, r.RTUNo, m.NoOfAnalogIp, m.NoOfDigitalIp, r.NetworkId, ActiveRTUModels = m.Active })
        //                                            .Where(r => r.Active == true && r.ActiveRTUModels == true && (r.NoOfAnalogIp != 0 || r.NoOfDigitalIp != 0)).OrderBy(s => s.RTUNo);
        //                            DataView dvFilter = ds.Tables[0].DefaultView;
        //                            DataTable dtTemp = new DataTable();
        //                            List<int> listRTUIds = ZoneNwRtu2.AsEnumerable().Select(s => s.RTUId).ToList();
        //                            string result = string.Join<int>(", ", listRTUIds);
        //                            dvFilter.RowFilter = "Value in (" + result + ")";
        //                            dtTemp = dvFilter.ToTable();

        //                            DDLRtu.DataSource = dtTemp;
        //                            DDLRtu.DataValueField = "Value";
        //                            DDLRtu.DataTextField = "DisplayText";
        //                            DDLRtu.DataBind();
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    string ErrorMessage = "Error occured in BindZoneNwRtu event in MaintainConfigSensor page";
        //                    _log.Error(ErrorMessage);
        //                    Response.Redirect("../Landing/Login.aspx", false);
        //                }
        //            }
        //            else
        //            {
        //                var ZoneNwRtu = jdc.RTUs.OrderBy(s => s.RTUNo).Where(r => r.Active == true).Select(r => new { DisplayText = r.RTUName, Value = r.RTUId });
        //                DDLRtu.DataSource = ZoneNwRtu;
        //                DDLRtu.DataValueField = "Value";
        //                DDLRtu.DataTextField = "DisplayText";
        //                DDLRtu.DataBind();
        //            }


        //        }

        //        DDLRtu.Items.Insert(0, new ListItem("Select", "Select"));
        //    }

        //    catch (Exception ex)
        //    {
        //        string ErrorMessage = string.Format("Error while fetching data in BindZoneNwRtu function of MaintainEventLogBook page.");
        //        //_log.Error(ErrorMessage, ex);
        //    }

        //}
        //public void BindZoneNwBlock()
        //{
        //    try
        //    {
        //        JainH2ODataAccessDataContext jdc = new JainH2ODataAccessDataContext(MyConString);

        //        if (DDLNetwork.SelectedValue != "Select" && DDLNetwork.SelectedValue != "0")
        //        {
        //            var ZoneNwBlock = jdc.Blocks.OrderBy(b => b.BlockNo).Where(r => r.NetworkId == Convert.ToInt32(DDLNetwork.SelectedValue) && r.ZoneId != 0).Select(r => new { DisplayText = r.Name, Value = r.BlockId });
        //            DDLBlock.DataSource = ZoneNwBlock;
        //            DDLBlock.DataValueField = "Value";
        //            DDLBlock.DataTextField = "DisplayText";
        //            DDLBlock.DataBind();
        //        }
        //        else if (DDLZone.SelectedValue != "Select")
        //        {
        //            //var ZoneNwBlock = jdc.Blocks.Where(r => r.ZoneId == Convert.ToInt32(DDLZone.SelectedValue)).Select(r => new { DisplayText = r.Name, Value = r.BlockId });
        //            var ZoneNwBlock = jdc.Blocks.OrderBy(b => b.BlockNo).Where(r => r.ZoneId == Convert.ToInt32(DDLZone.SelectedValue)).Select(r => new { DisplayText = r.Name, Value = r.BlockId });
        //            DDLBlock.DataSource = ZoneNwBlock;
        //            DDLBlock.DataValueField = "Value";
        //            DDLBlock.DataTextField = "DisplayText";
        //            DDLBlock.DataBind();
        //        }
        //        else
        //        {


        //            if (Session["Privileges"].ToString() == "All")
        //            {
        //                var zoneName = jdc.Zones.OrderBy(s => s.ZoneNo).Select(r => new { DisplayText = r.Name, Value = r.ZoneId });
        //                var ZoneNwBlock = jdc.Blocks.Where(r => r.ZoneId != 0).OrderBy(b => b.BlockNo).Select(r => new { DisplayText = r.Name, Value = r.BlockId });
        //                DDLBlock.DataSource = ZoneNwBlock;
        //                DDLBlock.DataValueField = "Value";
        //                DDLBlock.DataTextField = "DisplayText";
        //                DDLBlock.DataBind();
        //            }
        //            else
        //            {
        //                List<int> zoneName = jdc.Zones.OrderBy(s => s.ZoneNo).Select(s => (int)s.ZoneId).ToList();
        //                IEnumerable<AdminPrivilege> apObj = jdc.AdminPrivileges.Where(ap => ap.UserId == HttpContext.Current.Session["UserId"].ToString());
        //                if (apObj.Where(a => a.AllZones == true).ToList().Count > 0)
        //                {
        //                    zoneName = jdc.Zones.OrderBy(s => s.ZoneNo).Select(s => (int)s.ZoneId).ToList();
        //                }
        //                else
        //                {
        //                    List<int> zoneObj = apObj.Where(a => a.Zone != 0).Select(n => (int)n.Zone).ToList();
        //                    List<int> blocksObj = apObj.Where(a => a.Block != 0).Select(n => (int)n.Block).ToList();
        //                    List<int> zones = jdc.Blocks.Where(z => blocksObj.Contains(z.BlockId)).Select(s => (int)s.ZoneId).ToList();
        //                    //zoneName = zoneName.Where(s => zoneObj.Contains(s.Value) || zones.Contains(s.Value)).Distinct().ToList();
        //                    zoneName = jdc.Zones.Where(s => zoneObj.Contains(s.ZoneId) || zones.Contains(s.ZoneId)).OrderBy(s => s.ZoneNo).Distinct().Select(s => (int)s.ZoneId).ToList();
        //                }
        //                var ZoneNwBlock = jdc.Blocks.Where(r => r.ZoneId != 0 && zoneName.Contains((int)r.ZoneId)).OrderBy(b => b.BlockNo).Select(r => new { DisplayText = r.Name, Value = r.BlockId });
        //                DDLBlock.DataSource = ZoneNwBlock;
        //                DDLBlock.DataValueField = "Value";
        //                DDLBlock.DataTextField = "DisplayText";
        //                DDLBlock.DataBind();
        //            }

        //        }

        //        DDLBlock.Items.Insert(0, new ListItem("Select", "Select"));
        //    }
        //    catch (Exception ex)
        //    {
        //        string ErrorMessage = string.Format("Error while fetching data in BindZoneNwBlock function of MaintainEventLogBook page.");
        //        //_log.Error(ErrorMessage, ex);
        //    }

        //} 
        #endregion

    }
}
