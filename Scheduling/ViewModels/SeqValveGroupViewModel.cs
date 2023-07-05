using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SeqValveGroupViewModel
    {
        public SeqValveGroupViewModel()
        {
            SequenceValves = new List<KeyValuePairViewModel>();
            SequenceGroups = new List<KeyValuePairGroupViewModel>();
           // SequenecGroupsValves = new List<SeqGroupValves>();
        }

        public List<KeyValuePairViewModel> SequenceValves { get; set; }
        public List<KeyValuePairGroupViewModel> SequenceGroups { get; set; }
       // public List<SeqGroupValves> SequenecGroupsValves { get; set; }
    }

    public class SeqGroupValves
    {
        public string GroupName { get; set; }
        public List<KeyValuePairViewModel> SequenceValves { get; set; } = new List<KeyValuePairViewModel>();
    }
}
