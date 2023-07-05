using Scheduling.Data.EventEntities;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class MOMetaDataViewModel
    {
        public List<MOTypeViewModel> Motypes { get; set; } = new List<MOTypeViewModel>();
        public List<ActionTypesViewModel> ActionTypes { get; set; } = new List<ActionTypesViewModel>();
        public List<AlarmLevelsViewModel> AlarmLevels { get; set; } = new List<AlarmLevelsViewModel>();
        public List<RuleElementsMetadataViewModel> ElementToOverride { get; set; } = new List<RuleElementsMetadataViewModel>();
        public List<MoNetwork> Networks { get; set; } = new List<MoNetwork>();
        public List<MoZone> Zones { get; set; } = new List<MoZone>();
        public List<KeyValueViewModel> Blocks { get; set; } = new List<KeyValueViewModel>();
    }

    public class EventlogDDlViewModel    {
       
        public List<MoNetwork> Networks { get; set; } = new List<MoNetwork>();
        public List<MoZone> Zones { get; set; } = new List<MoZone>();
        public List<KeyValueViewModel> Blocks { get; set; } = new List<KeyValueViewModel>();
        public List<KeyValueViewModel> Rtus { get; set; } = new List<KeyValueViewModel>();

    }

    public class ElementsToOverride
    {
        public string ElementId { get; set; }
        public string ElementName { get; set; }
        public List<KeyValueViewModel> ElemmentNo { get; set; } = new List<KeyValueViewModel>();

    }

    public class MoNetwork
    {
        public int NetworkId { get; set; }
        public string NetworkName { get; set; }
        public int? NetworkNo { get; set; }
        public List<MoZone> Zones { get; set; } = new List<MoZone>();
        public List<KeyValueViewModel> Blocks { get; set; } = new List<KeyValueViewModel>();
    }


    public class EventLogBookZoneNwBkRtu
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<KeyValueViewModel> Blocks { get; set; } = new List<KeyValueViewModel>();
    }

    public class MoZone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<KeyValueViewModel> Blocks { get; set; } = new List<KeyValueViewModel>();
    }

    public class ElementNo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RTUId { get; set; }
    }
    public class SequenceForMO
    {
        public int SeqId { get; set; }
        public string SeqName { get; set; }
    }

    public class ValveScheduleForMO
    {
        public DateTime SeqStDate { get; set; }
        public DateTime SeqEndDate { get; set; }
        public int SeqId { get; set; }
        public int ChannelId { get; set; }
        public string valve { get; set; }
        public string SeqName { get; set; }
        public int StartTimeSpanId { get; set; }
        public string StartTime { get; set; }
        public int EndTimeSpanId { get; set; }
        public string EndTime { get; set; }

    }
}

