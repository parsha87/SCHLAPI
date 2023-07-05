using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardGraphData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(100)]
        public string GraphAxis { get; set; }
        [StringLength(100)]
        public string GraphType { get; set; }
        public int? GraphScreen { get; set; }
        [StringLength(100)]
        public string GraphName { get; set; }
    }
}
