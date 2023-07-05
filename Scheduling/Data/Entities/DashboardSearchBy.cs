using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardSearchBy
    {
        public int SearchId { get; set; }
        [Required]
        [StringLength(100)]
        public string SearchBy { get; set; }
        public int ListView { get; set; }
        public int MatrixView { get; set; }
        public int GraphView { get; set; }
        public int TimeLineView { get; set; }
        public int MapView { get; set; }
    }
}
