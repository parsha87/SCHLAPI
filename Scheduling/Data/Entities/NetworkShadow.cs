using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("Network_Shadow")]
    public partial class NetworkShadow
    {
        [Key]
        public int Id { get; set; }
        public int NetworkId { get; set; }
        public int PrjId { get; set; }
        [Column("BSTId1")]
        public int Bstid1 { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Column("BSTId2")]
        public int? Bstid2 { get; set; }
        [Column("NoOfRTU")]
        public int? NoOfRtu { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int? NetworkNo { get; set; }
        [Column("UseTemplateForRTU")]
        public bool? UseTemplateForRtu { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        [Column("Shadow_SDT", TypeName = "datetime")]
        public DateTime ShadowSdt { get; set; }
        [Column("Shadow_EDT", TypeName = "datetime")]
        public DateTime ShadowEdt { get; set; }
        [Required]
        [StringLength(1)]
        public string ActionType { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
    }
}
