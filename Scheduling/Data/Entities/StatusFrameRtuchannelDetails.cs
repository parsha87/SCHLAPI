using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusFrameRTUChannelDetails")]
    public partial class StatusFrameRtuchannelDetails
    {
        [Key]
        public int PrjId { get; set; }
        [Key]
        public int NetworkId { get; set; }
        [Key]
        [Column("RTUIdInNW")]
        public int RtuidInNw { get; set; }
        [Key]
        [Column("RTUTimeStamp", TypeName = "datetime")]
        public DateTime RtutimeStamp { get; set; }
        [Key]
        public int ChannelId { get; set; }
        [Key]
        public int SlotId { get; set; }
        [Required]
        [StringLength(1)]
        public string AorDorV { get; set; }
        public int AnalogValue { get; set; }
        public bool DigitalStatus { get; set; }
        public int DigitalCounterVal { get; set; }
        public int DigitalFlowVal { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.StatusFrameRtuchannelDetails))]
        public virtual Project Prj { get; set; }
    }
}
