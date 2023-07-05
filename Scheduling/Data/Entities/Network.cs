using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Network
    {
        [Key]
        public int NetworkId { get; set; }
        [Key]
        public int PrjId { get; set; }
        [Column("BSTId1")]
        public int Bstid1 { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Column("BSTId2")]
        public int? Bstid2 { get; set; }
        public int? NoOfZones { get; set; }
        [Column("NoOfRTU")]
        public int? NoOfRtu { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int? NetworkNo { get; set; }
        [Column("UseTemplateForRTU")]
        public bool? UseTemplateForRtu { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        public bool? NetworkLock { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.Network))]
        public virtual Project Prj { get; set; }
    }
}
