using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectConfiguration
    {
        [Key]
        public int Id { get; set; }
        public int? MaxNodeSerialNo { get; set; }
        public int? MaxNodeId { get; set; }
        public int? MaxGwValves { get; set; }
        public int? MaxGwSensors { get; set; }
        public int? MaxNodeValves { get; set; }
        public int? MaxNodeSensors { get; set; }
        public int? MaxMobileNoSize { get; set; }
        public int? MaxSequences { get; set; }
        public int? MaxElementsInSequences { get; set; }
        public int? MaxFilters { get; set; }
        public int? MaxPumps { get; set; }
        public int? MaxFert { get; set; }
        [Column("MaxRTUSCheduleTransferDis")]
        public int? MaxRtuscheduleTransferDis { get; set; }
        [Column("MaxSchOperatedGW")]
        public int? MaxSchOperatedGw { get; set; }
        [Column("MaxGWInProject")]
        public int? MaxGwinProject { get; set; }
        public int? MaxNodePerGw { get; set; }
        public int? MaxNodeInProject { get; set; }
        public int? MaxNetworkInProject { get; set; }
    }
}
