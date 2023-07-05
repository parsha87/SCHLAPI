using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiDataLogger
    {
        [Key]
        public int Id { get; set; }
        public int? GwId { get; set; }
        public int? ProjectId { get; set; }
        [Column("DBId")]
        public int? Dbid { get; set; }
        [Column("CMDNo")]
        public int? Cmdno { get; set; }
        public int? NwUpid { get; set; }
        [Column("MaxLengthKB")]
        public int? MaxLengthKb { get; set; }
        [StringLength(100)]
        public string FrameType { get; set; }
        [Column(TypeName = "text")]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
    }
}
