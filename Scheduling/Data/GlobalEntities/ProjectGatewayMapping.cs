using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.GlobalEntities
{
    public partial class ProjectGatewayMapping
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        [StringLength(500)]
        public string ProjectName { get; set; }
        public int? GatewayId { get; set; }
        public int? GatewayNo { get; set; }
        public int? GatewaySrNo { get; set; }
        public int? IsUsed { get; set; }
    }
}
