using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ZoneInNetwork
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkId { get; set; }
        public int? NetworkNo { get; set; }
        public int? ZoneId { get; set; }
    }
}
