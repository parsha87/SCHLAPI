using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IGroupService
    {
        Task<GroupDetailsViewModel> GetById(int Id);

        Task<bool> Delete(int Id);
        bool IfGroupExists(string GroupName);

        Task<GroupDetailsViewModel> AddGroup(GroupDetailsViewModel model);
        Task<GroupDetailsViewModel> EditGroup(GroupDetailsViewModel model);
        Task<List<MultiGroupMaster>> GetGroupMaster();
        Task<List<MultiGroupData>> GetGroupData();

        Task<List<MultiGroupData>> GetSensorsForGroup();

    }
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<GroupService> _logger;

        public GroupService(ILogger<GroupService> logger,
              MainDBContext mainDBContext,
              IMapper mapper,
              IZoneTimeService zoneTimeService
          )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
        }


        public bool IfGroupExists(string GroupName)
        {
            try
            {
                if (_mainDBContext.MultiGroupMaster.Any(x => x.GroupName == GroupName))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(IfGroupExists) + "]" + ex);
                throw ex;
            }
        }
        public async Task<bool> Delete(int Id)
        {
            try
            {
                MultiGroupMaster multiGroupMaster = _mainDBContext.MultiGroupMaster.Where(x => x.Id == Id).FirstOrDefault();
                List<MultiGroupData> multiGroupDatas = _mainDBContext.MultiGroupData.Where(x => x.GroupId == Id).ToList();
                _mainDBContext.Remove(multiGroupMaster);
                _mainDBContext.SaveChanges();

                _mainDBContext.RemoveRange(multiGroupDatas);
                _mainDBContext.SaveChanges();

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(Delete) + "]" + ex);
                throw ex;
            }
        }

        public async Task<GroupDetailsViewModel> GetById(int Id)
        {
            try
            {
                MultiGroupMaster multiGroupMaster = _mainDBContext.MultiGroupMaster.Where(x => x.Id == Id).FirstOrDefault();
                List<MultiGroupData> multiGroupDatas = _mainDBContext.MultiGroupData.Where(x => x.GroupId == Id).ToList();

                GroupDetailsViewModel groupDetailsViewModel = new GroupDetailsViewModel();
                groupDetailsViewModel.MultiGroupMaster = _mapper.Map<MultiGroupMasterViewModel>(multiGroupMaster);
                groupDetailsViewModel.MultiGroupData = _mapper.Map<List<MultiGroupDataViewModel>>(multiGroupDatas);
                return groupDetailsViewModel;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(GetById) + "]" + ex);
                throw ex;
            }
        }
        public async Task<GroupDetailsViewModel> AddGroup(GroupDetailsViewModel model)
        {
            try
            {
                MultiGroupMaster multiGroupMaster = _mapper.Map<MultiGroupMaster>(model.MultiGroupMaster);
                _mainDBContext.MultiGroupMaster.Add(multiGroupMaster);
                _mainDBContext.SaveChanges();
                foreach (var item in model.MultiGroupData)
                {
                    item.GroupId = multiGroupMaster.Id;
                }
                List<MultiGroupData> multiGroupData = _mapper.Map<List<MultiGroupData>>(model.MultiGroupData);
                _mainDBContext.MultiGroupData.AddRange(multiGroupData);
                _mainDBContext.SaveChanges();
                return model;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(AddGroup) + "]" + ex);
                throw ex;
            }
        }

        public async Task<GroupDetailsViewModel> EditGroup(GroupDetailsViewModel model)
        {
            try
            {

                MultiGroupMaster multiGroupMaster = _mainDBContext.MultiGroupMaster.Where(x => x.Id == model.MultiGroupMaster.Id).FirstOrDefault();
                multiGroupMaster.Tagname = model.MultiGroupMaster.Tagname;
                _mainDBContext.SaveChanges();

                var itemsToDelete = _mainDBContext.MultiGroupData.Where(x => x.GroupId == model.MultiGroupMaster.Id).ToList();
                _mainDBContext.MultiGroupData.RemoveRange(itemsToDelete);
                _mainDBContext.SaveChanges();

                model.MultiGroupData.Where(w => w.GroupId == model.MultiGroupMaster.Id).ToList().ForEach(i => i.Id = 0);
                model.MultiGroupData.Where(w => w.GroupId == 0).ToList().ForEach(i => i.GroupId = model.MultiGroupMaster.Id);
                List<MultiGroupData> multiGroupData = _mapper.Map<List<MultiGroupData>>(model.MultiGroupData);
                _mainDBContext.MultiGroupData.AddRange(multiGroupData);
                _mainDBContext.SaveChanges();
                return model;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(EditGroup) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<MultiGroupData>> GetGroupData()
        {
            try
            {
                var list = _mainDBContext.MultiGroupData.ToList();
                return list;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(GetGroupData) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<MultiGroupData>> GetSensorsForGroup()
        {
            try
            {
                var list = _mainDBContext.Analog05vsensor.Select(x => new MultiGroupData
                {
                    Id = 0,
                    GroupId = 0,
                    SensorName = x.SensorName,
                    SensorTagName = x.TagName,
                    ProductType = x.ProductType,
                    Gwsrn = x.GwSrn,
                    NodeProductId = x.NodePorductId,
                    NodeId = x.NodeId,
                    Ssno = x.SsNo,
                    ConfigurationId = x.Id,
                    Priority = 0
                }).ToList();
                list.AddRange(_mainDBContext.Analog420mAsensor.Select(x => new MultiGroupData
                {
                    Id = 0,
                    GroupId = 0,
                    SensorName = x.SensorName,
                    SensorTagName = x.TagName,
                    ProductType = x.ProductType,
                    Gwsrn = x.GwSrn,
                    NodeProductId = x.NodePorductId,
                    NodeId = x.NodeId,
                    Ssno = x.SsNo,
                    ConfigurationId = x.Id,
                    Priority = 0
                }).ToList());
                return list;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(GetSensorsForGroup) + "]" + ex);
                throw ex;
            }
        }

        public async Task<List<MultiGroupMaster>> GetGroupMaster()
        {
            try
            {
                var list = await _mainDBContext.MultiGroupMaster.ToListAsync();
                return list;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("[" + nameof(GroupService) + "." + nameof(GetGroupMaster) + "]" + ex);
                throw ex;
            }
        }
    }
}
