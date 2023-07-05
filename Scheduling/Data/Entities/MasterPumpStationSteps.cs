using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MasterPumpStationSteps
    {
        [Key]
        public int PumpStepConfigId { get; set; }
        [Column("MSTPumpStationId")]
        public int MstpumpStationId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        public int PumpId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DesignPumpFlow { get; set; }
        [StringLength(10)]
        public string PumpFlowUnit { get; set; }
        public int? NominalPressure { get; set; }
        [StringLength(10)]
        public string PressureUnit { get; set; }
        [StringLength(10)]
        public string StepNo { get; set; }
        public bool? Value { get; set; }
        public bool? CommFailuare { get; set; }
        public int? CommDelay { get; set; }
    }
}
