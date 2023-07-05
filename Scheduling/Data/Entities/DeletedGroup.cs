using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedGroup
    {
        [Key]
        public int Id { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        [Column("OPGroupTypeId")]
        public int OpgroupTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string GroupName { get; set; }
        [Column("GrpNoinNWZone")]
        public int? GrpNoinNwzone { get; set; }
    }
}
