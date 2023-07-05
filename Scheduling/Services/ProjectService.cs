using AutoMapper;
using Dapper;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project = Scheduling.Data.Entities.Project;

namespace Scheduling.Services
{
    public interface IProjectService
    {
        Task<List<MultiNetworkRtu>> GetMultiNetworkRtu();
        Task<List<Gateway>> GetGateways();
        Task<List<MultiNodeDashbordData>> GetMultiNodeLatLong();
        //Task<string> CheckSequenceValidity();
        //Task<string> GetStringAsync();
        Task<bool> CheckProjectSerialNo(int id, int srnno);
        Task<bool> UpdateGateway(Gateway model);
        Task<List<UpdateIdsRequired>> GetUpdateIds();
        Task<IEnumerable> GetUsersHavingPrivilageOnlySevenAndLogout(string userId, int flag, DateTime TimeZoneDateTime);
        Task<ProjectViewModel> GetProjectForTimeZone();

        Task<MultiUiversion> GetMultiUiversion();
        Task<ProjectConfiguration> GetProject();

        Task<List<ProjectConfiguration>> GetProjectList();
        Task<bool> SaveValveSensorLatLong(MultiLatLongValveSensor model);
        Task<bool> SaveNodeLatLong(MultiNodeLatLong model);
    }

    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private IConfiguration _iconnectionstring;
        private string connectionstring;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IConfiguration iconnectionstring,
            ILogger<ProjectService> logger,
            MainDBContext mainDBContext, IMapper mapper)
        {
            _iconnectionstring = iconnectionstring;
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            connectionstring = _iconnectionstring["ConnectionStrings:DefaultConnection"];
            _logger = logger;
        }

        /// <summary>
        /// get project
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectConfiguration> GetProject()
        {
            try
            {
                var project = await _mainDBContext.ProjectConfiguration.FirstAsync();
                // ProjectConfiguration projectViewModel = _mapper.Map<ProjectConfiguration>(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetProject)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<Gateway>> GetGateways()
        {
            try
            {
                var project = await _mainDBContext.Gateway.ToListAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetGateways)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<MultiNodeDashbordData>> GetMultiNodeLatLong()
        {
            try
            {
                List<MultiNodeDashbordData> multiNodeDashbordDatas = new List<MultiNodeDashbordData>();
                var MultiNodeLatLongLst = _mainDBContext.MultiNodeLatLong.AsQueryable().ToList();

                var Analog05vsensorList = _mainDBContext.Analog05vsensor.AsQueryable().ToList();
                var Analog420mAsensorList = _mainDBContext.Analog420mAsensor.AsQueryable().ToList();
                var DigitalNoNctypeSensorList = _mainDBContext.DigitalNoNctypeSensor.AsQueryable().ToList();
                var DigitalCounterTypeSensorList = _mainDBContext.DigitalCounterTypeSensor.AsQueryable().ToList();
                var WaterMeterSensorSettingList = _mainDBContext.WaterMeterSensorSetting.AsQueryable().ToList();
                var VrtList = _mainDBContext.Vrtsetting.AsQueryable().ToList();
                List<VrtsettingViewModel> vrtsettingViewModels = _mapper.Map<List<VrtsettingViewModel>>(VrtList);
                List<MultiValveEvent> valveevents = _mainDBContext.MultiValveEvent.Where(x => x.AddedDateTime >= DateTime.Now.AddHours(-12)).OrderByDescending(x => x.AddedDateTime).AsQueryable().ToList();
                List<MultiSensorEvent> sensorAlarmData = _mainDBContext.MultiSensorEvent.Where(x => x.AddedDateTime >= DateTime.Now.AddHours(-12)).OrderByDescending(x => x.AddedDateTime).AsQueryable().ToList();
                List<MultiLatLongValveSensor> multiLatLongValveSensors = new List<MultiLatLongValveSensor>();
                vrtsettingViewModels.ForEach(async item =>
                {
                    var state = valveevents.
                    Where(x => x.NetworkNo == item.GwSrn
                    && x.NodeNo == item.NodeId
                    && x.ValveNo == item.ValveNo
                    ).OrderByDescending(x => x.AddedDateTime).FirstOrDefault();
                    if (state == null)
                    {
                        item.ValveStatus = 4;
                    }
                    else
                    {
                        item.ValveStatus = (int)state.CurrentState;
                    }
                });

                MultiNodeLatLongLst.ForEach(async item =>
                {
                    MultiNodeDashbordData multiNodeDashbordData = new MultiNodeDashbordData();
                    multiNodeDashbordData.NodeId = item.NodeId;
                    multiNodeDashbordData.NodeNo = item.NodeId & 1023;
                    multiNodeDashbordData.NetworkNo = item.NodeId >> 10;
                    multiNodeDashbordData.Latitude = item.ManualLatitude;
                    multiNodeDashbordData.Longitude = item.ManualLongitude;
                    multiNodeDashbordData.VrtList = vrtsettingViewModels.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).ToList();
                    //multiNodeDashbordData.VrtList = VrtList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.NetworkNo == multiNodeDashbordData.NetworkNo).ToList();
                    foreach (var itemV in multiNodeDashbordData.VrtList)
                    {
                        if (!_mainDBContext.MultiLatLongValveSensor.Any(x => x.Vstype == 1 && x.NodeNo == multiNodeDashbordData.NodeNo && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemV.ValveNo))
                        {
                            MultiLatLongValveSensor multiLatLong = new MultiLatLongValveSensor();
                            multiLatLong.NodeId = item.NodeId;
                            multiLatLong.NodeNo = multiNodeDashbordData.NodeNo;
                            multiLatLong.NetworkNo = multiNodeDashbordData.NetworkNo;
                            multiLatLong.ElementNo = itemV.ValveNo;
                            multiLatLong.Vstype = 1;
                            multiLatLong.Latitude = item.Latitude + (decimal)0.0001;
                            if (itemV.ValveNo == 1) { multiLatLong.Longitude = item.Longitude - (decimal)0.00004; }
                            else if (itemV.ValveNo == 2) { multiLatLong.Longitude = item.Longitude - (decimal)0; }
                            else if (itemV.ValveNo == 3) { multiLatLong.Longitude = item.Longitude + (decimal)0.00008; }
                            else if (itemV.ValveNo == 4) { multiLatLong.Longitude = item.Longitude + (decimal)0.00004; }
                            else { multiLatLong.Longitude = item.Longitude; }

                            multiLatLongValveSensors.Add(multiLatLong);

                            itemV.Latitude = multiLatLong.Latitude;
                            itemV.Longitude = multiLatLong.Longitude;
                        }
                        else
                        {
                            var valveLatlong = _mainDBContext.MultiLatLongValveSensor.Where(x => x.Vstype == 1 && x.NodeNo == itemV.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemV.ValveNo).FirstOrDefault();
                            itemV.Latitude = valveLatlong.Latitude;
                            itemV.Longitude = valveLatlong.Longitude;
                        }
                    }
                    List<SensorListModel> ssList = new List<SensorListModel>();
                    var ss1 = Analog05vsensorList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).Select(x => new SensorListModel
                    {
                        Id = x.Id,
                        GwSrn = x.GwSrn,
                        NodeId = x.NodeId,
                        NodePorductId = x.NodePorductId,
                        ProductType = x.ProductType,
                        SsNo = x.SsNo,
                        TagName = x.TagName,
                        SensorType = 3
                    }).ToList();
                    foreach (var itemSS2 in ss1)
                    {
                        var val = sensorAlarmData.Where(x => x.Sstype == itemSS2.SensorType && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.NodeNo == multiNodeDashbordData.NodeNo).FirstOrDefault();
                        if (val != null)
                        {
                            itemSS2.SensorValue = val.SensorValue;
                        }
                        else
                        {
                            itemSS2.SensorValue = 0;
                        }

                        if (!_mainDBContext.MultiLatLongValveSensor.Any(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo))
                        {
                            MultiLatLongValveSensor multiLatLong = new MultiLatLongValveSensor();
                            multiLatLong.NodeId = item.NodeId;
                            multiLatLong.NodeNo = multiNodeDashbordData.NodeNo;
                            multiLatLong.NetworkNo = multiNodeDashbordData.NetworkNo;
                            multiLatLong.ElementNo = itemSS2.SsNo;
                            multiLatLong.Vstype = 2;
                            multiLatLong.Latitude = item.Latitude - (decimal)0.0001;
                            if (itemSS2.SsNo == 1) { multiLatLong.Longitude = item.Longitude - (decimal)0.00004; }
                            else if (itemSS2.SsNo == 2) { multiLatLong.Longitude = item.Longitude - (decimal)0; }
                            else if (itemSS2.SsNo == 3) { multiLatLong.Longitude = item.Longitude + (decimal)0.00008; }
                            else if (itemSS2.SsNo == 4) { multiLatLong.Longitude = item.Longitude + (decimal)0.00004; }
                            else { multiLatLong.Longitude = item.Longitude; }

                            multiLatLongValveSensors.Add(multiLatLong);

                            itemSS2.Latitude = multiLatLong.Latitude;
                            itemSS2.Longitude = multiLatLong.Longitude;
                        }
                        else
                        {
                            var valveLatlong = _mainDBContext.MultiLatLongValveSensor.Where(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo).FirstOrDefault();
                            itemSS2.Latitude = valveLatlong.Latitude;
                            itemSS2.Longitude = valveLatlong.Longitude;
                        }

                    }

                    var ss2 = Analog420mAsensorList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).Select(x => new SensorListModel
                    {
                        Id = x.Id,
                        GwSrn = x.GwSrn,
                        NodeId = x.NodeId,
                        NodePorductId = x.NodePorductId,
                        ProductType = x.ProductType,
                        SsNo = x.SsNo,
                        TagName = x.TagName,
                        ScaleMin = x.ScaleMin,
                        ScaleMax = x.ScaleMax,
                        SensorType = 2
                    }).ToList();

                    foreach (var itemSS2 in ss2)
                    {
                        decimal mslope = 0;
                        decimal constantVal = 0;
                        decimal actualVal = 0;
                        var val = sensorAlarmData.Where(x => x.Ssno == itemSS2.SsNo && x.Sstype == itemSS2.SensorType && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.NodeNo == multiNodeDashbordData.NodeNo).FirstOrDefault();
                        if (val != null)
                        {

                            decimal scaleMinMaxDiff = (decimal)(itemSS2.ScaleMax - itemSS2.ScaleMin);
                            if (scaleMinMaxDiff > 0)
                            {
                                mslope = 100 / scaleMinMaxDiff;
                                constantVal = (decimal)itemSS2.ScaleMin * mslope;
                                actualVal = ((decimal)(val.SensorValue + constantVal) / mslope);
                            }
                            itemSS2.SensorValue = actualVal;
                        }
                        else
                        {
                            itemSS2.SensorValue = 0;
                        }

                        if (!_mainDBContext.MultiLatLongValveSensor.Any(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo))
                        {
                            MultiLatLongValveSensor multiLatLong = new MultiLatLongValveSensor();
                            multiLatLong.NodeId = item.NodeId;
                            multiLatLong.NodeNo = multiNodeDashbordData.NodeNo;
                            multiLatLong.NetworkNo = multiNodeDashbordData.NetworkNo;
                            multiLatLong.ElementNo = itemSS2.SsNo;
                            multiLatLong.Vstype = 2;
                            multiLatLong.Latitude = item.Latitude - (decimal)0.0001;
                            if (itemSS2.SsNo == 1) { multiLatLong.Longitude = item.Longitude - (decimal)0.00004; }
                            else if (itemSS2.SsNo == 2) { multiLatLong.Longitude = item.Longitude - (decimal)0; }
                            else if (itemSS2.SsNo == 3) { multiLatLong.Longitude = item.Longitude + (decimal)0.00008; }
                            else if (itemSS2.SsNo == 4) { multiLatLong.Longitude = item.Longitude + (decimal)0.00004; }
                            else { multiLatLong.Longitude = item.Longitude; }

                            multiLatLongValveSensors.Add(multiLatLong);

                            itemSS2.Latitude = multiLatLong.Latitude;
                            itemSS2.Longitude = multiLatLong.Longitude;
                        }
                        else
                        {
                            var valveLatlong = _mainDBContext.MultiLatLongValveSensor.Where(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo).FirstOrDefault();
                            itemSS2.Latitude = valveLatlong.Latitude;
                            itemSS2.Longitude = valveLatlong.Longitude;
                        }
                    }


                    var ss3 = DigitalNoNctypeSensorList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).Select(x => new SensorListModel
                    {
                        Id = x.Id,
                        GwSrn = x.GwSrn,
                        NodeId = x.NodeId,
                        NodePorductId = x.NodePorductId,
                        ProductType = x.ProductType,
                        SsNo = x.SsNo,
                        TagName = x.TagName,
                        SensorType = 4
                    }).ToList();
                    foreach (var itemSS2 in ss3)
                    {
                        var val = sensorAlarmData.Where(x => x.Sstype == itemSS2.SensorType && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.NodeNo == multiNodeDashbordData.NodeNo).FirstOrDefault();
                        if (val != null)
                        {
                            itemSS2.SensorValue = val.SensorValue;
                        }
                        else
                        {
                            itemSS2.SensorValue = 0;
                        }

                        if (!_mainDBContext.MultiLatLongValveSensor.Any(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo))
                        {
                            MultiLatLongValveSensor multiLatLong = new MultiLatLongValveSensor();
                            multiLatLong.NodeId = item.NodeId;
                            multiLatLong.NodeNo = multiNodeDashbordData.NodeNo;
                            multiLatLong.NetworkNo = multiNodeDashbordData.NetworkNo;
                            multiLatLong.ElementNo = itemSS2.SsNo;
                            multiLatLong.Vstype = 2;
                            multiLatLong.Latitude = item.Latitude - (decimal)0.0001;
                            if (itemSS2.SsNo == 1) { multiLatLong.Longitude = item.Longitude - (decimal)0.00004; }
                            else if (itemSS2.SsNo == 2) { multiLatLong.Longitude = item.Longitude - (decimal)0; }
                            else if (itemSS2.SsNo == 3) { multiLatLong.Longitude = item.Longitude + (decimal)0.00008; }
                            else if (itemSS2.SsNo == 4) { multiLatLong.Longitude = item.Longitude + (decimal)0.00004; }
                            else { multiLatLong.Longitude = item.Longitude; }

                            multiLatLongValveSensors.Add(multiLatLong);

                            itemSS2.Latitude = multiLatLong.Latitude;
                            itemSS2.Longitude = multiLatLong.Longitude;
                        }
                        else
                        {
                            var valveLatlong = _mainDBContext.MultiLatLongValveSensor.Where(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo).FirstOrDefault();
                            itemSS2.Latitude = valveLatlong.Latitude;
                            itemSS2.Longitude = valveLatlong.Longitude;
                        }
                    }

                    var ss4 = DigitalCounterTypeSensorList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).Select(x => new SensorListModel
                    {
                        Id = x.Id,
                        GwSrn = x.GwSrn,
                        NodeId = x.NodeId,
                        NodePorductId = x.NodePorductId,
                        ProductType = x.ProductType,
                        SsNo = x.SsNo,
                        TagName = x.TagName,
                        SensorType = 5
                    }).ToList();

                    var ss5 = WaterMeterSensorSettingList.Where(x => x.NodeId == multiNodeDashbordData.NodeNo && x.GwSrn == multiNodeDashbordData.NetworkNo).Select(x => new SensorListModel
                    {
                        Id = x.Id,
                        GwSrn = x.GwSrn,
                        NodeId = x.NodeId,
                        NodePorductId = x.NodePorductId,
                        ProductType = x.ProductType,
                        SsNo = x.SsNo,
                        TagName = x.TagName,
                        SensorType = 37,
                        PulseValue = x.PulseValue

                    }).ToList();
                    foreach (var itemSS2 in ss5)
                    {
                        var val = sensorAlarmData.Where(x => x.Ssno == itemSS2.SsNo && x.Sstype == itemSS2.SensorType && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.NodeNo == multiNodeDashbordData.NodeNo).FirstOrDefault();
                        if (val != null)
                        {
                            itemSS2.SensorValue = val.Frequency * itemSS2.PulseValue;
                        }
                        else
                        {
                            itemSS2.SensorValue = 0;
                        }

                        if (!_mainDBContext.MultiLatLongValveSensor.Any(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo))
                        {
                            MultiLatLongValveSensor multiLatLong = new MultiLatLongValveSensor();
                            multiLatLong.NodeId = item.NodeId;
                            multiLatLong.NodeNo = multiNodeDashbordData.NodeNo;
                            multiLatLong.NetworkNo = multiNodeDashbordData.NetworkNo;
                            multiLatLong.ElementNo = itemSS2.SsNo;
                            multiLatLong.Vstype = 2;
                            multiLatLong.Latitude = item.Latitude - (decimal)0.0001;
                            if (itemSS2.SsNo == 1) { multiLatLong.Longitude = item.Longitude - (decimal)0.00004; }
                            else if (itemSS2.SsNo == 2) { multiLatLong.Longitude = item.Longitude - (decimal)0; }
                            else if (itemSS2.SsNo == 3) { multiLatLong.Longitude = item.Longitude + (decimal)0.00008; }
                            else if (itemSS2.SsNo == 4) { multiLatLong.Longitude = item.Longitude + (decimal)0.00004; }
                            else { multiLatLong.Longitude = item.Longitude; }

                            multiLatLongValveSensors.Add(multiLatLong);

                            itemSS2.Latitude = multiLatLong.Latitude;
                            itemSS2.Longitude = multiLatLong.Longitude;
                        }
                        else
                        {
                            var valveLatlong = _mainDBContext.MultiLatLongValveSensor.Where(x => x.Vstype == 2 && x.NodeNo == itemSS2.NodeId && x.NetworkNo == multiNodeDashbordData.NetworkNo && x.ElementNo == itemSS2.SsNo).FirstOrDefault();
                            itemSS2.Latitude = valveLatlong.Latitude;
                            itemSS2.Longitude = valveLatlong.Longitude;
                        }
                    }

                    ssList.AddRange(ss1);
                    ssList.AddRange(ss2);
                    ssList.AddRange(ss3);
                    ssList.AddRange(ss4);
                    ssList.AddRange(ss5);

                    multiNodeDashbordData.SensorList = ssList;

                    multiNodeDashbordDatas.Add(multiNodeDashbordData);
                });

                _mainDBContext.MultiLatLongValveSensor.AddRange(multiLatLongValveSensors);
                _mainDBContext.SaveChanges();
                //foreach (var item in MultiNodeLatLongLst)
                //{

                //}
                return multiNodeDashbordDatas;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetMultiNodeLatLong)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get project
        /// </summary>
        /// <returns></returns>
        public async Task<List<MultiNetworkRtu>> GetMultiNetworkRtu()
        {
            try
            {
                var project = await _mainDBContext.MultiNetworkRtu.Select(x => new MultiNetworkRtu
                {
                    NetworkNo = x.NetworkNo,
                    RtuId = x.RtuId,
                    NodeNo = x.NodeNo
                }).Distinct().OrderBy(x => x.NetworkNo).ThenBy(x => x.RtuId).ToListAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetMultiNetworkRtu)}]{ex}");
                throw ex;
            }
        }
        public async Task<bool> CheckProjectSerialNo(int id, int srnno)
        {
            try
            {
                if (_mainDBContext.Gateway.Any(x => x.SerialNo == srnno && x.Id != id))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(CheckProjectSerialNo)}]{ex}");
                return false;
            }
        }
        public async Task<bool> SaveValveSensorLatLong(MultiLatLongValveSensor model)
        {
            try
            {
                var project = await _mainDBContext.MultiLatLongValveSensor.Where(x => x.NodeNo == model.NodeNo && x.NetworkNo == model.NetworkNo && x.ElementNo == model.ElementNo && x.Vstype == model.Vstype).FirstOrDefaultAsync();
                project.Longitude = model.Longitude;
                project.Latitude = model.Latitude;
                _mainDBContext.MultiLatLongValveSensor.Update(project);
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(SaveValveSensorLatLong)}]{ex}");
                return false;
            }
        }

        public async Task<bool> SaveNodeLatLong(MultiNodeLatLong model)
        {
            try
            {
                var project = await _mainDBContext.MultiNodeLatLong.Where(x => x.NodeId == model.NodeId&& x.NetworkNo == model.NetworkNo).FirstOrDefaultAsync();
                project.ManualLatitude = model.ManualLatitude;
                project.ManualLongitude = model.ManualLongitude;
                _mainDBContext.MultiNodeLatLong.Update(project);
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(SaveNodeLatLong)}]{ex}");
                return false;
            }
        }

        public async Task<bool> UpdateGateway(Gateway model)
        {
            try
            {
                var project = await _mainDBContext.Gateway.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                project.SerialNo = model.SerialNo;
                project.Longitude = model.Longitude;
                project.Latitude = model.Latitude;
                project.IsActive = model.IsActive;
                project.TagName = model.TagName;
                _mainDBContext.Gateway.Update(project);
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetUpdateIds)}]{ex}");
                return false;
            }
        }

        public async Task<List<UpdateIdsRequired>> GetUpdateIds()
        {
            try
            {
                var project = await _mainDBContext.UpdateIdsRequired.Select(x => new UpdateIdsRequired { NetworkNo = x.NetworkNo, NodeId = x.NodeId }).Distinct().ToListAsync();
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetUpdateIds)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get project
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProjectConfiguration>> GetProjectList()
        {
            try
            {
                var project = await _mainDBContext.ProjectConfiguration.ToListAsync();
                //List<ProjectViewModel> projectViewModel = _mapper.Map<List<ProjectViewModel>>(project);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ProjectService)}.{nameof(GetProjectList)}]{ex}");
                throw ex;
            }
        }


        /// <summary>
        /// get users having privilege only seven and logout
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="flag"></param>
        /// <param name="TimeZoneDateTime"></param>
        /// <returns></returns>
        public Task<IEnumerable> GetUsersHavingPrivilageOnlySevenAndLogout(string userId, int flag, DateTime TimeZoneDateTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get project for timezon
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectViewModel> GetProjectForTimeZone()
        {
            Project projectInfo = await _mainDBContext.Project.FirstOrDefaultAsync();
            ProjectViewModel projectViewModel = _mapper.Map<ProjectViewModel>(projectInfo);
            return projectViewModel;
        }


        public async Task<MultiUiversion> GetMultiUiversion()
        {
            MultiUiversion model = await _mainDBContext.MultiUiversion.FirstOrDefaultAsync();
            return model;
        }

    }
}
