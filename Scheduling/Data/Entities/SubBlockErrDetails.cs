using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SubBlockErrDetails
    {
        [Key]
        public int ErrorId { get; set; }
        [Required]
        public string ErrorDetail { get; set; }
        public bool IsError { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ImportDate { get; set; }
    }
}
