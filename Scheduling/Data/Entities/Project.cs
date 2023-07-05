using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Project
    {
        public Project()
        {
            Block = new HashSet<Block>();
            DummySequence = new HashSet<DummySequence>();
            Equipment = new HashSet<Equipment>();
            Network = new HashSet<Network>();
            NonProgEquipments = new HashSet<NonProgEquipments>();
            ProjectAdmin = new HashSet<ProjectAdmin>();
            ProjectElements = new HashSet<ProjectElements>();
            Rtu = new HashSet<Rtu>();
            Sequence = new HashSet<Sequence>();
            SequenceErrDetails = new HashSet<SequenceErrDetails>();
            SequenceWeeklySchedule = new HashSet<SequenceWeeklySchedule>();
            Slot = new HashSet<Slot>();
            SlotSubblockConnection = new HashSet<SlotSubblockConnection>();
            StatusFrameRtuchannelDetails = new HashSet<StatusFrameRtuchannelDetails>();
            SubBlock = new HashSet<SubBlock>();
            Zone = new HashSet<Zone>();
        }

        [Key]
        public int PrjId { get; set; }
        public int PrjTypeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [Column("NoOfTotalOPSubscribed")]
        public int? NoOfTotalOpsubscribed { get; set; }
        public int? CntryId { get; set; }
        [StringLength(100)]
        public string TimeZone { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MaxPermissibleTotalFlowRate { get; set; }
        public int? UnitsId { get; set; }
        public int? NoOfTotalZones { get; set; }
        public int? NoOfTotalNetwork { get; set; }
        public int? TimeZoneOffsetMinutes { get; set; }
        [Column("MaxNoOfRTUPerNetwork")]
        public int? MaxNoOfRtuperNetwork { get; set; }
        public int? NoOfTotalBlocks { get; set; }
        [StringLength(500)]
        public string Offset { get; set; }

        [ForeignKey(nameof(UnitsId))]
        [InverseProperty("Project")]
        public virtual Units Units { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Block> Block { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<DummySequence> DummySequence { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Equipment> Equipment { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Network> Network { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<NonProgEquipments> NonProgEquipments { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<ProjectAdmin> ProjectAdmin { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<ProjectElements> ProjectElements { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Rtu> Rtu { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Sequence> Sequence { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<SequenceErrDetails> SequenceErrDetails { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<SequenceWeeklySchedule> SequenceWeeklySchedule { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Slot> Slot { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<SlotSubblockConnection> SlotSubblockConnection { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<StatusFrameRtuchannelDetails> StatusFrameRtuchannelDetails { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<SubBlock> SubBlock { get; set; }
        [InverseProperty("Prj")]
        public virtual ICollection<Zone> Zone { get; set; }
    }
}
