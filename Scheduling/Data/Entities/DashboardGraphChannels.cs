using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardGraphChannels
    {
        [Key]
        public int Id { get; set; }
        public int? IdofGraphData { get; set; }
        public string ChannelId { get; set; }
        public string UserId { get; set; }
    }
}
