using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashBoardCriteria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public int? ViewId { get; set; }
        [StringLength(100)]
        public string Criteria { get; set; }
        public int? CriteriaValue { get; set; }
    }
}
