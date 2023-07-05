using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("MultiNetworkRTU")]
    public partial class MultiNetworkRtu
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkNo { get; set; }
        public int? RtuId { get; set; }
        public int? NodeNo { get; set; }
        public bool? IsActive { get; set; }
    }
}
