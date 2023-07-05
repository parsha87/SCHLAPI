using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedSchedule
    {
        [Key]
        [Column("DSchId")]
        public int DschId { get; set; }
        public int? SchNo { get; set; }
        public bool? Reused { get; set; }
        public int? ChannelId { get; set; }
        public int? ProgId { get; set; }
    }
}
