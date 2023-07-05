﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ZoneViewModel
    {
        public int ZoneId { get; set; }
        public int PrjId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? NoOfBlock { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MaxFlowRate { get; set; }
        public bool? UsedInPrj { get; set; }
        [StringLength(6)]
        public string DayStartTime { get; set; }
        public int? ZoneNo { get; set; }
        public int? NoOfPumpStations { get; set; }
        public int? NoOfMasterValves { get; set; }
        public int? NoOfFilterStations { get; set; }
        public int? NoOfFertCenters { get; set; }
        public int? BlockNoStartFrom { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        [StringLength(6)]
        public string OperationHours { get; set; }
        public ProjectViewModel Prj { get; set; } = new ProjectViewModel();
    }
}
