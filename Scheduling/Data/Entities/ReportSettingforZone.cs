using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ReportSettingforZone
    {
        [Key]
        public int Id { get; set; }
        public int? ZoneId { get; set; }
        [StringLength(500)]
        public string BlockId { get; set; }
    }
}
