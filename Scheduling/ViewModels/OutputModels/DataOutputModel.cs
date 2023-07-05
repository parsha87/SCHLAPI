
using Scheduling.Data.Entities;
using Scheduling.ViewModels.Lib;
using System.Collections.Generic;
namespace Scheduling.ViewModels.OutputModels
{
    public class DataOutputModel
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiRtuAnalysis> Items { get; set; }
    }

    public class DtoMultiNodeNwDataFrame
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiNodeNwDataFrame> Items { get; set; }
    }

    public class DtoGwstatusData
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<GwstatusData> Items { get; set; }
    }

    public class DtoMultiNodeJoinDataFrame
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiNodeJoinDataFrame> Items { get; set; }
    }
    public class DtoMultiHandShakeReach
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiHandShakeReach> Items { get; set; }
    }

    public class DtoMultiHandShakeNonReach
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiHandShakeNonReach> Items { get; set; }
    }

    public class DtoMultiValveEvent
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiValveEvent> Items { get; set; }
    }

    public class DtoMultiValveAlarmData
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiValveAlarmData> Items { get; set; }
    }

    public class DtoMultiSensorAlarmData
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiSensorAlarmData> Items { get; set; }
    }

    public class DtoMultiNodeAlarm
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiNodeAlarm> Items { get; set; }
    }

    public class DtoMultiSensorEvent
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiSensorEvent> Items { get; set; }
    }


    public class DtoMultiDataLogger
    {
        public PagingHeader Paging { get; set; }
        public List<LinkInfo> Links { get; set; }
        public List<MultiDataLogger> Items { get; set; }

        public List<GatewayDDLViewModel> GwItems { get; set; }
    }
}
