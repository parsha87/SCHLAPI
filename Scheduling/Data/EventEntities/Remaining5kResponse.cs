using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class Remaining5kResponse
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public string PendingResponse { get; set; }
        public string ParentId { get; set; }
        public int? NetworkId { get; set; }
        [Column("CMDLetters")]
        [StringLength(5)]
        public string Cmdletters { get; set; }
        public string SentResponse { get; set; }
        public int? ChildId { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public string OldResponseSent { get; set; }
        public string OriginalSentResponse { get; set; }
    }
}
