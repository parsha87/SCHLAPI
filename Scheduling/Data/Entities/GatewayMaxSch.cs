using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class GatewayMaxSch
    {
        [Key]
        public int Id { get; set; }
        public int? GatewayNo { get; set; }
        public int? MaxSchUpId { get; set; }
    }
}
