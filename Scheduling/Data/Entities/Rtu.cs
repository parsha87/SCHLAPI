using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("RTU")]
    public partial class Rtu
    {
        public Rtu()
        {
            Channel = new HashSet<Channel>();
            EquipmentConfigValues = new HashSet<EquipmentConfigValues>();
            Slot = new HashSet<Slot>();
            SlotSubblockConnection = new HashSet<SlotSubblockConnection>();
        }

        [Key]
        [Column("RTUId")]
        public int Rtuid { get; set; }
        public int NetworkId { get; set; }
        [Column("RTUIdInNW")]
        public int? RtuidInNw { get; set; }
        [Column("RTUModelId")]
        public int RtumodelId { get; set; }
        public bool? Active { get; set; }
        public int? PrjId { get; set; }
        [Column("RTUName")]
        [StringLength(100)]
        public string Rtuname { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int? NoOfExpCards { get; set; }
        public int? BlockId { get; set; }
        [StringLength(25)]
        public string PhysicalLocation { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SamplingRate { get; set; }
        public bool? IsProgrammable { get; set; }
        [Column("RFPowerLevel")]
        public int? RfpowerLevel { get; set; }
        [Column("DCLatchPulseDuration")]
        public int? DclatchPulseDuration { get; set; }
        public bool? AuxSupply { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ScanningRate { get; set; }
        [Column("RTUNo")]
        public int? Rtuno { get; set; }
        [Column("IsSentToBST")]
        public bool? IsSentToBst { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        [Column("dflag")]
        public int? Dflag { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("Rtu")]
        public virtual Block Block { get; set; }
        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.Rtu))]
        public virtual Project Prj { get; set; }
        [InverseProperty("Rtu")]
        public virtual ICollection<Channel> Channel { get; set; }
        [InverseProperty("Rtu")]
        public virtual ICollection<EquipmentConfigValues> EquipmentConfigValues { get; set; }
        [InverseProperty("Rtu")]
        public virtual ICollection<Slot> Slot { get; set; }
        [InverseProperty("Rtu")]
        public virtual ICollection<SlotSubblockConnection> SlotSubblockConnection { get; set; }
    }
}
