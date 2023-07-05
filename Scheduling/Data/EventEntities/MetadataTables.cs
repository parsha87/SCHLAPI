using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class MetadataTables
    {
        public int MetadataTableId { get; set; }
        [StringLength(100)]
        public string MetadataTableName { get; set; }
    }
}
