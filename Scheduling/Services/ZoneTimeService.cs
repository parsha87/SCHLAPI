using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IZoneTimeService
    {
        Task<DateTime> TimeZoneDateTime();
        string TimeZoneBaseUtcOffset(string TimeZoneIdentifier);
    }

    public class ZoneTimeService : IZoneTimeService
    {
        private readonly IMapper _mapper;
        private IProjectService _projectService;
        private readonly ILogger<ZoneTimeService> _logger;

        public ZoneTimeService(IProjectService projectService, 
            ILogger<ZoneTimeService> logger, 
            MainDBContext mainDBContext, 
            IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _projectService = projectService;
        }

        /// <summary>
        /// Get Time Zone Date Time
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime> TimeZoneDateTime()
        {
            DateTime DateTimeFromZone;
            ProjectViewModel projectInfo = await _projectService.GetProjectForTimeZone();
            //var TimeZonelst = jdc.Timezones.Where(t => t.Id == ProjectInfo.TimeZoneId).FirstOrDefault();
            TimeZoneInfo Standard_Time_As_Per_Country = TimeZoneInfo.FindSystemTimeZoneById(projectInfo.TimeZone);
            DateTimeFromZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Standard_Time_As_Per_Country);
            //Return Date Time
            return DateTimeFromZone;
        }

        /// <summary>
        /// Get Base UTC Offset in Minutes
        /// </summary>
        /// <param name="TimeZoneIdentifier"></param>
        /// <returns></returns>
        public string TimeZoneBaseUtcOffset(string TimeZoneIdentifier)
        {
            TimeZoneInfo Standard_Time_As_Per_Country = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneIdentifier);
            var BaseUtcOffset = Standard_Time_As_Per_Country.BaseUtcOffset.TotalMinutes;
            return BaseUtcOffset.ToString();

        }
    }
}
