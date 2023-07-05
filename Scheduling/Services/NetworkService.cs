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
    public interface INetworkService
    {
        Task<List<NetworkShortViewModel>> GetShortNetworks(int zoneId, string priviledges, string userId);
        Task<List<Network>> GetNetworks();

        Task<List<AvailableNetworkObject>> GetAvailableNetworks();

    }

    public class NetworkService : INetworkService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<NetworkService> _logger;

        public NetworkService(ILogger<NetworkService> logger,
                MainDBContext mainDBContext,
                IMapper mapper
            )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
        }
        public async Task<List<Network>> GetNetworks()
        {

            try
            {
                List<Network> networks = await _mainDBContext.Network.ToListAsync();
                return networks;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(NetworkService)}.{nameof(GetShortNetworks)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<AvailableNetworkObject>> GetAvailableNetworks()
        {

            try
            {
                List<Network> networks = await _mainDBContext.Network.ToListAsync();

                var aq = await _mainDBContext.DeletedNetwork.Where(cc => cc.Reused == false).Select(c => new { DisplayText = c.NetworkNo.ToString(), Value = c.NetworkNo }).OrderBy(n => n.Value).ToListAsync();
                var nomenclature = await _mainDBContext.Nomenclature.FirstOrDefaultAsync(n => n.Nomenclature1 == "Network");
                var AvailableNetworkLst = new List<AvailableNetworkObject>();
                AvailableNetworkLst.Add(new AvailableNetworkObject { DisplayText = "Select", Value = 0 });
                foreach (var item in aq)
                {
                    AvailableNetworkLst.Add(new AvailableNetworkObject { DisplayText = nomenclature.Name + item.DisplayText, Value = Convert.ToInt32(item.Value) });
                }
                return AvailableNetworkLst;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(NetworkService)}.{nameof(GetShortNetworks)}]{ex}");
                throw ex;
            }
        }

        public async Task<List<NetworkShortViewModel>> GetShortNetworks(int zoneId, string priviledges, string userId)
        {
            try
            {
                List<NetworkShortViewModel> networkShortList = new List<NetworkShortViewModel>();
                // get networks in zone
                networkShortList = await _mainDBContext.Network
                        .Join(_mainDBContext.ZoneInNetwork,
                            network => network.NetworkId,
                            zoneInNetwork => zoneInNetwork.NetworkId,
                            (network, zoneInNetwork) => new { network, zoneInNetwork })
                        .Where(x => x.zoneInNetwork.ZoneId == zoneId)
                        .Select(x =>
                            new NetworkShortViewModel
                            {
                                NetworkId = x.network.NetworkId,
                                NetworkName = x.network.Name,
                                NetworkNo = (int)x.network.NetworkNo
                            }).ToListAsync();

                // filter data for non admin users
                if (priviledges != "All")
                {
                    // get networks assigned to users for selected zone
                    networkShortList = networkShortList.Join(_mainDBContext.AdminPrivileges,
                        network => network.NetworkId,
                        adminPrivileges => adminPrivileges.Network,
                        (network, adminPrivileges) => new { network, adminPrivileges })
                        .Where(x => x.adminPrivileges.UserId == userId)
                        .Select(x =>
                            new NetworkShortViewModel
                            {
                                NetworkId = x.network.NetworkId,
                                NetworkName = x.network.NetworkName,
                                NetworkNo = (int)x.network.NetworkNo
                            }).ToList();
                }
                NetworkShortViewModel networkShortViewModel = new NetworkShortViewModel();
                networkShortViewModel.NetworkId = 0;
                networkShortViewModel.NetworkName = "All";
                networkShortViewModel.NetworkNo = 0;
                networkShortList.Add(networkShortViewModel);
                return networkShortList.OrderBy(x => x.NetworkNo).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(NetworkService)}.{nameof(GetShortNetworks)}]{ex}");
                throw ex;
            }
        }
    }
}
