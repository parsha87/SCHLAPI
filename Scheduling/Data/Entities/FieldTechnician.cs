using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FieldTechnician
    {
        [Key]
        public int FieldTechId { get; set; }
        [Required]
        [StringLength(20)]
        public string PhoneNo { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int NetworkId { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [Column("IsAuthorityToUseHHD")]
        public bool? IsAuthorityToUseHhd { get; set; }
        [Column("IsSMSAlertFacility")]
        public bool? IsSmsalertFacility { get; set; }
    }
}
