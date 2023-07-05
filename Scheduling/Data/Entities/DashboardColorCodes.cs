using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardColorCodes
    {
        public int? ColorId { get; set; }
        [StringLength(15)]
        public string Name { get; set; }
    }
}
