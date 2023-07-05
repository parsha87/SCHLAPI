using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardGaugeSetting
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? GaugeChannelId1 { get; set; }
        public int? GaugeChannelId2 { get; set; }
        public int? GaugeChannelId3 { get; set; }
        public int? GaugeChannelId4 { get; set; }
    }
}
