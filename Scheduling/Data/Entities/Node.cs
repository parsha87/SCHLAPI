using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Node
    {
        [Key]
        public int Id { get; set; }
        public int? NodeNo { get; set; }
        [StringLength(200)]
        public string NodeName { get; set; }
        public int? NetworkId { get; set; }
        public int? NetworkNo { get; set; }
        public int? RtuId { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? ProductTypeId { get; set; }
        public bool? IsAddonCard { get; set; }
    }
}
