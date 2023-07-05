using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class PrivilegeNodeChildRelation
    {
        [Key]
        public int Id { get; set; }
        public int PrivilegeActionKey { get; set; }
        public int NodeId { get; set; }
        public int ChildId { get; set; }
        [StringLength(300)]
        public string Node { get; set; }
    }
}
