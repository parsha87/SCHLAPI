using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashBoardScreenCriteria
    {
        [Key]
        public int Id { get; set; }
        public int? ViewId { get; set; }
        public string UserId { get; set; }
        [StringLength(100)]
        public string ElementName { get; set; }
        public int? ElementId { get; set; }
        public int? ScreenId { get; set; }
        public int? CriteriaId { get; set; }
    }
}
