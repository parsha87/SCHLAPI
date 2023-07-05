using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ProjectViewModel
    {

        public int PrjId { get; set; }
        public int PrjTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [Column("NoOfTotalOPSubscribed")]
        public int? NoOfTotalOpsubscribed { get; set; }
        public int? CntryId { get; set; }
        [StringLength(500)]
        public string TimeZone { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MaxPermissibleTotalFlowRate { get; set; }
        public int? UnitsId { get; set; }
        public int? NoOfTotalZones { get; set; }
        public int? NoOfTotalNetwork { get; set; }
        public int? TimeZoneOffsetMinutes { get; set; }
        [Column("MaxNoOfRTUPerNetwork")]
        public int? MaxNoOfRtuperNetwork { get; set; }
        public int? NoOfTotalBlocks { get; set; }
        [StringLength(500)]
        public string Offset { get; set; }

    }

}
