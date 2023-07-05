using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ActionTypesViewModel
    {
        public int OperationCode { get; set; }
        public string Action { get; set; }
        public string ResetCondition { get; set; }
        public bool? IsActiveForRule { get; set; }
    }
}
