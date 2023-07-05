﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.TimestampEntities
{
    public partial class MetadataTables
    {
        public int MetadataTableId { get; set; }
        [StringLength(100)]
        public string MetadataTableName { get; set; }
    }
}
