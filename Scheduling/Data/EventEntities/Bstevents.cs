using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("BSTEvents")]
    public partial class Bstevents
    {
        public Bstevents()
        {
            BsteventsConfig = new HashSet<BsteventsConfig>();
        }

        [Key]
        [Column("BSTEventsId")]
        public int BsteventsId { get; set; }
        public int? NetworkId { get; set; }
        [Column("BSTId")]
        public int? Bstid { get; set; }
        [Column("BSTBatVolt")]
        public int? BstbatVolt { get; set; }
        [Column("BSTBatCharging")]
        public bool? BstbatCharging { get; set; }
        [Column("BSTGsmStrength")]
        public int? BstgsmStrength { get; set; }
        [Column("CMDNo")]
        [StringLength(50)]
        public string Cmdno { get; set; }
        [Column("CMDType")]
        [StringLength(2)]
        public string Cmdtype { get; set; }

        [InverseProperty("Bstevent")]
        public virtual ICollection<BsteventsConfig> BsteventsConfig { get; set; }
    }
}
