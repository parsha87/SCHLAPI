using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Scheduling.Services
{

    public interface IZoneService
    {
        Task<List<ZoneShortViewModel>> GetShortZones();
        Task<List<ZoneViewModel>> GetZoneByTypeAndId(string charType, int id);
    }

    public class ZoneService : IZoneService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<ZoneService> _logger;

        public ZoneService(ILogger<ZoneService> logger,
                MainDBContext mainDBContext,
                IMapper mapper
            )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
        }

        public async Task<List<ZoneShortViewModel>> GetShortZones()
        {
            try
            {
                var zones = await _mainDBContext.Zone.Select(x=> new ZoneShortViewModel { 
                        ZoneId = x.ZoneId,
                        ZoneName = x.Name
                    }).ToListAsync();
                return zones;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ZoneService)}.{nameof(GetShortZones)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get zones by id, for example networkId (N) or zoneId (Z) or all zones (All),
        /// Note: Type ZN is not included (sp: GetZone)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="charType"></param>
        /// <returns>List<ZoneViewModel></returns>
        public async Task<List<ZoneViewModel>> GetZoneByTypeAndId(string charType, int id)
        {
            try
            {
                List<ZoneViewModel> models = new List<ZoneViewModel>();
                if(charType.Equals("Z", StringComparison.OrdinalIgnoreCase))
                {
                    models = _mapper.Map<List<ZoneViewModel>>(await _mainDBContext.Zone
                        .Where(x => x.ZoneId == id).OrderBy(x=>x.Name).ToListAsync());
                }
                else if(charType.Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    var zones = _mainDBContext.Zone
                        .Join(_mainDBContext.Block.Where(x => x.NetworkId == id),
                        z => z.ZoneId,
                        b => b.ZoneId,
                        (z, b) => new { z });
                    models = _mapper.Map<List<ZoneViewModel>>(await zones.OrderBy(x => x.z.Name).ToListAsync());
                }
                else
                {
                    models = _mapper.Map<List<ZoneViewModel>>
                        (await _mainDBContext.Zone.OrderBy(x => x.Name).ToListAsync());
                }
                return models;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(ZoneService)}.{nameof(GetZoneByTypeAndId)}]{ex}");
                throw ex;
            }
        }
    }
}
