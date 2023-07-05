using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AdminPrivileges
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int? Zone { get; set; }
        public bool? AllZones { get; set; }
        public int? Block { get; set; }
        public bool? AllBlocks { get; set; }
        public int? SubBlock { get; set; }
        public bool? AllSubBlocks { get; set; }
        public int? Network { get; set; }
        public bool? AllNetworks { get; set; }
        public bool? Crop { get; set; }
    }
}
