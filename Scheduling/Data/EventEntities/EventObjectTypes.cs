using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class EventObjectTypes
    {
        public EventObjectTypes()
        {
            BsteventsConfig = new HashSet<BsteventsConfig>();
            Events = new HashSet<Events>();
            ManualOverrideMaster = new HashSet<ManualOverrideMaster>();
        }

        [Key]
        public int EventObjTypeId { get; set; }
        [StringLength(50)]
        public string ObjectName { get; set; }

        [InverseProperty("EventObjectType")]
        public virtual ICollection<BsteventsConfig> BsteventsConfig { get; set; }
        [InverseProperty("ObjType")]
        public virtual ICollection<Events> Events { get; set; }
        [InverseProperty("OverrideFor")]
        public virtual ICollection<ManualOverrideMaster> ManualOverrideMaster { get; set; }
    }
}
