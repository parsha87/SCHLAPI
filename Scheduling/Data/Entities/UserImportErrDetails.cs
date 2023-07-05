﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UserImportErrDetails
    {
        [Key]
        public int ErrorId { get; set; }
        public string ErrorDetail { get; set; }
        public bool? IsError { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ImportDate { get; set; }
    }
}
