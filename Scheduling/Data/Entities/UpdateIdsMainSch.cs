using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UpdateIdsMainSch
    {
        [Key]
        public int Id { get; set; }
        public int? SeqMaxUpid { get; set; }
        public int? GwSeqMaxUpId { get; set; }
    }
}
