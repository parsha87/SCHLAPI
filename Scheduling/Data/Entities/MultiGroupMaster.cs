using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiGroupMaster
    {
        [Key]
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Tagname { get; set; }
        public bool? IsActive { get; set; }
    }
}
