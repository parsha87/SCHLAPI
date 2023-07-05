using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceViewModel
    {
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        [Required]
        public DateTime? SeqStartDate { get; set; }
        [Required]
        public DateTime? SeqEndDate { get; set; }

        public string SeqMasterStartTime { get; set; }

        public bool? IsProgrammable { get; set; }
        [MaxLength(10)]
        public string BasisOfOp { get; set; }
        public int? IntervalDays { get; set; }
        public int? PrjTypeId { get; set; }
        public int? OperationTypeId { get; set; }
        [MaxLength(1)]
        public string MannualorAutoEt { get; set; }
        public bool? IsSmartEt { get; set; }
        [MaxLength(50)]
        public string SeqName { get; set; }
        public bool? ValidationState { get; set; }
        public bool? IsValid { get; set; }
        public int? SeqNo { get; set; }

        public string ZoneName { get; set; }
        public string NetworkName { get; set; }
        [MaxLength(200)]
        public string SeqDesc { get; set; }
        [MaxLength(25)]
        [Required]
        public string SeqTagName { get; set; }
        [MaxLength(3)]
        public string SeqType { get; set; }
        public int? IsSent { get; set; }
        public bool? IsDeleting { get; set; }
        public List<int> WeekDays { get; set; } = new List<int>();
        public ProjectViewModel Prj { get; set; } = new ProjectViewModel();
        public ProjectTypeViewModel PrjType { get; set; } = new ProjectTypeViewModel();
        public List<SequenceErrDetailsViewModel> SequenceErrDetails { get; set; } = new List<SequenceErrDetailsViewModel>();
        public List<SequenceMasterConfigViewModel> SequenceMasterConfig { get; set; } = new List<SequenceMasterConfigViewModel>();
        public List<SequenceValveConfigViewModel> SequenceValveConfig { get; set; } = new List<SequenceValveConfigViewModel>();
        public List<SequenceWeeklyScheduleViewModel> SequenceWeeklySchedule { get; set; } = new List<SequenceWeeklyScheduleViewModel>();
    }

    public class DatesToConfigure
    {
        public DateTime date{ get; set; }
    }

    public class ErrorMsg
    {
        public bool flag { get; set; }
        public List<string> errors { get; set; } = new List<string>();
         
    }

    public class FertGr
    {
        public int MSTFertPumpId { get; set; }
    }

    public class FilterGr
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
