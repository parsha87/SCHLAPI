using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiAddonCardTypes
    {
        [Key]
        public int Id { get; set; }
        public string CardName { get; set; }
        public int? CardType { get; set; }
    }
}
