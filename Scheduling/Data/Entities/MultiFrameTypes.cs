using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiFrameTypes
    {
        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string AppId { get; set; }
        public int? AppIdValue { get; set; }
        [StringLength(200)]
        public string RtosFrameType { get; set; }
        public int? RtosValue { get; set; }
    }
}
