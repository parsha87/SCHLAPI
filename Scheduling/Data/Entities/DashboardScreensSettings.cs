using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardScreensSettings
    {
        [Key]
        public int Id { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public int? ScreenId { get; set; }
        public int? ViewId { get; set; }
        public int? PrjId { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(DashboardScreens.DashboardScreensSettingsPrj))]
        public virtual DashboardScreens Prj { get; set; }
        [ForeignKey(nameof(ScreenId))]
        [InverseProperty(nameof(DashboardScreens.DashboardScreensSettingsScreen))]
        public virtual DashboardScreens Screen { get; set; }
        [ForeignKey(nameof(ViewId))]
        [InverseProperty(nameof(DashboardScreens.DashboardScreensSettingsView))]
        public virtual DashboardScreens View { get; set; }
    }
}
