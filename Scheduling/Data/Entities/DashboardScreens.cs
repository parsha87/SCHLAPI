using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardScreens
    {
        public DashboardScreens()
        {
            DashboardScreensSettingsPrj = new HashSet<DashboardScreensSettings>();
            DashboardScreensSettingsScreen = new HashSet<DashboardScreensSettings>();
            DashboardScreensSettingsView = new HashSet<DashboardScreensSettings>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string ScreenName { get; set; }
        public int? ScrId { get; set; }

        [InverseProperty(nameof(DashboardScreensSettings.Prj))]
        public virtual ICollection<DashboardScreensSettings> DashboardScreensSettingsPrj { get; set; }
        [InverseProperty(nameof(DashboardScreensSettings.Screen))]
        public virtual ICollection<DashboardScreensSettings> DashboardScreensSettingsScreen { get; set; }
        [InverseProperty(nameof(DashboardScreensSettings.View))]
        public virtual ICollection<DashboardScreensSettings> DashboardScreensSettingsView { get; set; }
    }
}
